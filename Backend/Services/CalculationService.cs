using Backend.DTOs;
using Backend.Models;

namespace Backend.Services
{
    public class CalculationService : ICalculationService
    {
        private readonly IProductService _productService;
        private const decimal SensitivityPerturbation = 0.20m;

        public CalculationService(IProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        public RepairReplaceResultDto Calculate(RepairReplaceRequestDto request)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            var productType = _productService.GetProductTypeById(request.ProductTypeId)
                ?? throw new ArgumentException($"Unknown product type '{request.ProductTypeId}'.", nameof(request));

            var rule = _productService.GetRuleForProductType(request.ProductTypeId);

            return CalculateInternal(request, productType, rule);
        }

        public RepairReplaceResultDto CalculateWithRule(RepairReplaceRequestDto request, CalculationRule rule)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            if (rule is null) throw new ArgumentNullException(nameof(rule));

            // Product type may legitimately be null here (e.g. unit tests supplying
            // a rule directly) — ResolveInputs falls back to request-supplied values.
            var productType = _productService.GetProductTypeById(request.ProductTypeId);

            return CalculateInternal(request, productType, rule);
        }

        // public IReadOnlyList<RepairReplaceResultDto> CalculateBatch(IEnumerable<RepairReplaceRequestDto> requests)
        // {
        //     if (requests is null) throw new ArgumentNullException(nameof(requests));
        //     return requests.Select(Calculate).ToList();
        // }

        // ------------------------------------------------------------------
        // Core pipeline
        // ------------------------------------------------------------------

        private RepairReplaceResultDto CalculateInternal(
            RepairReplaceRequestDto request, ProductType? productType, CalculationRule rule)
        {
            var inputs = ResolveInputs(request, productType);

            var repairPath = BuildRepairPath(inputs, rule);
            var replacePath = BuildReplacePath(inputs, rule);

            var factors = EvaluateFactors(inputs, rule, repairPath, replacePath);
            var score = ComputeScore(factors);
            var verdict = DetermineVerdict(score, rule);
            var uncertainty = AssessUncertainty(inputs, rule);

            var netAdvantage = repairPath.TotalCostOfOwnership - replacePath.TotalCostOfOwnership;
            var breakEvenYears = CalculateBreakEvenYears(repairPath, replacePath);

            return new RepairReplaceResultDto
            {
                Recommendation = verdict,
                Score = Math.Round(score, 3),
                Summary = BuildSummary(verdict, factors, repairPath, replacePath),
                RepairPath = repairPath,
                ReplacePath = replacePath,
                NetAdvantage = Math.Round(netAdvantage, 2),
                BreakEvenYears = breakEvenYears,
                Factors = factors,
                Uncertainty = uncertainty,
                CurrencyCode = string.IsNullOrWhiteSpace(request.CurrencyCode) ? "USD" : request.CurrencyCode,
                CalculatedAtUtc = DateTime.UtcNow,
                RuleSetVersion = _productService.GetRuleSetVersion()
            };
        }

        // ------------------------------------------------------------------
        // Input resolution — fills defaults, records what was assumed
        // ------------------------------------------------------------------

        private ResolvedInputs ResolveInputs(RepairReplaceRequestDto request, ProductType? productType)
        {
            var assumptions = new List<string>();

            decimal replacementCost = request.ReplacementCost
                ?? productType?.TypicalReplacementCost
                ?? throw new InvalidOperationException(
                    "ReplacementCost not provided and no product type default available.");
            if (request.ReplacementCost is null)
                assumptions.Add("ReplacementCost defaulted from product type");

            int expectedLifespan = request.ExpectedLifespanYears
                ?? productType?.TypicalLifespanYears
                ?? throw new InvalidOperationException(
                    "ExpectedLifespanYears not provided and no product type default available.");
            if (request.ExpectedLifespanYears is null)
                assumptions.Add("ExpectedLifespanYears defaulted from product type");

            decimal residualValue = request.ResidualValue
                ?? (replacementCost * (productType?.ResidualValueFactor ?? 0m));
            if (request.ResidualValue is null)
                assumptions.Add("ResidualValue defaulted from product type residual factor");

            decimal currentAnnualKwh = request.Energy?.CurrentAnnualKwh
                ?? productType?.AnnualEnergyKwh ?? 0m;
            decimal replacementAnnualKwh = request.Energy?.ReplacementAnnualKwh
                ?? (currentAnnualKwh * (1m - (productType?.EnergyDegradationPerYear ?? 0m)));
            decimal energyPricePerKwh = request.Energy?.EnergyPricePerKwh ?? 0m;
            decimal energyInflation = request.Energy?.AnnualEnergyPriceInflation ?? 0m;

            if (request.Energy is null)
                assumptions.Add("Energy inputs omitted — treated as zero impact");

            return new ResolvedInputs
            {
                AgeYears = request.AgeYears,
                RepairCost = request.RepairCost,
                PriorRepairCount = request.PriorRepairCount,
                PriorRepairTotalCost = request.PriorRepairTotalCost,
                ExpectedYearsAfterRepair = Math.Max(request.ExpectedYearsAfterRepair, 1),
                ReplacementCost = replacementCost,
                ExpectedLifespanYears = Math.Max(expectedLifespan, 1),
                ResidualValue = residualValue,
                CurrentAnnualKwh = currentAnnualKwh,
                ReplacementAnnualKwh = replacementAnnualKwh,
                EnergyPricePerKwh = energyPricePerKwh,
                EnergyInflation = energyInflation,
                AssumptionsUsed = assumptions
            };
        }

