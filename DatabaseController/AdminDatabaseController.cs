using WebApi.Models;
using WebApi.Dto;
using System.Text;
using System.Security.Cryptography;
using WebApi.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Data
{
    public class AdminDatabaseController
    {
        private readonly EfContext _context;

        public AdminDatabaseController(EfContext context)
        {
            _context = _context;
        }

        public async Task SaveJWT(JwtToken jwtToken)
        {
            await _context.JwtTokens.AddAsync(jwtToken);
            await _context.SaveChangesAsync();
        }
    }
}