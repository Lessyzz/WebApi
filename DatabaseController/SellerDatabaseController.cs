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
    public class SellerDatabaseController(EfContext context, JWTTokenSystem jWtTokenSystem, IHubContext<NotificationHub> hubContext)
    {
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;


        public async Task<Response> Register(RegisterDto _us)
        {
            var existingUser = await context.Sellers.FirstOrDefaultAsync(u => u.Number == _us.Number);
            if (existingUser != null)
            {
                return new Response("Number problem!", 400);
            }
            existingUser = await context.Sellers.FirstOrDefaultAsync(u => u.Email == _us.Email);
            if (existingUser != null)
            {
                return new Response("Email problem!", 400);
            }
            
            var user = new Seller
            {
                Username = _us.Username,
                Password = ComputeSha256Hash(_us.Password),
                Email = _us.Email,
                Number = _us.Number,
                Roles = "Seller",
                Address = _us.Address,
                Verified = false
            };
            
            await context.Sellers.AddAsync(user);
            await context.SaveChangesAsync();

            return new Response("User registered successfully!");
        }
        
        public async Task<LoginResponseDto?> Login(LoginDto _us)
        {
            var entity = await context.Sellers.FirstOrDefaultAsync(User => User.Number == _us.Number && User.Password == ComputeSha256Hash(_us.Password));
            if (entity != null) return new LoginResponseDto
            {
                Id = entity.Id,
                Username = entity.Username,
                Roles = entity.Roles,
                RefreshToken = await jWtTokenSystem.GenerateRefreshTokenAsync(entity.Id),
                AccessToken = await jWtTokenSystem.GenerateAccessTokenAsync(entity.Id, entity.Roles)
            }; 
            return null;
        } 
        
        public async Task<Response> GetAccessToken(string tokenJti, string userId)
        {
            JwtToken? jwt = await context.JwtTokens.FirstOrDefaultAsync(t => tokenJti == t.TokenJti);
            if (jwt == null) return new Response("logged_out", 400);

            var entity = await context.Sellers.FirstOrDefaultAsync(User => User.Id == userId);

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