        // ------------------------------------------------------------------
        // Cost paths
        // ------------------------------------------------------------------

        private CostBreakdownDto BuildRepairPath(ResolvedInputs inputs, CalculationRule rule)
        {
            int serviceYears = inputs.ExpectedYearsAfterRepair;

            decimal energyCost = CalculateEnergyCostOverHorizon(
                inputs.CurrentAnnualKwh, inputs.EnergyPricePerKwh, inputs.EnergyInflation,
                serviceYears, rule.DiscountRate);

            // Transparent heuristic, not a hidden black box: if this is already a
            // repeat repair, assume roughly one more at half cost within the horizon.
            decimal expectedFutureRepairCost = inputs.PriorRepairCount >= rule.RepeatRepairThreshold
                ? inputs.RepairCost * 0.5m
                : 0m;

            decimal upfront = inputs.RepairCost;
            decimal total = upfront + expectedFutureRepairCost + energyCost;

            var cashFlows = BuildCashFlowSeries(upfront, expectedFutureRepairCost, energyCost, serviceYears);
            decimal npv = CalculateNetPresentValue(cashFlows, rule.DiscountRate);

            return new CostBreakdownDto
            {
                PathName = "Repair",
                UpfrontCost = Math.Round(upfront, 2),
                ExpectedFutureRepairCost = Math.Round(expectedFutureRepairCost, 2),
                EnergyCostOverHorizon = Math.Round(energyCost, 2),
                ResidualValueCredit = 0m, // repairing doesn't realize a salvage value
                TotalCostOfOwnership = Math.Round(total, 2),
                NetPresentValue = Math.Round(npv, 2),
                CostPerYearOfService = serviceYears > 0 ? Math.Round(total / serviceYears, 2) : total,
                ExpectedServiceYears = serviceYears
            };
        }

        private CostBreakdownDto BuildReplacePath(ResolvedInputs inputs, CalculationRule rule)
        {
            int serviceYears = inputs.ExpectedLifespanYears;

            decimal energyCost = CalculateEnergyCostOverHorizon(
                inputs.ReplacementAnnualKwh, inputs.EnergyPricePerKwh, inputs.EnergyInflation,
                serviceYears, rule.DiscountRate);

            decimal upfront = inputs.ReplacementCost;
            decimal residualCredit = -inputs.ResidualValue; // reduces total cost
            decimal total = upfront + energyCost + residualCredit;

            var cashFlows = BuildCashFlowSeries(upfront, 0m, energyCost, serviceYears);
            decimal npv = CalculateNetPresentValue(cashFlows, rule.DiscountRate) + residualCredit;

            return new CostBreakdownDto
            {
                PathName = "Replace",
                UpfrontCost = Math.Round(upfront, 2),
                ExpectedFutureRepairCost = 0m,
                EnergyCostOverHorizon = Math.Round(energyCost, 2),
                ResidualValueCredit = Math.Round(residualCredit, 2),
                TotalCostOfOwnership = Math.Round(total, 2),
                NetPresentValue = Math.Round(npv, 2),
                CostPerYearOfService = serviceYears > 0 ? Math.Round(total / serviceYears, 2) : total,
                ExpectedServiceYears = serviceYears
            };
        }

        private List<decimal> BuildCashFlowSeries(
            decimal upfront, decimal midHorizonCost, decimal totalEnergyCost, int years)
        {
            var flows = new List<decimal> { upfront };
            if (years <= 0) return flows;

            decimal annualEnergy = totalEnergyCost / years;
            for (int year = 1; year <= years; year++)
            {
                decimal flow = annualEnergy;
                if (midHorizonCost > 0 && year == years / 2) flow += midHorizonCost;
                flows.Add(flow);
            }
            return flows;
        }

        // ------------------------------------------------------------------
        // Financial math
        // ------------------------------------------------------------------

