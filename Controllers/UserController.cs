using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class UserController(UserDatabaseController _userDatabaseController, EfContext _context) : Controller
    {
        [HttpPost]
        [Route("/User/Register")]
        public IActionResult RegisterPOST(RegisterDto registerDto)
        {
            var registerResult = _userDatabaseController.Register(registerDto);
            return new JsonResult(new { message = registerResult });
        }
        
        [HttpPost]
        [Route("/User/Login")]
        public IActionResult LoginPOST(LoginDto loginDto)
        {
            var User = _userDatabaseController.Login(loginDto);
            if (User == null) return new Response("Invalid username or password.", 400);
            return new Response(User);
        }
    }
}