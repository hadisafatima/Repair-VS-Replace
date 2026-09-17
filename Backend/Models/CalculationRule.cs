namespace Backend.Models
{
    public class CalculationRule
    {
        public string Id { get; set; }                       // "appliance-standard"
        public string AppliesToCategory { get; set; }        // "Appliance"

        // Core thresholds
        public decimal RepairCostRatioThreshold { get; set; }   // 0.50 — repair cost as % of replacement
        public decimal AgeRatioThreshold { get; set; }          // 0.75 — age as % of lifespan
        public int RepeatRepairThreshold { get; set; }          // 3 — prior repairs before "replace"

        // Weighting of each factor in the final score (should sum to 1.0)
        public decimal CostWeight { get; set; }
        public decimal AgeWeight { get; set; }
        public decimal ReliabilityWeight { get; set; }
        public decimal EnergyWeight { get; set; }

        // Financial assumptions
        public decimal DiscountRate { get; set; }            // 0.05 — for NPV of future costs
        public int AnalysisHorizonYears { get; set; }        // 10

        // Verdict banding on the 0–1 score
        public decimal ReplaceScoreThreshold { get; set; }   // ≥ 0.65 → Replace
        public decimal RepairScoreThreshold { get; set; }    // ≤ 0.35 → Repair
                                                            // between → Borderline
    }
}