        private decimal CalculateEnergyCostOverHorizon(
            decimal annualKwh, decimal pricePerKwh, decimal inflation, int years, decimal discountRate)
        {
            if (years <= 0 || annualKwh <= 0 || pricePerKwh <= 0) return 0m;

            decimal total = 0m;
            decimal price = pricePerKwh;

            for (int year = 1; year <= years; year++)
            {
                decimal yearCost = annualKwh * price;
                total += yearCost / DiscountFactor(discountRate, year);
                price *= (1 + inflation);
            }

            return total;
        }

        private decimal CalculateNetPresentValue(IEnumerable<decimal> cashFlows, decimal discountRate)
        {
            decimal npv = 0m;
            int period = 0;

            foreach (var flow in cashFlows)
            {
                npv += flow / DiscountFactor(discountRate, period);
                period++;
            }

            return npv;
        }

        private decimal DiscountFactor(decimal discountRate, int period) =>
            period == 0 ? 1m : (decimal)Math.Pow((double)(1 + discountRate), period);

        private decimal? CalculateBreakEvenYears(CostBreakdownDto repair, CostBreakdownDto replace)
        {
            // At what point does replace's higher upfront cost get overtaken by
            // its lower ongoing cost per year? Null if replace never wins.
            if (replace.CostPerYearOfService >= repair.CostPerYearOfService) return null;

            decimal upfrontGap = replace.UpfrontCost - repair.UpfrontCost;
            if (upfrontGap <= 0) return 0m;

            decimal annualSavings = repair.CostPerYearOfService - replace.CostPerYearOfService;
            if (annualSavings <= 0) return null;

            return Math.Round(upfrontGap / annualSavings, 1);
        }

        // ------------------------------------------------------------------
        // Scoring
        // ------------------------------------------------------------------

        private List<CalculationFactorDto> EvaluateFactors(
            ResolvedInputs inputs, CalculationRule rule, CostBreakdownDto repairPath, CostBreakdownDto replacePath)
        {
            var factors = new List<CalculationFactorDto>();

            decimal costRatio = replacePath.UpfrontCost > 0
                ? inputs.RepairCost / replacePath.UpfrontCost : 0m;
            factors.Add(BuildFactor(
                "Repair cost ratio", costRatio, rule.RepairCostRatioThreshold, rule.CostWeight,
                favorsReplace: costRatio >= rule.RepairCostRatioThreshold,
                explanation: $"Repair cost is {costRatio:P0} of replacement cost."));

            decimal ageRatio = inputs.ExpectedLifespanYears > 0
                ? (decimal)inputs.AgeYears / inputs.ExpectedLifespanYears : 0m;
            factors.Add(BuildFactor(
                "Age ratio", ageRatio, rule.AgeRatioThreshold, rule.AgeWeight,
                favorsReplace: ageRatio >= rule.AgeRatioThreshold,
                explanation: $"Unit is at {ageRatio:P0} of its expected lifespan."));

            decimal reliabilityScore = rule.RepeatRepairThreshold > 0
                ? Math.Min(1m, (decimal)inputs.PriorRepairCount / rule.RepeatRepairThreshold) : 0m;
            factors.Add(BuildFactor(
                "Repair frequency", reliabilityScore, 1m, rule.ReliabilityWeight,
                favorsReplace: inputs.PriorRepairCount >= rule.RepeatRepairThreshold,
                explanation: $"{inputs.PriorRepairCount} prior repair(s) recorded."));

            decimal energySavingsRatio = repairPath.EnergyCostOverHorizon > 0
                ? 1m - (replacePath.EnergyCostOverHorizon / repairPath.EnergyCostOverHorizon)
                : 0m;
            energySavingsRatio = Math.Clamp(energySavingsRatio, 0m, 1m);
            factors.Add(BuildFactor(
                "Energy efficiency gain", energySavingsRatio, 0.3m, rule.EnergyWeight,
                favorsReplace: energySavingsRatio >= 0.3m,
                explanation: "Replacement unit is meaningfully more energy-efficient."));

            return factors;
        }

        private CalculationFactorDto BuildFactor(
            string name, decimal value, decimal threshold, decimal weight, bool favorsReplace, string explanation)
        {
            decimal contribution = favorsReplace ? weight : 0m;

            return new CalculationFactorDto
            {
                Name = name,
                Value = Math.Round(value, 3),
                Threshold = threshold,
                Weight = weight,
                Contribution = Math.Round(contribution, 3),
                Direction = favorsReplace ? "FavorsReplace" : "FavorsRepair",
                Explanation = explanation
            };
        }

        private decimal ComputeScore(List<CalculationFactorDto> factors) =>
            Math.Clamp(factors.Sum(f => f.Contribution), 0m, 1m);

        private string DetermineVerdict(decimal score, CalculationRule rule)
        {
            if (score >= rule.ReplaceScoreThreshold) return "Replace";
            if (score <= rule.RepairScoreThreshold) return "Repair";
            return "Borderline";
        }

