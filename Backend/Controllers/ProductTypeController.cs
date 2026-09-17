using System.Collections.Generic;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductTypesController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductTypesController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// GET /api/producttypes
        /// GET /api/producttypes?category=Appliance
        /// Serves the in-memory catalog loaded at startup — no per-request I/O.
        /// Lets the frontend prefill a form with defaults for the chosen product.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ProductType>), StatusCodes.Status200OK)]
        public ActionResult<IReadOnlyList<ProductType>> GetAll([FromQuery] string? category)
        {
            var results = string.IsNullOrWhiteSpace(category)
                ? _productService.GetAllProductTypes()
                : _productService.GetProductTypesByCategory(category);

            return Ok(results);
        }

        /// <summary>
        /// GET /api/producttypes/categories — distinct category list, useful
        /// for populating a filter dropdown before fetching the full catalog.
        /// </summary>
        [HttpGet("categories")]
        [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
        public ActionResult<IReadOnlyList<string>> GetCategories()
        {
            return Ok(_productService.GetCategories());
        }

        /// <summary>
        /// GET /api/producttypes/{id} — single product type by id.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductType), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ProductType> GetById(string id)
        {
            var productType = _productService.GetProductTypeById(id);

            if (productType is null)
                return NotFound(new { error = $"Product type '{id}' was not found." });

            return Ok(productType);
        }
    }
}
