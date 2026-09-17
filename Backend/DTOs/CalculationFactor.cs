namespace Backend.DTOs
{
    public class CalculationFactorDto
    {
        public string Name { get; set; }          // "Repair cost ratio"
        public decimal Value { get; set; }        // 0.62
        public decimal Threshold { get; set; }    // 0.50
        public decimal Weight { get; set; }       // 0.35
        public decimal Contribution { get; set; } // weighted push toward replace
        public string Direction { get; set; }     // "FavorsReplace" | "FavorsRepair" | "Neutral"
        public string Explanation { get; set; }
    }
}