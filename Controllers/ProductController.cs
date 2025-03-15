using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Controllers;

public class ProductController(UserDatabaseController _userDatabaseController, EfContext _context) : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpGet]
    [Route("/User/GetBasketProducts")]
    public IActionResult GetBasketProducts(string userId)
    {
        var registerResult = _userDatabaseController.GetBasketProducts(userId);

        if (registerResult == null) return new JsonResult(new { message = "Error" });
        return new JsonResult(new { message = registerResult });
    }
    
    [HttpPost]
    [Route("/User/AddProduct")]
    public IActionResult AddProduct(AddProductDto addProductDto)
    {
        var registerResult = _userDatabaseController.AddProduct(addProductDto, "asd");

        if (registerResult == null) return new JsonResult(new { message = "Error" });
        return new JsonResult(new { message = registerResult });
    }
    
    [HttpPost]
    [Route("/User/AddProductToBasketProduct")]
    public IActionResult AddProductToBasketProduct(string productId, string userId)
    {
        var registerResult = _userDatabaseController.AddProductToBasketProduct(productId, userId);

        if (registerResult == null) return new JsonResult(new { message = "Error" });
        return new JsonResult(new { message = registerResult });
    }
    
    // [HttpPost]
    // [Route("/User/UpdateProducts")]
    // public IActionResult UpdateProducts(string basketId)
    // {
    //     var registerResult = _userDatabaseController.UpdateProducts(basketId);
    //
    //     if (registerResult == null) return new JsonResult(new { message = "Error" });
    //     return new JsonResult(new { message = registerResult });
    // }
}