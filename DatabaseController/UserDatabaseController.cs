using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Dto;
using WebApi.Hubs;
using WebApi.Models;

namespace WebApi.DatabaseController
{
    public class UserDatabaseController(EfContext context, JWTTokenSystem jWtTokenSystem, IHubContext<NotificationHub> hubContext)
    {
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;


        public async Task<string> Register(RegisterDto _us)
        {
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Number == _us.Number);
            if (existingUser != null)
            {
                throw new Exception($"User with number {_us.Number} already exists");
            }
            existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == _us.Email);
            if (existingUser != null)
            {
                throw new Exception($"User with email {_us.Email} already exists");
            }
            
            var user = new User
            {
                Username = _us.Username,
                Password = ComputeSha256Hash(_us.Password),
                Email = _us.Email,
                Number = _us.Number,
                Roles = "User",
                Verified = false
            };
            
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            return await jWtTokenSystem.GenerateRefreshTokenAsync(user.Id,user.Username);
        }
        
        public async Task<string> Login(LoginDto _us)
        {
            var entity = await context.Users.FirstOrDefaultAsync(user => user.Number == _us.Number && user.Password == ComputeSha256Hash(_us.Password));
            
            if (entity == null) throw new Exception($"Wrong username or password");
            
            return await jWtTokenSystem.GenerateRefreshTokenAsync(entity.Id,entity.Username);
        } 
        
        public async Task<Response> GetAccessToken(string tokenJti, string userId)
        {
            JwtToken? jwt = await context.JwtTokens.FirstOrDefaultAsync(t => tokenJti == t.TokenJti);
            if (jwt == null) return new Response("logged_out", 400);

            var entity = await context.Users.FirstOrDefaultAsync(User => User.Id == userId);

            if (entity != null)
            {
                var AccessToken = await jWtTokenSystem.GenerateAccessTokenAsync(entity.Id, entity.Roles);

                var loginResponse = new LoginResponseDto {  Id = entity.Id,
                    Roles = entity.Roles,
                    AccessToken = AccessToken,
                    Username = entity.Username,
                };
                
                return new Response(loginResponse);
            }
            else return new Response("Couln't get access token!", 400);
        }
        
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