        // ------------------------------------------------------------------
        // Uncertainty — re-scores under perturbed inputs to check verdict stability
        // ------------------------------------------------------------------

        private UncertaintyDto AssessUncertainty(ResolvedInputs inputs, CalculationRule rule)
        {
            var baseline = ComputeScoreForInputs(inputs, rule);
            var perturbedUp = ComputeScoreForInputs(Perturb(inputs, 1 + SensitivityPerturbation), rule);
            var perturbedDown = ComputeScoreForInputs(Perturb(inputs, 1 - SensitivityPerturbation), rule);

            decimal lower = Math.Min(perturbedDown.score, perturbedUp.score);
            decimal upper = Math.Max(perturbedDown.score, perturbedUp.score);

            string baselineVerdict = DetermineVerdict(baseline.score, rule);
            bool stable =
                DetermineVerdict(lower, rule) == baselineVerdict &&
                DetermineVerdict(upper, rule) == baselineVerdict;

            var sensitiveInputs = new List<string>();
            if (Math.Abs(perturbedUp.score - perturbedDown.score) > 0.15m)
            {
                sensitiveInputs.Add("RepairCost");
                sensitiveInputs.Add("ReplacementCost");
            }

            return new UncertaintyDto
            {
                ConfidenceLevel = stable ? 0.85m : 0.5m,
                ScoreLowerBound = Math.Round(lower, 3),
                ScoreUpperBound = Math.Round(upper, 3),
                IsVerdictStable = stable,
                NetAdvantageLowerBound = Math.Round(perturbedDown.netAdvantage, 2),
                NetAdvantageUpperBound = Math.Round(perturbedUp.netAdvantage, 2),
                SensitiveInputs = sensitiveInputs,
                AssumptionsUsed = inputs.AssumptionsUsed
            };
        }

        private (decimal score, decimal netAdvantage) ComputeScoreForInputs(
            ResolvedInputs inputs, CalculationRule rule)
        {
            var repairPath = BuildRepairPath(inputs, rule);
            var replacePath = BuildReplacePath(inputs, rule);
            var factors = EvaluateFactors(inputs, rule, repairPath, replacePath);
            var score = ComputeScore(factors);
            var netAdvantage = repairPath.TotalCostOfOwnership - replacePath.TotalCostOfOwnership;
            return (score, netAdvantage);
        }

        private ResolvedInputs Perturb(ResolvedInputs inputs, decimal factor) => inputs with
        {
            RepairCost = inputs.RepairCost * factor,
            ReplacementCost = inputs.ReplacementCost * factor
        };

        // ------------------------------------------------------------------
        // Summary
        // ------------------------------------------------------------------

        private string BuildSummary(
            string verdict, List<CalculationFactorDto> factors, CostBreakdownDto repair, CostBreakdownDto replace)
        {
            var topFactor = factors.OrderByDescending(f => f.Contribution).First();

            return verdict switch
            {
                "Replace" =>
                    $"Replacing costs {replace.CostPerYearOfService:C}/year of service versus " +
                    $"{repair.CostPerYearOfService:C}/year to keep repairing. Main driver: {topFactor.Name.ToLower()}.",
                "Repair" =>
                    $"Repairing remains cheaper at {repair.CostPerYearOfService:C}/year of service " +
                    $"versus {replace.CostPerYearOfService:C}/year to replace.",
                _ =>
                    $"Repair and replace costs are close ({repair.CostPerYearOfService:C} vs " +
                    $"{replace.CostPerYearOfService:C} per year) — either is defensible."
            };
        }
    }

    /// <summary>
    /// Every optional field from the request resolved to a concrete value, with a
    /// record of which defaults were applied. Internal only — lives for the duration
    /// of one Calculate() call and is never serialized back to the client directly.
    /// Its assumptions flow into UncertaintyDto.AssumptionsUsed instead. Immutable
    /// and `with`-expression friendly so the uncertainty pass can cheaply produce
    /// perturbed copies.
    /// </summary>
    internal record ResolvedInputs
    {
        public required int AgeYears { get; init; }
        public required decimal RepairCost { get; init; }
        public required int PriorRepairCount { get; init; }
        public required decimal PriorRepairTotalCost { get; init; }
        public required int ExpectedYearsAfterRepair { get; init; }

        public required decimal ReplacementCost { get; init; }
        public required int ExpectedLifespanYears { get; init; }
        public required decimal ResidualValue { get; init; }

        public required decimal CurrentAnnualKwh { get; init; }
        public required decimal ReplacementAnnualKwh { get; init; }
        public required decimal EnergyPricePerKwh { get; init; }
        public required decimal EnergyInflation { get; init; }

        public required List<string> AssumptionsUsed { get; init; }
    }
}