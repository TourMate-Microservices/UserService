using Microsoft.IdentityModel.Tokens;
using TourMate.UserService.Repositories.IRepositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;



namespace TourMate.UserService.Services.Utils
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateAccessToken(int accountId, string fullName, string roleName, int keyId)
        {
            // Lấy khóa bảo mật từ cấu hình
            var key = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
                throw new InvalidOperationException("Jwt:Key is missing.");

            // Kiểm tra và parse thời gian hết hạn token (phút)
            if (!int.TryParse(_config["Jwt:AccessTokenExpireMinutes"], out var accessExpireMinutes) || accessExpireMinutes <= 0)
                accessExpireMinutes = 10;

            var claims = new[]
            {
                new Claim("AccountId", accountId.ToString()),
                new Claim("FullName", fullName),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("SuppliedId", keyId.ToString()),
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);



            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(accessExpireMinutes),
                signingCredentials: creds);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
