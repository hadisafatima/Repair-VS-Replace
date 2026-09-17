using FluentValidation;
using Backend.DTOs;

namespace Backend.Validators
{
    /// <summary>
    /// Validates the optional nested energy block. Only runs when Energy is provided —
    /// omitting it entirely is valid (the calculation service treats it as zero impact).
    /// </summary>
    public class EnergyInputDtoValidator : AbstractValidator<EnergyInputDto>
    {
        public EnergyInputDtoValidator()
        {
            RuleFor(x => x.CurrentAnnualKwh)
                .GreaterThanOrEqualTo(0)
                .WithMessage("CurrentAnnualKwh cannot be negative.");

            RuleFor(x => x.ReplacementAnnualKwh)
                .GreaterThanOrEqualTo(0)
                .WithMessage("ReplacementAnnualKwh cannot be negative.");

            RuleFor(x => x.EnergyPricePerKwh)
                .GreaterThanOrEqualTo(0)
                .WithMessage("EnergyPricePerKwh cannot be negative.");

            RuleFor(x => x.AnnualEnergyPriceInflation)
                .InclusiveBetween(-0.5m, 1m)
                .When(x => x.AnnualEnergyPriceInflation.HasValue)
                .WithMessage("AnnualEnergyPriceInflation should be between -50% and 100% (as a decimal, e.g. 0.03).");
        }
    }

    /// <summary>
    /// Rejects malformed input before it reaches RepairReplaceCalculationService,
    /// so the service can assume its inputs are sane (no negative costs, no
    /// zero-length horizons) rather than defending against them internally.
    /// </summary>
    public class RepairReplaceRequestValidator : AbstractValidator<RepairReplaceRequestDto>
    {
        public RepairReplaceRequestValidator()
        {
            RuleFor(x => x.ProductTypeId)
                .NotEmpty()
                .WithMessage("ProductTypeId is required.");

            RuleFor(x => x.AgeYears)
                .GreaterThanOrEqualTo(0)
                .WithMessage("AgeYears cannot be negative.")
                .LessThanOrEqualTo(100)
                .WithMessage("AgeYears looks unrealistic — please check the value.");

            RuleFor(x => x.OriginalPurchasePrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("OriginalPurchasePrice cannot be negative.");

            RuleFor(x => x.RepairCost)
                .GreaterThan(0)
                .WithMessage("RepairCost must be greater than zero.");

            RuleFor(x => x.PriorRepairCount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("PriorRepairCount cannot be negative.");

            RuleFor(x => x.PriorRepairTotalCost)
                .GreaterThanOrEqualTo(0)
                .WithMessage("PriorRepairTotalCost cannot be negative.");

            RuleFor(x => x.ExpectedYearsAfterRepair)
                .GreaterThan(0)
                .WithMessage("ExpectedYearsAfterRepair must be at least 1.")
                .LessThanOrEqualTo(50)
                .WithMessage("ExpectedYearsAfterRepair looks unrealistic — please check the value.");

            // These three are nullable in the DTO by design — null means "use the
            // product type default". Only validate when the caller actually supplied one.
            RuleFor(x => x.ReplacementCost)
                .GreaterThan(0)
                .When(x => x.ReplacementCost.HasValue)
                .WithMessage("ReplacementCost must be greater than zero when provided.");

            RuleFor(x => x.ExpectedLifespanYears)
                .GreaterThan(0)
                .When(x => x.ExpectedLifespanYears.HasValue)
                .WithMessage("ExpectedLifespanYears must be at least 1 when provided.");

            RuleFor(x => x.ResidualValue)
                .GreaterThanOrEqualTo(0)
                .When(x => x.ResidualValue.HasValue)
                .WithMessage("ResidualValue cannot be negative when provided.");

            RuleFor(x => x.CurrencyCode)
                .Length(3)
                .When(x => !string.IsNullOrWhiteSpace(x.CurrencyCode))
                .WithMessage("CurrencyCode should be a 3-letter ISO code, e.g. USD.");

            RuleFor(x => x.Energy!)
                .SetValidator(new EnergyInputDtoValidator())
                .When(x => x.Energy is not null);
        }
    }
}