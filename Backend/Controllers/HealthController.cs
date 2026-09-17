using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Backend.Services;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IProductService _productService;

        // IProductService is injected so a failure to load rules.json at startup
        // (which would already have thrown, per ProductService's validation) is
        // reflected in health checks if config is ever reloaded — cheap insurance.
        public HealthController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// GET /api/health — liveness/readiness check for deployment and monitoring.
        /// Confirms the process is up and reference data loaded successfully.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";

            return Ok(new
            {
                status = "healthy",
                timestampUtc = DateTime.UtcNow,
                version,
                ruleSetVersion = _productService.GetRuleSetVersion(),
                productTypesLoaded = _productService.GetAllProductTypes().Count
            });
        }
    }
}