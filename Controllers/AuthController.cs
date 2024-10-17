using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using EOE_WebAPI.Utils;
using EOE_WebAPI.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EOE_WebAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        private readonly EOEDbContext _db;

        public AuthController(ILogger<AuthController> logger, IConfiguration configuration, EOEDbContext db)
        {
            _logger = logger;
            _configuration = configuration;
            _db = db;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                // Tìm tài khoản trong cơ sở dữ liệu
                var account = _db.Account.Include(a => a.User)
                    .FirstOrDefault(u => u.Email == request.Email);
                if (account == null || account.User == null || !BCrypt.Net.BCrypt.Verify(request.Password, account.Password))
                {
                    return Unauthorized(); // Trả về 401 nếu thông tin đăng nhập không hợp lệ
                }
                User user = account.User;
                // Tạo access token
                var token = JWTUtils.GenerateToken(user.UserId.ToString());
                var refreshToken = JWTUtils.GenerateRefreshToken(user.UserId.ToString()); // Giả sử bạn có hàm này

                // Trả về thông tin người dùng, access token và refresh token
                return Ok(new
                {
                    user.UserId,
                    account.Email,
                    user.DisplayName,
                    user.Sex,
                    Birth = user.Birth.ToString(),
                    AccessToken = token,
                    RefreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng nhập.");
                return StatusCode(500, "Đã xảy ra lỗi trong quá trình đăng nhập.");
            }
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                // Kiểm tra refresh token (đơn giản)
                var principal = JWTUtils.GetPrincipalFromExpiredToken(refreshToken);
                var userId = principal.FindFirst("UserId");
                if (principal == null || userId == null)
                {
                    return Unauthorized(); // Trả về 401 nếu refresh token không hợp lệ
                }

                var newAccessToken = JWTUtils.GenerateToken(userId.ToString());
                var newRefreshToken = JWTUtils.GenerateRefreshToken(userId.ToString()); // Tạo refresh token mới

                return Ok(new
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi làm mới token.");
                return StatusCode(500, "Đã xảy ra lỗi trong quá trình làm mới token.");
            }
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            // Kiểm tra hợp lệ của model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Trả về lỗi nếu model không hợp lệ
            }

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Kiểm tra nếu email đã tồn tại
                var existingAccount = await _db.Account.FirstOrDefaultAsync(a => a.Email == request.Email);
                if (existingAccount != null)
                {
                    return BadRequest("Email đã được sử dụng."); // Trả về 400 nếu email đã tồn tại
                }

                // Tạo tài khoản mới
                var newAccount = new Account
                {
                    Email = request.Email!,
                    Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    CreatedAt = DateTime.Now
                };
                await _db.Account.AddAsync(newAccount);
                await _db.SaveChangesAsync();

                // Tạo người dùng mới
                var newUser = new User
                {
                    AccountId = newAccount.AccountId,
                    DisplayName = request.DisplayName!,
                    Birth = request.Birth,
                    Sex = request.Sex
                };
                await _db.User.AddAsync(newUser);
                await _db.SaveChangesAsync();

                // Cam kết transaction
                await transaction.CommitAsync();

                return Ok(new { messages = "Đăng ký thành công." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng ký.");
                await transaction.RollbackAsync(); // Quay lại transaction nếu có lỗi
                return StatusCode(500, "Đã xảy ra lỗi trong quá trình đăng ký."); // Trả về 500 nếu có lỗi không xác định
            }
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                // Tìm tài khoản
                var account = _db.Account.FirstOrDefault(u => u.Email == request.Email);
                if (account == null)
                {
                    return BadRequest("Email không tồn tại."); // Trả về 400 nếu email không tồn tại
                }

                // Gửi email (logic gửi email không có trong ví dụ này)
                // EmailUtils.SendEmail(account.Email, resetToken); // Gửi email

                return Ok("Một email đã được gửi đến bạn để đặt lại mật khẩu.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email đặt lại mật khẩu.");
                return StatusCode(500, "Đã xảy ra lỗi trong quá trình gửi email.");
            }
        }
    }

    // Model cho yêu cầu đăng nhập
    public class LoginRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    // Model cho yêu cầu đăng ký
    public class SignUpRequest
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Tên hiển thị là bắt buộc.")]
        public string? DisplayName { get; set; }

        [Required(ErrorMessage = "Ngày sinh là bắt buộc.")]
        public DateTime? Birth { get; set; }

        [Required(ErrorMessage = "Giới tính là bắt buộc.")]
        public bool? Sex { get; set; }
    }

    // Model cho yêu cầu quên mật khẩu
    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
    }
}
