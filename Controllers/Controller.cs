using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class Controller(UserDatabaseController _userDatabaseController, EfContext _context) : Microsoft.AspNetCore.Mvc.Controller
    {
        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        [Route("/product")]
        public IActionResult Product()
        {
            return View();
        }
    }
}