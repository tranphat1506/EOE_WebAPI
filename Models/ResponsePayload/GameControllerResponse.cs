namespace EOE_WebAPI.Models.ResponsePayload
{
    public class GetGameResponse
    {
        public int GameId { get; set; }
        public string Game_Name { get; set; }
        public string Game_Desc { get; set; }
        public string Game_Image { get; set; } // Giá trị có thể là null
        public string GameImageId { get; set; }
        public MediaResponse GameImage { get; set; } // Tham chiếu đến mô hình MediaResponse
    }

    public class MediaResponse
    {
        public string MediaId { get; set; }
        public string MediaName { get; set; }
        public string MediaType { get; set; }
        public string MediaUrl { get; set; }
        public string Tags { get; set; }
    }
}
