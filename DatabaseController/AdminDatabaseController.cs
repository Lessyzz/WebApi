using WebApi.Models;
using WebApi.Dto;
using System.Text;
using System.Security.Cryptography;
using WebApi.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Data
{
    public class AdminDatabaseController(EfContext context)
    {
        public async Task SaveJWT(JwtToken jwtToken)
        {
            await context.JwtTokens.AddAsync(jwtToken);
            await context.SaveChangesAsync();
        }
    }
}