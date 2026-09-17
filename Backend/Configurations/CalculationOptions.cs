using Backend.Models;
namespace Backend.Configuration
{
    public class CalculationOptions
    {
        public string RuleSetVersion { get; set; } = string.Empty;
        public List<ProductType> ProductTypes { get; set; } = new();
        public List<CalculationRule> CalculationRules { get; set; } = new();
    }
}