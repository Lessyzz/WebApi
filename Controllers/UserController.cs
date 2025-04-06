using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.DatabaseController;
using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class UserController(UserDatabaseController _userDatabaseController, EfContext _context) : Microsoft.AspNetCore.Mvc.Controller
    {
        [HttpGet]
        [Route("/User/Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        [Route("/User/Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        [Route("/User/Logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwtToken");

            return Redirect("/");
        }


        [HttpPost]
        [Route("/User/Register")]
        public async Task<IActionResult> RegisterPOST([FromForm]RegisterDto registerDto)
        {
            var token = await _userDatabaseController.Register(registerDto);

            Response.Cookies.Append("jwtToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddYears(1453)
            });

            // Success
            return Redirect("/");
        }

        
        [HttpPost]
        [Route("/User/Login")]
        public async Task<IActionResult> LoginPOST([FromForm]LoginDto loginDto)
        {
            var token = await _userDatabaseController.Login(loginDto);

            Response.Cookies.Append("jwtToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddYears(1453)
            });
            
            return Redirect("/");
        }
    }
}