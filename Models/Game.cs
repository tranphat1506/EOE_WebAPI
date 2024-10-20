using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EOE_WebAPI.Models
{
    public class Game
    {
        public int GameId { get; set; }
        public string Game_Name { get; set; }
        public string Game_Desc { get; set; }
        public string? Game_Image { get; set; }
        public string? GameImageId { get; set; }

        public virtual ICollection<GameScore> GameScores { get; set; } // Để truy cập danh sách điểm số
        public virtual Media? GameImage { get; set; } // Để truy cập GameImageId
    }

}
