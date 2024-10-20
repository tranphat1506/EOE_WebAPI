using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EOE_WebAPI.Models
{
    public class Media
    {
        public string MediaId { get; set; } // Khóa chính
        public string MediaName { get; set; }
        public string MediaType { get; set; } // Chỉ nhận các giá trị như 'gif', 'image', 'video'
        public string MediaUrl { get; set; }
        public string Tags { get; set; }

        public virtual ICollection<User> Users { get; set; } // Liên kết với người dùng
        public virtual ICollection<Game> Games { get; set; } // Liên kết với game

    }
}
