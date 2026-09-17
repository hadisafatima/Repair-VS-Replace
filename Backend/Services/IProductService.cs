using System.Collections.Generic;
using Backend.Models;

namespace Backend.Services
{
    /// <summary>
    /// Read-only lookup for in-memory reference data (product types and calculation
    /// rules), loaded once at startup from configuration. No I/O per call, no user
    /// data involved — this is catalog data, not anything submitted by a client.
    /// </summary>
    public interface IProductService
    {
        IReadOnlyList<ProductType> GetAllProductTypes();

        IReadOnlyList<ProductType> GetProductTypesByCategory(string category);

        ProductType? GetProductTypeById(string id);

        bool ProductTypeExists(string id);

        /// <summary>
        /// Resolves the rule for a product type. Throws if the product type doesn't
        /// exist or if it references a rule that isn't defined — both are configuration
        /// errors, not user errors, and should surface as 500s via the exception middleware.
        /// </summary>
        CalculationRule GetRuleForProductType(string productTypeId);

        CalculationRule? GetRuleById(string ruleId);

        IReadOnlyList<string> GetCategories();

        /// <summary>
        /// Stamped onto every result so a past calculation can be explained even
        /// after the rule set has since been retuned.
        /// </summary>
        string GetRuleSetVersion();
    }
}