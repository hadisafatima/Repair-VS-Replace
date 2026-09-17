namespace Backend.Models
{
    public class ProductType
    {
        public string Id { get; set; }                  // "refrigerator"
        public string DisplayName { get; set; }         // "Refrigerator"
        public string Category { get; set; }            // "Appliance"
        public int TypicalLifespanYears { get; set; }   // 13
        public decimal TypicalReplacementCost { get; set; }
        public decimal AnnualEnergyKwh { get; set; }        // modern unit baseline
        public decimal EnergyDegradationPerYear { get; set; } // % efficiency lost annually
        public decimal ResidualValueFactor { get; set; }    // salvage/trade-in as % of current value
        public string CalculationRuleId { get; set; }       // which rule set applies
    }
}
