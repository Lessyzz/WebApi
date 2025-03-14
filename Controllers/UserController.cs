using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class UserController(UserDatabaseController _userDatabaseController) : Controller
    {
        private readonly EfContext _context;
        
        [HttpPost]
        [Route("/User/Register")]
        public IActionResult RegisterGET(RegisterDto registerDto)
        {
            _userDatabaseController.Register(registerDto);
            return new JsonResult(new { message = "Register successful." });
        }
        
        [HttpPost]
        [Route("/User/Login")]
        public IActionResult LoginPOST(LoginDto loginDto)
        {
            var User = _userDatabaseController.Login(loginDto);
            return new Response(User);
        }
    }
}