using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EOE_WebAPI.Models
{
    public class Account
{
    public int AccountId { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } // Để truy cập danh sách người dùng
}
}
