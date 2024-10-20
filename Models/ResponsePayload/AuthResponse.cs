namespace EOE_WebAPI.Models.ResponsePayload
{
    public class LoginResponse
    {
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DisplayName { get; set; }
        public DateTime? Birth { get; set; }
        public bool Sex { get; set; }
        
        public ImageResponse? ProfileImage { get; set; }
    }

    public class ImageResponse
    {
        public string MediaId { get; set; } // Khóa chính
        public string MediaName { get; set; }
        public string MediaType { get; set; } // Chỉ nhận các giá trị như 'gif', 'image', 'video'
        public string MediaUrl { get; set; }
        public string Tags { get; set; }
    }
}
