using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApi.Data;
using WebApi.Hubs;
using WebApi.Models;
using WebApi.Dto;

namespace WebApi.Data
{
    public class UserDatabaseController
    {
        private readonly EfContext _context;
        private readonly JWTTokenSystem _jWTTokenSystem;
        private readonly IHubContext<NotificationHub> _hubContext;

        public UserDatabaseController(EfContext context, JWTTokenSystem jWTTokenSystem, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _jWTTokenSystem = jWTTokenSystem;
            _hubContext = hubContext;
        }
        
        
        public async Task Register(RegisterDto _us)
        {
            var user = new User
            {
                Username = _us.Username,
                Password = ComputeSha256Hash(_us.Password),
                Email = _us.Email,
                Number = _us.Number,
                Roles = "User",
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        
        public async Task<LoginResponseDto?> Login(LoginDto _us)
        {
            var entity = await _context.Users.FirstOrDefaultAsync(User => User.Username == _us.Username && User.Password == _us.Password);
            if (entity != null) return new LoginResponseDto
            {
                Id = entity.Id,
                Username = entity.Username,
                Roles = entity.Roles,
                RefreshToken = await _jWTTokenSystem.GenerateRefreshTokenAsync(entity.Id)
            }; 
            return null;
        }

        
        #region GetAccessToken

        public async Task<Response> GetAccessToken(string tokenJti, string userId)
        {
            JwtToken jwt = await _context.JwtTokens.FirstOrDefaultAsync(t => tokenJti == t.TokenJti);
            if (jwt == null) return new Response("logged_out", 400);

            var entity = await _context.Users.FirstOrDefaultAsync(User => User.Id == userId);

            if (entity != null)
            {
                var AccessToken = await _jWTTokenSystem.GenerateAccessTokenAsync(entity.Id, entity.Roles);

                var loginResponse = new LoginResponseDto {  Id = entity.Id,
                    Roles = entity.Roles,
                    AccessToken = AccessToken,
                    Username = entity.Username,
                };
                
                return new Response(loginResponse);
            }
            else return new Response("Couln't get access token!", 400);
        }

        #endregion
        
        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}