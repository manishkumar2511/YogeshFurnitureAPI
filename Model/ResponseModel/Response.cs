namespace YogeshFurnitureAPI.Model.ResponseModel
{
    public class Response
    {
        public Response(object? data, int? totalCount, bool isSuccessfull)
        {
            Data = data;
            TotalCount = totalCount;
            IsSuccessfull = isSuccessfull;
        }
        public object? Data { get; set; }
        public int? TotalCount { get; set; }
        public bool IsSuccessfull { get; set; }
    }
    public class ResponseMessage
    {
        public ResponseMessage(string message, object? data, bool isSuccessfull)
        {
            Message = message;
            Data = data;
            IsSuccessfull = isSuccessfull;
        }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public bool IsSuccessfull { get; set; }
    }
}
