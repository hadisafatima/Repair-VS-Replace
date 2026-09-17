using System.Collections.Generic;
using Backend.DTOs;
using Backend.Models;

namespace Backend.Services
{
    /// <summary>
    /// Pure calculation over the data in the request — nothing is read from or
    /// written to storage. Every call is self-contained: same request in, same
    /// result out, nothing retained after the response is returned.
    /// </summary>
    public interface ICalculationService
    {
        /// <summary>
        /// Resolves the product type's rule internally via IProductService, then calculates.
        /// This is what CalculationsController should call.
        /// </summary>
        RepairReplaceResultDto Calculate(RepairReplaceRequestDto request);

        /// <summary>
        /// Calculates using an explicitly supplied rule instead of resolving one from
        /// the product type. This is what makes the service testable without stubbing
        /// IProductService for every test case.
        /// </summary>
        RepairReplaceResultDto CalculateWithRule(RepairReplaceRequestDto request, CalculationRule rule);

        /// <summary>
        /// Optional convenience for comparing several requests in one call
        /// (e.g. "which of these five appliances should I replace first").
        /// </summary>
        /// 
        // IReadOnlyList<RepairReplaceResultDto> CalculateBatch(IEnumerable<RepairReplaceRequestDto> requests);
    }
}