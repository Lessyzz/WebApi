using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Controllers;

public class ProductController(ProductDatabaseController _productDatabaseController, EfContext _context) : Microsoft.AspNetCore.Mvc.Controller
{

    [HttpGet]
    [Route("/User/GetProducts")]
    public async Task<IActionResult> GetProducts()
    {
        var registerResult = await _productDatabaseController.GetProducts();
        return new JsonResult(new { message = registerResult });
    }

    [HttpGet]
    [Route("/User/GetProductById")]
    public async Task<IActionResult> GetProductById(string productId)
    {
        var registerResult = await _productDatabaseController.GetProductById(productId);

        if (registerResult == null) return new JsonResult(new { message = "Error" });
        return new JsonResult(new { message = registerResult });
    }


    [HttpGet]
    [Route("/User/GetBasketProducts")]
    public async Task<IActionResult> GetBasketProducts(string userId)
    {
        var registerResult = await _productDatabaseController.GetBasketProducts(userId);

        if (registerResult == null) return new JsonResult(new { message = "Error" });
        return new JsonResult(new { message = registerResult });
    }
    
    [HttpPost]
    [Route("/User/AddProduct")]
    public async Task<IActionResult> AddProduct(AddProductDto addProductDto)
    {
        await _productDatabaseController.AddProduct(addProductDto, "asd");
        return new JsonResult(new { message = "Successful!" });
    }
    
    [HttpPost]
    [Route("/User/AddProductToBasketProduct")]
    public async Task<IActionResult> AddProductToBasketProduct(string productId, string userId)
    {
        await _productDatabaseController.AddProductToBasketProduct(productId, userId);
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