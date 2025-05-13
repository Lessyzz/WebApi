
using System.Security.Cryptography;
using System.Text;
using ForgotPasswordPhoneProject.Dtos;
using ForgotPasswordPhoneProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;

namespace WebApi.Controllers;

public class SearchController(EfContext context) : Microsoft.AspNetCore.Mvc.Controller
{
        
    [HttpGet("/Search/GetSearchResults")]
    public async Task<IActionResult> GetSearchResults(string query)
    {
        if (string.IsNullOrEmpty(query) || query.Length < 2)
        {
            return Json(new { products = new object[] { }, categories = new object[] { } });
        }

        // Query products by name (case insensitive)
        var products = await context.Products
            .Include(p => p.Category)
            .Where(p => EF.Functions.Like(p.Name, $"%{query}%"))
            .Select(p => new 
            {
                p.Id,
                p.Name,
                p.Images,
                Category = new { p.Category.Id, p.Category.Name },
                DiscountedPrice = p.GetDiscountedPrice()
            })
            .Take(5)
            .ToListAsync();

        // Query categories by name (case insensitive)
        var categories = await context.Categories
            .Where(c => EF.Functions.Like(c.Name, $"%{query}%"))
            .Select(c => new { c.Id, c.Name })
            .Take(3)
            .ToListAsync();

        return Json(new { products, categories });
    }
}