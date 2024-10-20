namespace EOE_WebAPI.Models.ResponsePayload
{
    
    public class ApiResponse<TPayload>
    {
        public int HttpCode { get; set; }
        public string Message { get; set; }
        public TPayload? Payload { get; set; }

        public ApiResponse(int statusCode, string message, TPayload payload) 
        { 
            HttpCode = statusCode;
            Message = message;
            Payload = payload;
        }
    }
}
