using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.Dto;

namespace WebApi.Controllers
{
    public class UserController(UserDatabaseController _userDatabaseController) : Controller
    {
        private readonly EfContext _context;

        [HttpGet]
        [Route("/User/Index")]
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        [Route("/User/Register")]
        public IActionResult Register(RegisterDto registerDto)
        {
            _userDatabaseController.Register(registerDto);
            return new JsonResult(new { message = "Register successful." });
        }
    }
}