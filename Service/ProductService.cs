using YogeshFurnitureAPI.Data;
using YogeshFurnitureAPI.Interface;
using YogeshFurnitureAPI.Model.ResponseModel;
using YogeshFurnitureAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace YogeshFurnitureAPI.Service
{
    public class ProductService : IProductService
    {
        private readonly YogeshFurnitureDbContext _context;

        public ProductService(YogeshFurnitureDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseMessage> AddProductAsync(Product productRequest)//, IFormFile image
        {
            try
            {
                //if (image != null && image.Length > 0)
                //{
                //    var category = await _context.Categories
                //        .Where(c => c.CategoryId == productRequest.CategoryId)
                //        .FirstOrDefaultAsync();

                //    if (category == null)
                //    {
                //        return new ResponseMessage("Category not found", null, false);
                //    }

                //    var folderPath = Path.Combine("wwwroot", "images", category.CategoryName);
                //    Directory.CreateDirectory(folderPath);

                //    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                //    var filePath = Path.Combine(folderPath, fileName);

                //    using (var fileStream = new FileStream(filePath, FileMode.Create))
                //    {
                //        await image.CopyToAsync(fileStream);
                //    }

                //    productRequest.ImageUrl = $"/images/{category.CategoryName}/{fileName}";
                //}

                _context.Products.Add(productRequest);
                await _context.SaveChangesAsync();

                return new ResponseMessage("Product added successfully", productRequest, true);
            }
            catch (Exception ex)
            {
                return new ResponseMessage("Error adding product", null, false);
            }
        }




        public async Task<ResponseMessage> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return new ResponseMessage("Product not found.", null, false);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return new ResponseMessage("Product deleted successfully.", product, true);
        }

        public async Task<Response> GetAllProductsAsync()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            if (products == null || !products.Any())
            {
                return new Response(null, 0, false);
            }
            return new Response(products, products.Count, true);
        }

        public async Task<Response> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _context.Products
                                          .Where(p => p.CategoryId == categoryId)
                                          .Include(p => p.Category)
                                          .ToListAsync();

            if (products == null || !products.Any())
            {
                return new Response(null, 0, false);
            }
            return new Response(products, products.Count, true);
        }

        public async Task<ResponseMessage> UpdateProductAsync(int id, Product product)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return new ResponseMessage("Product not found.", null, false);
            }

            existingProduct.ProductName = product.ProductName;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;
            existingProduct.CategoryId = product.CategoryId;

            await _context.SaveChangesAsync();

            return new ResponseMessage("Product updated successfully.", product, true);
        }
    }

}
