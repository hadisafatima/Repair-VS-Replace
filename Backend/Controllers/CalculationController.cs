using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Backend.DTOs;
using Backend.Services;

namespace RepairReplaceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalculationController : ControllerBase
    {
        private readonly ICalculationService _calculationService;
        private readonly IValidator<RepairReplaceRequestDto> _validator;
        private readonly ILogger<CalculationController> _logger;

        public CalculationController(
            ICalculationService calculationService,
            IValidator<RepairReplaceRequestDto> validator,
            ILogger<CalculationController> logger)
        {
            _calculationService = calculationService;
            _validator = validator;
            _logger = logger;
        }

        /// <summary>
        /// POST /api/calculations — runs one repair-vs-replace calculation.
        /// Nothing here is persisted: the request is used to build a result and
        /// both are discarded once the response is sent. Only the route, status
        /// code, and product type are logged — never the full request/result body.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(RepairReplaceResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RepairReplaceResultDto>> Calculate([FromBody] RepairReplaceRequestDto request)
        {
            if (request is null)
                return BadRequest(new { error = "Request body is required." });

            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                return ValidationProblem(ModelState);
            }

            try
            {
                var result = _calculationService.Calculate(request);

                _logger.LogInformation(
                    "Calculation completed for product type {ProductTypeId} -> {Recommendation}",
                    request.ProductTypeId, result.Recommendation);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                // Unknown product type, missing required field with no default, etc.
                // Client's fault, not the server's — 400, not 500.
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// POST /api/calculations/batch — same calculation, run over several
        /// requests in one call (e.g. comparing multiple appliances at once).
        /// Capped to keep response time and payload size predictable.
        /// </summary>
        
        // [HttpPost("batch")]
        // [ProducesResponseType(typeof(IReadOnlyList<RepairReplaceResultDto>), StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<ActionResult<IReadOnlyList<RepairReplaceResultDto>>> CalculateBatch(
        //     [FromBody] List<RepairReplaceRequestDto> requests)
        // {
        //     const int maxBatchSize = 20;

        //     if (requests is null || requests.Count == 0)
        //         return BadRequest(new { error = "At least one request is required." });

        //     if (requests.Count > maxBatchSize)
        //         return BadRequest(new { error = $"Batch size exceeds the maximum of {maxBatchSize} requests." });

        //     for (int i = 0; i < requests.Count; i++)
        //     {
        //         var validationResult = await _validator.ValidateAsync(requests[i]);
        //         if (!validationResult.IsValid)
        //         {
        //             foreach (var error in validationResult.Errors)
        //                 ModelState.AddModelError($"[{i}].{error.PropertyName}", error.ErrorMessage);
        //         }
        //     }

        //     if (!ModelState.IsValid)
        //         return ValidationProblem(ModelState);

        //     try
        //     {
        //         var results = _calculationService.CalculateBatch(requests);

        //         _logger.LogInformation("Batch calculation completed for {Count} requests", requests.Count);

        //         return Ok(results);
        //     }
        //     catch (ArgumentException ex)
        //     {
        //         return BadRequest(new { error = ex.Message });
        //     }
        // }
    }
}