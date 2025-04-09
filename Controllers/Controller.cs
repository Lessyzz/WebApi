using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.DatabaseController;

namespace WebApi.Controllers
{
    public class Controller(UserDatabaseController _userDatabaseController, ProductDatabaseController _productDatabaseController, EfContext _context) : Microsoft.AspNetCore.Mvc.Controller
    {
        [HttpGet]
        [Route("/")]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            ViewBag.Products = await _productDatabaseController.GetProducts();
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
        public IActionResult Checkout()
        {
            return View();
        }
        
        [HttpGet("/Success")]
        public IActionResult Success()
        {
            return View();
        }
    }
}