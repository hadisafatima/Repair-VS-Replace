namespace Backend.DTOs
{
    public class CostBreakdownDto
    {
        public string PathName { get; set; }             // "Repair" | "Replace"
        public decimal UpfrontCost { get; set; }
        public decimal ExpectedFutureRepairCost { get; set; }
        public decimal EnergyCostOverHorizon { get; set; }
        public decimal ResidualValueCredit { get; set; } // negative cost
        public decimal TotalCostOfOwnership { get; set; }
        public decimal NetPresentValue { get; set; }
        public decimal CostPerYearOfService { get; set; } // the key comparison number
        public int ExpectedServiceYears { get; set; }
    }
}
