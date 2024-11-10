
using YogeshFurnitureAPI.Model.Account;
using YogeshFurnitureAPI.Model.ResponseModel;

namespace YogeshFurnitureAPI.Interface.Account
{
    public interface IAccountService
    {
        Task<ResponseMessage> LoginAsync(LoginRequest request);
        Task<ResponseMessage> CreateYogeshFurnitureUsersAsync(CreateYogeshFurnitureUsers request);
    }
}
