using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Controllers;

public class ProductController(UserDatabaseController _userDatabaseController, EfContext _context) : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpGet]
    [Route("/User/GetBasketProducts")]
    public async Task<IActionResult> GetBasketProducts(string userId)
    {
        var registerResult = await _userDatabaseController.GetBasketProducts(userId);

        if (registerResult == null) return new JsonResult(new { message = "Error" });
        return new JsonResult(new { message = registerResult });
    }
    
    [HttpPost]
    [Route("/User/AddProduct")]
    public async Task<IActionResult> AddProduct(AddProductDto addProductDto)
    {
        await _userDatabaseController.AddProduct(addProductDto, "asd");
        return new JsonResult(new { message = "Successful!" });
    }
    
    [HttpPost]
    [Route("/User/AddProductToBasketProduct")]
    public async Task<IActionResult> AddProductToBasketProduct(string productId, string userId)
    {
        await _userDatabaseController.AddProductToBasketProduct(productId, userId);
        return new JsonResult(new { message = "Succesful!" });
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