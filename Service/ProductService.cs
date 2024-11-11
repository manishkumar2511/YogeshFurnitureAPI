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
