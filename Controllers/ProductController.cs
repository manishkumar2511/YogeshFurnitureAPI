using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YogeshFurnitureAPI.Data;
using YogeshFurnitureAPI.Model.ResponseModel;
using YogeshFurnitureAPI.Model;
using YogeshFurnitureAPI.Interface;
using AutoMapper;

namespace YogeshFurnitureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, ILogger<ProductController> logger, IMapper mapper)
        {
            _productService = productService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("AddProduct")]
        // [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseMessage))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorMessageWrapper))]
        public async Task<IActionResult> AddProduct(ProductDTO productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);
                var result = await _productService.AddProductAsync(product);
                if (result.IsSuccessfull)
                    return Ok(result);

                return BadRequest(new ErrorMessageWrapper { ErrorMessage = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in {controller}/{action}: {message}", nameof(ProductController), nameof(AddProduct), ex.Message);
                return BadRequest(new ErrorMessageWrapper { ErrorMessage = "Error adding product." });
            }
        }

        // GET: api/Product
        [HttpGet("GetProducts")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorMessageWrapper))]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var result = await _productService.GetAllProductsAsync();
                if (result.IsSuccessfull)
                    return Ok(result);

                return BadRequest(new ErrorMessageWrapper { ErrorMessage = "No products found." });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in {controller}/{action}: {message}", nameof(ProductController), nameof(GetProducts), ex.Message);
                return BadRequest(new ErrorMessageWrapper { ErrorMessage = "Error fetching products." });
            }
        }

        // GET: api/Product/category/{categoryId}
        [HttpGet("GetProductsByCategory/{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorMessageWrapper))]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            try
            {
                var result = await _productService.GetProductsByCategoryAsync(categoryId);
                if (result.IsSuccessfull)
                    return Ok(result);

                return BadRequest(new ErrorMessageWrapper { ErrorMessage = "No products found for the selected category." });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in {controller}/{action}: {message}", nameof(ProductController), nameof(GetProductsByCategory), ex.Message);
                return BadRequest(new ErrorMessageWrapper { ErrorMessage = "Error fetching products for the category." });
            }
        }

        // PUT: api/Product/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseMessage))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorMessageWrapper))]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            try
            {
                var result = await _productService.UpdateProductAsync(id, product);
                if (result.IsSuccessfull)
                    return Ok(result);

                return BadRequest(new ErrorMessageWrapper { ErrorMessage = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in {controller}/{action}: {message}", nameof(ProductController), nameof(UpdateProduct), ex.Message);
                return BadRequest(new ErrorMessageWrapper { ErrorMessage = "Error updating product." });
            }
        }
        // DELETE: api/Product/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseMessage))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorMessageWrapper))]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                if (result.IsSuccessfull)
                    return Ok(result);

                return BadRequest(new ErrorMessageWrapper { ErrorMessage = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in {controller}/{action}: {message}", nameof(ProductController), nameof(DeleteProduct), ex.Message);
                return BadRequest(new ErrorMessageWrapper { ErrorMessage = "Error deleting product." });
            }
        }
    }
}
