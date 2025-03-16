using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
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


        [HttpPost]
        [Route("/User/Register")]
        public async Task<IActionResult> RegisterPOST([FromForm]RegisterDto registerDto)
        {
            var registerResult = await _userDatabaseController.Register(registerDto);

            // Failed
            if (registerResult.Code == 400) return new JsonResult(new { message = registerResult });

            // Success
            return new JsonResult(new { message = "User Registered Successfully!" });
        }

        
        [HttpPost]
        [Route("/User/Login")]
        public async Task<IActionResult> LoginPOST([FromForm]LoginDto loginDto)
        {
            var User = await _userDatabaseController.Login(loginDto);

            if (User == null) return new JsonResult(new { message = "Hatalı Giriş" });

            // return new Response(User);
            return new JsonResult(new { message = "Başarılı Giriş!" });
        }
    }
}