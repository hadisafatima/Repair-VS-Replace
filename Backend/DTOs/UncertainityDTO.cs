namespace Backend.DTOs
{
    public class UncertaintyDto
    {
        public decimal ConfidenceLevel { get; set; }        // 0–1
        public decimal ScoreLowerBound { get; set; }
        public decimal ScoreUpperBound { get; set; }
        public bool IsVerdictStable { get; set; }           // does verdict hold across the range?
        public decimal NetAdvantageLowerBound { get; set; }
        public decimal NetAdvantageUpperBound { get; set; }
        public List<string> SensitiveInputs { get; set; }   // inputs that most move the result
        public List<string> AssumptionsUsed { get; set; }   // defaults filled in for missing fields
    }
}
