using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.DatabaseController;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class Controller(UserDatabaseController _userDatabaseController, ProductDatabaseController _productDatabaseController, CategoryDatabaseController _categoryDatabaseController, EfContext _context) : Microsoft.AspNetCore.Mvc.Controller
    {
        [HttpGet]
        [Route("/")]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            ViewBag.Products = await _productDatabaseController.GetProducts();
            var categories = await _categoryDatabaseController.GetCategories();

            var groupedCategories = categories
                .Where(c => c.ParentCategoryId == null) // Ana kategoriler
                .Select(category => new
                {
                    Category = category,
                    Subcategories = categories.Where(c => c.ParentCategoryId == category.Id).ToList() // Alt kategoriler
                })
                .ToList();

            ViewBag.GroupedCategories = groupedCategories;
            return View();
        }
        
        [HttpGet]
        [Route("/Product/{id}")]
        public async Task<IActionResult> Product(string id)
        {
            var product = await _productDatabaseController.GetProductById(id);
            if (product == null) return NotFound();
            return View(product);
        }
        
        [HttpGet]
        [Route("/Basket")]
        public IActionResult Basket()
        {
            return View();
        }

        [HttpGet("/Checkout")]
        public async Task<IActionResult> CheckoutAsync()
        {
            var userId = User.FindFirstValue("sid")!;
            var basketProducts = await _productDatabaseController.GetBasketProducts(userId);
            ViewBag.BasketProducts = basketProducts;
            return View();
        }
        
        [HttpGet("/Success")]
        public IActionResult Success()
        {
            var basketProducts = JsonSerializer.Deserialize<List<BasketProduct>>(TempData["BasketProducts"].ToString());
            var paymentInfo = JsonSerializer.Deserialize<PaymentInfoDto>(TempData["PaymentInfo"].ToString());
            ViewBag.BasketProducts = basketProducts;
            ViewBag.PaymentInfo = paymentInfo;
            return View();
        }

        [HttpGet("/Category/{id}")]
        public async Task<IActionResult> Category(int id)
        {
            var categories = await _categoryDatabaseController.GetCategories();
            var category = categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();

            var products = await _productDatabaseController.GetProductsByCategoryId(id);
            
            ViewBag.Products = products;
            ViewBag.Category = category;
            
            SetFeatures(category, products);
            
            return View();
        }

        [HttpGet("/Orders")]
        public async Task<IActionResult> Orders()
        {
            var userId = User.FindFirstValue("sid")!;
            var orders = await _context.PaidProducts
            .Where(o => o.UserId == userId)
            .Include(o => o.Product)
            .ToListAsync();
            return View(orders);
        }

        private void SetFeatures(Category category, List<Product> products)
        {
            Dictionary<string, List<string>> featureOptions = new Dictionary<string, List<string>>();
    
            if (!string.IsNullOrEmpty(category.Features))
            {
                var features = category.Features.Split(',').Select(f => f.Trim()).ToArray();
        
                foreach (var feature in features)
                {
                    featureOptions[feature] = new List<string>();
                }
        
                foreach (var product in products)
                {
                    try
                    {
                        var productFeatures = JsonSerializer.Deserialize<Dictionary<string, string>>(product.Features);
                
                        foreach (var feature in features)
                        {
                            if (productFeatures.ContainsKey(feature) && !featureOptions[feature].Contains(productFeatures[feature]))
                            {
                                featureOptions[feature].Add(productFeatures[feature]);
                            }
                        }
                    }
                    catch (JsonException)
                    {
                        continue;
                    }
                }
            }
    
            ViewBag.FeatureOptions = featureOptions;
        }
    }
}