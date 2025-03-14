using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Data;

namespace WebApi.Data
{
    public class JWTTokenSystem
    {
        private readonly AdminDatabaseController _databaseService;
        private readonly JwtConfiguration _jwtConfiguration;
        
        public JWTTokenSystem(AdminDatabaseController _databaseService, JwtConfiguration _jwtConfiguration)
        {
            this._jwtConfiguration = _jwtConfiguration;
            this._databaseService = _databaseService;
        }

        private string GenerateJwt(IEnumerable<Claim> claims, DateTime expires)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfiguration.Key);

            var token = new JwtSecurityToken
            (
                _jwtConfiguration.Issuer,
                _jwtConfiguration.Audience,
                claims,
                DateTime.UtcNow,
                expires,
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );


            return tokenHandler.WriteToken(token);
        }

        public async Task<string> GenerateRefreshTokenAsync(string userId)
        {
            var tokenJti = Guid.NewGuid().ToString();
            var validUntil = DateTime.UtcNow.AddMonths(3);

            var claims = new List<Claim>
            {
                new("jti", tokenJti),
                new("sid", userId),
                new("rol", "RefreshToken")
            };

            var token = GenerateJwt(claims, validUntil);

            await _databaseService.SaveJWT(new JwtToken
            { TokenJti = tokenJti, UserId = userId, ValidUntil = validUntil });

            return token;
        }

        public async Task<string> GenerateAccessTokenAsync(string userId, string roleName)
        {
            var tokenJti = Guid.NewGuid().ToString();
            var validUntil = DateTime.UtcNow.AddMinutes(15);

            var claims = new List<Claim>
            {
                new("jti", tokenJti),
                new("sid", userId),
                // new("rol", roleName),
            };
            
            var roles = roleName.Split(',');
            foreach (var role in roles)
            {
                claims.Add(new Claim("rol", role.Trim()));
            }

            var token = GenerateJwt(claims, validUntil);
            return token;
        }
    }

    public class JwtConfiguration
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
    }

    public class JwtToken
    {
        [Key]
        public required string TokenJti { get; set; }

        public required string UserId { get; set; }
        public DateTime ValidUntil { get; set; }
    }
}