using YogeshFurnitureAPI.Model.ResponseModel;
using YogeshFurnitureAPI.Model;

namespace YogeshFurnitureAPI.Interface
{
    public interface IProductService
    {
        Task<Response> GetAllProductsAsync();
        Task<Response> GetProductCategoryAsync();
        Task<Response> GetProductsByCategoryAsync(int categoryId);
        Task<ResponseMessage> UpdateProductAsync(int id, Product product);
        Task<ResponseMessage> AddProductAsync(Product product);
        Task<ResponseMessage> DeleteProductAsync(int id);
    }
}
