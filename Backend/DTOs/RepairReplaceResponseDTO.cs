namespace Backend.DTOs
{
    public class RepairReplaceResultDto
    {
        public string Recommendation { get; set; }       // "Repair" | "Replace" | "Borderline"
        public decimal Score { get; set; }               // 0–1, higher = replace
        public string Summary { get; set; }              // one-line plain-English reason

        public CostBreakdownDto RepairPath { get; set; }
        public CostBreakdownDto ReplacePath { get; set; }

        public decimal NetAdvantage { get; set; }        // repair total − replace total
        public decimal? BreakEvenYears { get; set; }     // when replacing overtakes repairing

        public List<CalculationFactorDto> Factors { get; set; }
        public UncertaintyDto Uncertainty { get; set; }

        public string CurrencyCode { get; set; }
        public DateTime CalculatedAtUtc { get; set; }
        public string RuleSetVersion { get; set; }       // so results are reproducible
    }
}
