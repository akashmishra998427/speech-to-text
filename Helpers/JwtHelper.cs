using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace speect_to_text_Api.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(Dictionary<string, object> request)
        {
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.Email,
                    request["Email"]?.ToString() ?? string.Empty
                )
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes( _configuration["Jwt:Key"]! )
            );

            var credentials = new SigningCredentials( key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}