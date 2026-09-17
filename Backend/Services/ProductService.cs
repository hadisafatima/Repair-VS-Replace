using Backend.Models;
using Microsoft.Extensions.Options;
using Backend.Configuration;

namespace Backend.Services
{
    public class ProductService : IProductService
    {

        // readonly here means, the reference cannot be reassigned after construction
        private readonly IReadOnlyList<ProductType> _productTypes;
        private readonly IReadOnlyList<CalculationRule> _calculationRules;
        private readonly string _ruleSetVersion;

        // constructor
        public ProductService(IOptions<CalculationOptions> options)
        {
            var config = options?.Value ?? throw new ArgumentNullException(nameof(options));
 
            _productTypes = config.ProductTypes ?? new List<ProductType>();
            _calculationRules = config.CalculationRules ?? new List<CalculationRule>();
            _ruleSetVersion = string.IsNullOrWhiteSpace(config.RuleSetVersion)
                ? "unversioned"
                : config.RuleSetVersion;
 
            ValidateConfiguration(); 
            // here this method is called to make sure that the configuration aren't broken, before this service starts recieving 
            // requests.
        }



        public IReadOnlyList<ProductType> GetAllProductTypes()
        {
            return _productTypes;
        }

        public IReadOnlyList<string> GetCategories()
        {
            // OrderBy() returns values in ascending order & 
            // OrderByDescending() returns values in descending order
            return _productTypes.Select(p => p.Category).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(c => c).ToList();
        }

        public ProductType? GetProductTypeById(string id)
        {
            if(string.IsNullOrWhiteSpace(id)) {
                return null;
            }

            // it returns the 1st matching producType
            return _productTypes.FirstOrDefault(p => string.Equals(p.Id, id, StringComparison.OrdinalIgnoreCase));
        }

        public IReadOnlyList<ProductType> GetProductTypesByCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                return Array.Empty<ProductType>(); // must return an array as method definition wants an array to be returned
            }

            return _productTypes.Where(p => string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase)).ToList();
            // return _productTypes.Select(p => string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public CalculationRule? GetRuleById(string ruleId)
        {
            if (string.IsNullOrWhiteSpace(ruleId))
            {
                return null;
            }

            return _calculationRules.FirstOrDefault(r => string.Equals(r.Id, ruleId, StringComparison.OrdinalIgnoreCase));
        }

        public CalculationRule GetRuleForProductType(string productTypeId)
        {
            var productType = GetProductTypeById(productTypeId)
                ?? throw new InvalidOperationException(
                    $"Product type '{productTypeId}' does not exist.");
 
            var rule = GetRuleById(productType.CalculationRuleId);
 
            if (rule is null)
                throw new InvalidOperationException(
                    $"Product type '{productTypeId}' references calculation rule " +
                    $"'{productType.CalculationRuleId}', which is not defined in configuration. " +
                    "This is a configuration error.");
 
            return rule;
        }

        public string GetRuleSetVersion() => _ruleSetVersion;

        public bool ProductTypeExists(string id) => GetProductTypeById(id) is not null;

        /// <summary>
        /// Fails fast at startup if rules.json is internally inconsistent (a product
        /// type pointing at a rule id that doesn't exist), rather than throwing on
        /// the first request that happens to hit the broken product type.
        /// </summary>
        private void ValidateConfiguration()
        {
            foreach (var productType in _productTypes)
            {
                var ruleExists = _calculationRules.Any(r =>
                    string.Equals(r.Id, productType.CalculationRuleId, StringComparison.OrdinalIgnoreCase));
 
                if (!ruleExists)
                {
                    throw new InvalidOperationException(
                        $"Configuration error: product type '{productType.Id}' references " +
                        $"calculation rule '{productType.CalculationRuleId}', which is not " +
                        "defined in rules.json.");
                }
            }
        }
    }
}