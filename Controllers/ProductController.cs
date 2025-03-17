using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Controllers;

public class ProductController(ProductDatabaseController _productDatabaseController, EfContext _context) : Microsoft.AspNetCore.Mvc.Controller
{

    [HttpGet]
    [Route("/Product/GetProducts")]
    public async Task<IActionResult> GetProducts()
    {
        var registerResult = await _productDatabaseController.GetProducts();
        return new JsonResult(new { message = registerResult });
    }

    [HttpGet]
    [Route("/Product/GetProductById")]
    public async Task<IActionResult> GetProductById(string productId)
    {
        var registerResult = await _productDatabaseController.GetProductById(productId);

        if (registerResult == null) return new JsonResult(new { message = "Error" });
        return new JsonResult(new { message = registerResult });
    }


    [HttpGet]
    [Route("/Product/GetBasketProducts")]
    public async Task<IActionResult> GetBasketProducts(string userId)
    {
        var registerResult = await _productDatabaseController.GetBasketProducts(userId);

        if (registerResult == null) return new JsonResult(new { message = "Error" });
        return new JsonResult(new { message = registerResult });
    }
    
    [HttpGet]
    [Route("/Product/GetPaidProducts")]
    public async Task<IActionResult> GetPaidProducts(string userId)
    {
        var registerResult = await _productDatabaseController.GetPaidProducts(userId);

        if (registerResult == null) return new JsonResult(new { message = "Error" });
        return new JsonResult(new { message = registerResult });
    }

    [HttpGet]
    [Route("/Product/GetPaidProductsAsSeller")]
    public async Task<IActionResult> GetPaidProductsAsSeller(string sellerId)
    {
        var registerResult = await _productDatabaseController.GetPaidProductsAsSeller(sellerId);

        if (registerResult == null) return new JsonResult(new { message = "Error" });
        return new JsonResult(new { message = registerResult });
    }

    [HttpPost]
    [Route("/Product/AddProduct")]
    public async Task<IActionResult> AddProduct(AddProductDto addProductDto)
    {
        await _productDatabaseController.AddProduct(addProductDto);
        return new JsonResult(new { message = "Successful!" });
    }
    

    // Sepete ürün eklemeye basıldığında eğer hiç ürün yoksa 1 tane ekler. Eğer varsa sayısını 1 arttırır.
    [HttpPost]
    [Route("/Product/AddProductToBasketProduct")]
    public async Task<IActionResult> AddProductToBasketProduct(string productId, string userId)
    {
        await _productDatabaseController.AddProductToBasketProduct(productId, userId);
        return new JsonResult(new { message = "Succesful!" });
    }
    
    // Sepetteki ürünün sayısını 1 azaltır.
    [HttpPost]
    [Route("/Product/RemoveOneProductFromBasketProduct")]
    public async Task<IActionResult> RemoveOneProductFromBasketProduct(string productId, string userId)
    {
        await _productDatabaseController.RemoveOneProductFromBasketProduct(productId, userId);
        return new JsonResult(new { message = "Succesful!" });
    }

    // Sepetteki ürünü tamamen siler.
    [HttpPost]
    [Route("/Product/RemoveProductFromBasketProduct")]
    public async Task<IActionResult> RemoveProductFromBasketProduct(string productId, string userId)
    {
        await _productDatabaseController.RemoveProductFromBasketProduct(productId, userId);
        return new JsonResult(new { message = "Succesful!" });
    }

    // Sepetteki tamamen temizler.
    [HttpPost]
    [Route("/Product/RemoveAllProductsFromBasketProduct")]
    public async Task<IActionResult> RemoveAllProductsFromBasketProduct(string userId)
    {
        await _productDatabaseController.RemoveAllProductsFromBasketProduct(userId);
        return new JsonResult(new { message = "Succesful!" });
    }


    [HttpPost]
    [Route("/Product/BuyProduct")]
    public async Task<IActionResult> BuyProduct(BuyProductDto buyProductDto)
    {
        await _productDatabaseController.BuyProduct(buyProductDto);
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