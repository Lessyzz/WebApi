using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebApi.Data;
using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Controllers;

public class SellerController(ProductDatabaseController productDatabaseController, SellerDatabaseController sellerDatabaseController) : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpGet]
    [Route("/Seller")]
    public async Task<IActionResult> Index()
    {
        return View();
    }

    [HttpGet]
    [Route("/Seller/Register")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpGet]
    [Route("/Seller/Login")]
    public IActionResult Login()
    {
        return View();
    }


    [HttpPost]
    [Route("/Seller/Register")]
    public async Task<IActionResult> RegisterPOST([FromForm]RegisterDto registerDto)
    {
        var registerResult = await sellerDatabaseController.Register(registerDto);

        // Failed
        if (registerResult.Code == 400) return new JsonResult(new { message = registerResult });

        // Success
        return Redirect("/");
    }

    
    [HttpPost]
    [Route("/Seller/Login")]
    public async Task<IActionResult> LoginPOST([FromForm]LoginDto loginDto)
    {
        var User = await sellerDatabaseController.Login(loginDto);

        if (User == null) return new JsonResult(new { message = "Hatali Giris!" });

        // return new Response(User);
        return Redirect("/");
    }

    [HttpGet]
    [Route("/Seller/Products")]
    public async Task<IActionResult> Products()
    {
        var registerResult = await productDatabaseController.GetProductsAsSeller("87b090c9-ca9e-4849-a7d7-27ba88b30785");
        return View(registerResult);
    }
}