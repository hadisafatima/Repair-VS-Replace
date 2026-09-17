namespace Backend.DTOs
{
    public class EnergyInputDto
    {
        public decimal CurrentAnnualKwh { get; set; }
        public decimal ReplacementAnnualKwh { get; set; }
        public decimal EnergyPricePerKwh { get; set; }
        public decimal? AnnualEnergyPriceInflation { get; set; }  // 0.03
    }
}
