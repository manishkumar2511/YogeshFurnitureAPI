namespace YogeshFurnitureAPI.Model.NotificationModel
{
    public class SendEmailDTO
    {
        public SendEmailDTO(string? to, string? subject, string? body)
        {
            To = to;
            Subject = subject;
            Body = body;
        }

        public string? To { get; set; } 
        public string? Subject { get; set; } 
        public string? Body { get; set; }

        //for query
        public string ? FirstName { get; set; }
        public string ? LastName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
