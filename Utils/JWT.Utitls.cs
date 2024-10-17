using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using EOE_WebAPI.Models;

namespace EOE_WebAPI.Utils
{
    public class Payload
    {
        public string? UserId { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? Jti { get; set; } // JWT ID
    }

    public static class JWTUtils
    {
        private static IConfiguration? _configuration; // Đánh dấu là có thể null

        // Hàm khởi tạo để nhận IConfiguration
        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration)); // Ném ngoại lệ nếu null
        }

        // Hàm tạo access token
        public static string GenerateToken(string userId)
        {
            if (_configuration == null)
            {
                throw new InvalidOperationException("JWTUtils is not initialized. Call Initialize method first.");
            }

            var secretKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured."); // Kiểm tra null
            var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer is not configured."); // Kiểm tra null
            var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience is not configured."); // Kiểm tra null

            var claims = new[]
            {
                new Claim("UserId", userId), // userId là dữ liệu bên ngoài
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("iss", issuer),
                new Claim("aud", audience)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:AccessTokenDurationInDays"] ?? "1")), // Thời gian hết hạn, mặc định 15 phút
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token); // Trả về token dưới dạng chuỗi
        }

        // Hàm tạo refresh token
        public static string GenerateRefreshToken(string userId)
        {
            if (_configuration == null)
            {
                throw new InvalidOperationException("JWTUtils is not initialized. Call Initialize method first.");
            }

            var secretKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured."); // Kiểm tra null
            var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer is not configured."); // Kiểm tra null
            var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience is not configured."); // Kiểm tra null

            var claims = new[]
            {
                new Claim("UserId", userId), // userId là dữ liệu bên ngoài
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("iss", issuer),
                new Claim("aud", audience)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:RefreshTokenDurationInDays"] ?? "7")), // Thời gian hết hạn, mặc định 15 phút
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token); // Trả về token dưới dạng chuỗi
        }

        // Hàm lấy payload từ token
        public static Payload GetPayload(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token)); // Kiểm tra token có null hoặc trắng
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            // Kiểm tra jwtToken có null không
            if (jwtToken == null)
            {
                throw new SecurityTokenException("Invalid token."); // Ném ngoại lệ nếu token không hợp lệ
            }

            // Lấy các claim từ jwtToken
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var issuer = jwtToken.Claims.FirstOrDefault(c => c.Type == "iss")?.Value;
            var audience = jwtToken.Claims.FirstOrDefault(c => c.Type == "aud")?.Value;
            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            // Trả về một đối tượng Payload
            return new Payload
            {
                UserId = userId,
                Issuer = issuer,
                Audience = audience,
                Jti = jti
            };
        }

        // Hàm lấy Principal từ token đã hết hạn
        public static ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (securityToken == null)
            {
                throw new SecurityTokenException("Invalid token."); // Ném ngoại lệ nếu token không hợp lệ
            }

            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!); // Lấy key từ configuration

            // Tạo validation parameters
            var validationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = false, // Bỏ qua thời gian hết hạn
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _); // Trả về ClaimsPrincipal
        }
    }
}
