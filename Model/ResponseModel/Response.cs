namespace YogeshFurnitureAPI.Model.ResponseModel
{
    public class Response
    {
        public Response(object? data, int? totalCount, bool isSuccessfull, int statusCode)
        {
            Data = data;
            TotalCount = totalCount;
            IsSuccessfull = isSuccessfull;
            StatusCode = statusCode;
        }

        public object? Data { get; set; }
        public int? TotalCount { get; set; }
        public bool IsSuccessfull { get; set; }
        public int StatusCode { get; set; }  
    }

    public class ResponseMessage
    {
        public ResponseMessage(string message, object? data, bool isSuccessfull, int statusCode)
        {
            Message = message;
            Data = data;
            IsSuccessfull = isSuccessfull;
            StatusCode = statusCode;  
        }

        public string? Message { get; set; }
        public object? Data { get; set; }
        public bool IsSuccessfull { get; set; }
        public int StatusCode { get; set; }  
    }
}
