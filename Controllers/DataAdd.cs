using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApi.Data;
using WebApi.DatabaseController;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    
    [Route("")]
    public class AAADataAddController(
        UserDatabaseController _userDatabaseController,
        ProductDatabaseController _productDatabaseController,
        CategoryDatabaseController _categoryDatabaseController,
        EfContext context,
        IReviewService reviewService
        ) : Microsoft.AspNetCore.Mvc.Controller
    {
        [HttpGet]
        [Route("/aaa")]
        public async Task<IActionResult> Index()
        {
            var jsonFiles = Directory.GetFiles("wwwroot/data/", "*.json");

            var count = await context.Users.CountAsync();
            
            foreach (var file in jsonFiles)
            {
                var content = await System.IO.File.ReadAllTextAsync(file);
                var fields = JsonConvert.DeserializeObject<List<ProductObject>>(content)!;
            
                foreach (var field in fields)
                {
                    
                    var index = new Random().Next(count);
                    var randomId = await context.Users
                        .Skip(index)
                        .Select(x => x.Id)
                        .FirstOrDefaultAsync();
                    
                    context.Products.Add(new Product
                    {
                        Images = field.Images,
                        Name = field.Name,
                        Price = field.Price,
                        Description = field.Description,
                        Discount = field.Discount,
                        Features = field.Features,
                        CategoryId = field.CategoryId,
                        Quantity = field.Quantity,
                        ProductSellerId = randomId,
                    });
                }
            }

            await context.SaveChangesAsync();

            return new OkResult();
        }
        
        
    }

    class ProductObject
    {
        public string Description;
        public int Discount;
        public float Price;
        public string Images;
        public string Name;
        public int Quantity;
        public string Features;
        public int CategoryId;
    }
}