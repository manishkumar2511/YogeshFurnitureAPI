using YogeshFurnitureAPI.Model.ResponseModel;
using YogeshFurnitureAPI.Model;

namespace YogeshFurnitureAPI.Interface
{
    public interface IProductService
    {
        Task<Response> GetAllProductsAsync();
        Task<Response> GetProductsByCategoryAsync(int categoryId);
        Task<ResponseMessage> UpdateProductAsync(int id, Product product);
        Task<ResponseMessage> AddProductAsync(Product product, IFormFile image);//, IFormFile image
        Task<ResponseMessage> DeleteProductAsync(int id);
    }
}
