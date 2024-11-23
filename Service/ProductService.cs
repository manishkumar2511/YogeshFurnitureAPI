using YogeshFurnitureAPI.Data;
using YogeshFurnitureAPI.Interface;
using YogeshFurnitureAPI.Model.ResponseModel;
using YogeshFurnitureAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace YogeshFurnitureAPI.Service
{
    public class ProductService : IProductService
    {
        private readonly YogeshFurnitureDbContext _context;

        public ProductService(YogeshFurnitureDbContext context)
        {
            _context = context;
        }
        public async Task<Response> GetProductCategoryAsync()
        {
            var categories = await _context.Categories.AsNoTracking().ToListAsync();
            if (categories == null || !categories.Any())
            {
                return new Response(null, 0, false, (int)HttpStatusCode.NotFound);
            }
            return new Response(categories, categories.Count, true, (int)HttpStatusCode.OK);
        }

        public async Task<ResponseMessage> AddProductAsync(Product productRequest)
        {
            try
            {
                var category = await _context.Categories
                    .Where(c => c.CategoryId == productRequest.CategoryId).AsNoTracking()
                    .FirstOrDefaultAsync();

                if (category == null)
                {
                    return new ResponseMessage("Category not found", null, false, (int)HttpStatusCode.NotFound);
                }

                if (productRequest.ProductImage != null && productRequest.ProductImage.Length > 0)
                {
                    var folderPath = Path.Combine("wwwroot", "images", category.CategoryName);
                    Directory.CreateDirectory(folderPath);

                    var fileName = $"{Path.GetFileNameWithoutExtension(productRequest.ProductImage.FileName)}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(productRequest.ProductImage.FileName)}";
                    var filePath = Path.Combine(folderPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await productRequest.ProductImage.CopyToAsync(fileStream);
                    }

                    productRequest.ImageUrl = $"/images/{category.CategoryName}/{fileName}";
                }
                else
                {
                    return new ResponseMessage("Image file is required", null, false, (int)HttpStatusCode.BadRequest);
                }

                _context.Products.Add(productRequest);
                await _context.SaveChangesAsync();

                return new ResponseMessage("Product added successfully", productRequest, true, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return new ResponseMessage($"Error adding product: {ex.Message}", null, false, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ResponseMessage> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return new ResponseMessage("Product not found.", null, false, (int)HttpStatusCode.NotFound);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return new ResponseMessage("Product deleted successfully.", product, true, (int)HttpStatusCode.OK);
        }
        

        public async Task<Response> GetAllProductsAsync()
        {
            var products = await _context.Products.Include(p => p.Category).AsNoTracking().ToListAsync();
            if (products == null || !products.Any())
            {
                return new Response(null, 0, false, (int)HttpStatusCode.NotFound);
            }
            return new Response(products, products.Count, true, (int)HttpStatusCode.OK);
        }

        public async Task<Response> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _context.Products
                                          .Where(p => p.CategoryId == categoryId)
                                          .Include(p => p.Category).AsNoTracking()
                                          .ToListAsync();

            if (products == null || !products.Any())
            {
                return new Response(null, 0, false, (int)HttpStatusCode.NotFound);
            }
            return new Response(products, products.Count, true, (int)HttpStatusCode.OK);
        }

        public async Task<ResponseMessage> UpdateProductAsync(int id, Product product)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return new ResponseMessage("Product not found.", null, false, (int)HttpStatusCode.NotFound);
            }

            existingProduct.ProductName = product.ProductName;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;
            existingProduct.CategoryId = product.CategoryId;

            await _context.SaveChangesAsync();

            return new ResponseMessage("Product updated successfully.", product, true, (int)HttpStatusCode.OK);
        }
    }
}
