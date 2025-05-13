using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.DatabaseController;
using WebApi.Dto;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    public class Controller(
        UserDatabaseController _userDatabaseController,
        ProductDatabaseController _productDatabaseController,
        CategoryDatabaseController _categoryDatabaseController,
        EfContext _context,
        IReviewService reviewService
        ) : Microsoft.AspNetCore.Mvc.Controller
    {
        [HttpGet]
        [Route("/")]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var parentCategories = await _context.Categories
                .Where(c => c.ParentCategoryId == null)
                .ToListAsync();
;           ViewBag.Categories = parentCategories;

            var products = await _context.Products.ToListAsync();
            ViewBag.Products = products;
            
            return View();
        }
        
        [HttpGet]
        [Route("/Product/{id}")]
        public async Task<IActionResult> Product(string id)
        {
            try
            {
                var product = await _productDatabaseController.GetProductById(id);
                
                var reviewStatistics = await reviewService.GetProductReviewsAsync(id);
            
                ViewBag.ReviewStatistics = reviewStatistics;
                
                return View(product);
            }
            catch (Exception e)
            {
                return Redirect("/");
            }
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

            // Get parent category features too
            List<string> allFeatures = new List<string>();
            if (!string.IsNullOrEmpty(category.Features))
            {
                allFeatures.AddRange(category.Features.Split(',').Select(f => f.Trim()));
            }

            // Add parent category features if they exist
            if (category.ParentCategoryId.HasValue)
            {
                var parentCategory = categories.FirstOrDefault(c => c.Id == category.ParentCategoryId.Value);
                if (parentCategory != null && !string.IsNullOrEmpty(parentCategory.Features))
                {
                    allFeatures.AddRange(parentCategory.Features.Split(',').Select(f => f.Trim()));
                }
            }

            var products = await _productDatabaseController.GetProductsByCategoryId(id);
            var childCategories = categories.Where(c => c.ParentCategoryId == category.Id).ToList();

            ViewBag.Products = products;
            ViewBag.Category = category;
            ViewBag.ChildCategories = childCategories;
    
            // Pass unique features to SetFeatures
            category.Features = string.Join(",", allFeatures.Distinct());
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
            List<string> allFeatures = new List<string>();

            // Get features from the current category
            if (!string.IsNullOrEmpty(category.Features))
            {
                var categoryFeatures = category.Features.Split(',').Select(f => f.Trim()).ToArray();
                allFeatures.AddRange(categoryFeatures);

                // Initialize feature options dictionary
                foreach (var feature in allFeatures)
                {
                    featureOptions[feature] = new List<string>();
                }
            }

            // Process product features
            foreach (var product in products)
            {
                try
                {
                    var productFeatures = JsonSerializer.Deserialize<Dictionary<string, string>>(product.Features);
                    foreach (var feature in allFeatures)
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

            ViewBag.FeatureOptions = featureOptions;
            ViewBag.AllFeatures = allFeatures;
        }
        [HttpPost("/promocode")]
        public IActionResult ApplyPromo([FromBody] PromoRequestDto request)
        {
            var promos = new Dictionary<string, double>
            {
                { "KOMRAD10", 10 },  // %10 indirim
                { "VONNEX20", 20 },  // %20 indirim
                { "BEDAVA100", 100 } // %100 indirim 😎
            };

            var code = request.PromoCode?.Trim().ToUpper() ?? "";
            promos.TryGetValue(code, out double percent);

            return Ok(new { discountPercent = percent }); // 0 ise geçersiz
        }

        }

    }
    

