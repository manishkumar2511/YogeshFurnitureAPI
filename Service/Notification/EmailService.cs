using System.Net;
using System.Net.Mail;
using YogeshFurnitureAPI.Interface.Notification;
using YogeshFurnitureAPI.Model.NotificationModel;

namespace YogeshFurnitureAPI.Service.Notification
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(SendEmailDTO sendEmailDTO)
        {
            try
            {
                var userName = _configuration["SMTP:UserName"];
                var password = _configuration["SMTP:Password"];
                var displayName = _configuration["Email:DisplayName"];

                // Ensure the email template path is correctly set
                var templatePath = Path.Combine(AppContext.BaseDirectory, "Helper", "Services", "Templates", "EmailTemplate.html");

                if (!File.Exists(templatePath))
                    throw new FileNotFoundException("Email template not found at: " + templatePath);

                var emailBody = await File.ReadAllTextAsync(templatePath);

                // Replace placeholders in the email body
                var fullName = $"{sendEmailDTO.FirstName} {sendEmailDTO.LastName}".Trim();
                emailBody = emailBody.Replace("{{FullName}}", !string.IsNullOrEmpty(fullName) ? fullName : "N/A")
                                     .Replace("{{SenderEmail}}", sendEmailDTO.To ?? "N/A")
                                     .Replace("{{PhoneNumber}}", sendEmailDTO.PhoneNumber ?? "N/A")
                                     .Replace("{{MessageContent}}", sendEmailDTO.Body ?? "N/A");

                using var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(userName, password)
                };

                var message = new MailMessage
                {
                    From = new MailAddress(userName, displayName),
                    Subject = sendEmailDTO.Subject,
                    Body = emailBody,
                    IsBodyHtml = true
                };

                message.To.Add(sendEmailDTO.To);

                await client.SendMailAsync(message);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }

    }
}
