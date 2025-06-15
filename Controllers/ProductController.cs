using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YogeshFurnitureAPI.Data;
using YogeshFurnitureAPI.Model.ResponseModel;
using YogeshFurnitureAPI.Model;
using YogeshFurnitureAPI.Interface;
using AutoMapper;
using System.Security.Claims;

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

        private string GetUserRole()
        {
            var roleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == roleClaimType)?.Value;
            _logger.LogInformation("Determined role: {Role}", role);
            return role;
        }

        [HttpGet("GetProductCategory")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorMessageWrapper))]
        public async Task<IActionResult> GetProdtCategory()
        {
            try
            {
                var result = await _productService.GetProductCategoryAsync();

                if (result.IsSuccessfull)
                    return Ok(result);

                return BadRequest(new ErrorMessageWrapper { ErrorMessage = "No products found for the selected category." });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in {controller}/{action}: {message}", nameof(ProductController), nameof(GetProductsByProductId), ex.Message);
                return BadRequest(new ErrorMessageWrapper { ErrorMessage = "Error fetching products for the category." });
            }
        }

        // POST: api/Product/AddProduct
        [HttpPost("AddProduct")]
        //[Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseMessage))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorMessageWrapper))]
        public async Task<IActionResult> AddProduct(ProductDTO productDto)
        {
            try
            {
                var userRole = GetUserRole(); 
                _logger.LogInformation("User with role {Role} accessed GetProductCategory.", userRole);

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

        // GET: api/Product/GetProducts
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

        // GET: api/Product/GetProductsByCategory/{categoryId}
        [HttpGet("GetProductsByProductId/{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorMessageWrapper))]
        public async Task<IActionResult> GetProductsByProductId(int productId)
        {
            try
            {
                var result = await _productService.GetProductsByProductIdAsync(productId);

                if (result.IsSuccessfull)
                    return Ok(result);

                return BadRequest(new ErrorMessageWrapper { ErrorMessage = "No products found for the selected productId." });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in {controller}/{action}: {message}", nameof(ProductController), nameof(GetProductsByProductId), ex.Message);
                return BadRequest(new ErrorMessageWrapper { ErrorMessage = "Error fetching products for the productId." });
            }
        }

        // PUT: api/Product/UpdateProduct/{id}
        [HttpPut("UpdateProduct/{id}")]
        //[Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseMessage))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorMessageWrapper))]
        public async Task<IActionResult> UpdateProduct(int id, ProductDTO productDto)
        {
            try
            {
                var userRole = GetUserRole(); 
                _logger.LogInformation("User with role {Role} accessed GetProductCategory.", userRole);

                var product = _mapper.Map<Product>(productDto);
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
       // [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseMessage))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorMessageWrapper))]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var userRole = GetUserRole(); 
                _logger.LogInformation("User with role {Role} accessed GetProductCategory.", userRole);

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
