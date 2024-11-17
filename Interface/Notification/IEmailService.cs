using YogeshFurnitureAPI.Model.NotificationModel;

namespace YogeshFurnitureAPI.Interface.Notification
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(SendEmailDTO sendEmailDTO);
    }
}
