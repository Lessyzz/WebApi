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
    [Route("/Product/GetProductsAsSeller")]
    public async Task<IActionResult> GetProductsAsSeller(string sellerId)
    {
        var registerResult = await _productDatabaseController.GetProductsAsSeller(sellerId);
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
    public async Task<IActionResult> AddProduct([FromForm] AddProductDto addProductDto, List<IFormFile> ImageFiles)
    {
        addProductDto.ProductSellerId = "c2ae16c5-4d71-4f38-94c6-107bd4fa47ae";
        
        var imageUrls = new List<string>();
        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");

        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        foreach (var file in ImageFiles)
        {
            if (file.Length <= 0) continue;
            
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = $"/images/products/{uniqueFileName}";
            imageUrls.Add(imageUrl);
        }
        
        addProductDto.Images = string.Join(',', imageUrls);

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
    
    [HttpPost]
    [Route("/Product/RemoveProduct")]
    public async Task<IActionResult> RemoveProduct(string productId)
    {
        await _productDatabaseController.RemoveProduct(productId);
        return new JsonResult(new { message = "Succesful!" });
    }

    [HttpPost]
    [Route("/User/UpdateProducts")]
    public async Task<IActionResult> UpdateProducts(UpdateProductDto updateProductDto)
    {
        await _productDatabaseController.UpdateProduct(updateProductDto);
        return new JsonResult(new { message = "Succesful!" });
    }
}