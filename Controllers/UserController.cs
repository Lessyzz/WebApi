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
        public IActionResult RegisterPOST(RegisterDto registerDto)
        {
            var registerResult = _userDatabaseController.Register(registerDto);

            // Failed
            if (registerResult.Result.Code == 400) return new JsonResult(new { message = registerResult }); 

            // Success
            return new JsonResult(new { message = "User Registered Successfuly!" });
        }
        
        [HttpPost]
        [Route("/User/Login")]
        public IActionResult LoginPOST(LoginDto loginDto)
        {
            var User = _userDatabaseController.Login(loginDto);

            // Failed
            if (User == null) return new JsonResult(new { message = "Invalid Username or Password!" });

            // Success
            return new Response(User);
        }
    }
}