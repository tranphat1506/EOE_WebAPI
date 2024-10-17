using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EOE_WebAPI.Models
{
    public class GameScore
    {
        public int ScoreId { get; set; }
        public int GameId { get; set; }
        public int Score { get; set; }
        public int UserId { get; set; }
        public int AccountId { get; set; } // Thêm cột AccountId
        public string PlayerName { get; set; }
        public string DefaultPlayerName => PlayerName ?? $"Player{ScoreId}"; // Giá trị mặc định cho PlayerName

        public virtual Game Game { get; set; } // Để truy cập thông tin game
        public virtual User User { get; set; } // Để truy cập thông tin người dùng
    }

}
