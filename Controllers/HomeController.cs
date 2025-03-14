using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class HomeController(UserDatabaseController _userDatabaseController, EfContext _context) : Controller
    {
        [HttpGet]
        [Route("/Home/Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}