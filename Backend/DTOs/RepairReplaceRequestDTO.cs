namespace Backend.DTOs
{
    public class RepairReplaceRequestDto
    {
        public string ProductTypeId { get; set; }
        public int AgeYears { get; set; }
        public decimal OriginalPurchasePrice { get; set; }

        public decimal RepairCost { get; set; }
        public int PriorRepairCount { get; set; }
        public decimal PriorRepairTotalCost { get; set; }
        public int ExpectedYearsAfterRepair { get; set; }   // how long the fix should last

        public decimal? ReplacementCost { get; set; }       // null → use product type default
        public int? ExpectedLifespanYears { get; set; }     // null → use product type default
        public decimal? ResidualValue { get; set; }         // trade-in / scrap value

        public EnergyInputDto? Energy { get; set; }          // optional

        // public string CurrencyCode { get; set; }            // "PKR", "USD"
    }
}