using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [Authorize]
    public async Task<IActionResult> GetBasketProducts()
    {
        var registerResult = await _productDatabaseController.GetBasketProducts(User.FindFirstValue("sid")!);
        
        return new JsonResult(registerResult);
    }
    
    [HttpGet]
    [Route("/Product/GetPaidProducts")]
    [Authorize]
    public async Task<IActionResult> GetPaidProducts()
    {
        var registerResult = await _productDatabaseController.GetPaidProducts(User.FindFirstValue("sid")!);

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
    [Authorize]
    public async Task<IActionResult> AddProduct([FromForm] AddProductDto addProductDto, List<IFormFile> ImageFiles)
    {
        addProductDto.ProductSellerId = User.Claims.First(c => c.Type == "sid").Value;
        
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


    // Tek seferde istediğin kadar sepete eklemeye yarıyor
    [HttpPost]
    [Route("/Product/AddProductToBasketProductWithQuantity")]
    [Authorize]
    public async Task<IActionResult> AddProductToBasketProductWithQuantity(string productId, int quantity)
    {
        await _productDatabaseController.AddProductToBasketProductWithQuantity(productId, User.FindFirstValue("sid")!, quantity);
        return new JsonResult(new { message = "Succesful!" });
    }
    

    // Sepete ürün eklemeye basıldığında eğer hiç ürün yoksa 1 tane ekler. Eğer varsa sayısını 1 arttırır.
    [HttpPost]
    [Route("/Product/AddProductToBasketProduct")]
    [Authorize]
    public async Task<IActionResult> AddProductToBasketProduct(string productId)
    {
        await _productDatabaseController.AddProductToBasketProduct(productId, User.FindFirstValue("sid")!);
        return new JsonResult(new { message = "Succesful!" });
    }
    
    // Sepetteki ürünün sayısını 1 azaltır.
    [HttpPost]
    [Route("/Product/RemoveOneProductFromBasketProduct")]
    [Authorize]
    public async Task<IActionResult> RemoveOneProductFromBasketProduct(string productId)
    {
        await _productDatabaseController.RemoveOneProductFromBasketProduct(productId, User.FindFirstValue("sid")!);
        return new JsonResult(new { message = "Succesful!" });
    }

    // Sepetteki ürünü tamamen siler.
    [HttpPost]
    [Route("/Product/RemoveProductFromBasketProduct")]
    [Authorize]
    public async Task<IActionResult> RemoveProductFromBasketProduct(string productId)
    {
        await _productDatabaseController.RemoveProductFromBasketProduct(productId, User.FindFirstValue("sid")!);
        return new JsonResult(new { message = "Succesful!" });
    }

    // Sepetteki tamamen temizler.
    [HttpPost]
    [Route("/Product/RemoveAllProductsFromBasketProduct")]
    [Authorize]
    public async Task<IActionResult> RemoveAllProductsFromBasketProduct()
    {
        await _productDatabaseController.RemoveAllProductsFromBasketProduct(User.FindFirstValue("sid")!);
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
    [Route("/Product/BuyProductAsList")]
    public async Task<IActionResult> BuyProductAsList(List<BuyProductDto> buyProductDtos)
    {
        foreach (var buyProductDto in buyProductDtos)
        {
            await _productDatabaseController.BuyProduct(buyProductDto);
        }
        return new JsonResult(new { message = "Succesful!" });
    }
    
    [HttpPost]
    [Route("/Product/RemoveProduct")]
    public async Task<IActionResult> RemoveProduct(string productId)
    {
        await _productDatabaseController.RemoveProduct(productId);
        return new JsonResult(new { message = "Succesful!" });
    }
    
    [HttpPost("/Seller/EditProduct/{id}")]
    [Authorize]
    public async Task<IActionResult> EditProduct(string id, [FromForm] Product product, List<IFormFile>? NewImageFiles)
    {
        var sellerId = User.FindFirstValue("sid")!;

        var originalProduct = await _productDatabaseController.GetProductById(id);
        product.ProductSellerId = sellerId;

        var newImageUrls = new List<string>();
        if (NewImageFiles is { Count: > 0 })
        {
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");

            foreach (var file in NewImageFiles)
            {
                if (file.Length <= 0) continue;

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadPath, uniqueFileName);

                try
                {
                    await using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    newImageUrls.Add($"/images/products/{uniqueFileName}");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("NewImageFiles", $"Error uploading file {file.FileName}: {ex.Message}");
                }
            }

            if (newImageUrls.Count != 0)
            {
                product.Images = string.Join(',', newImageUrls);
            }
        }
        else
        {
             product.Images = originalProduct.Images;
        }
        
        try
        {
            await _productDatabaseController.UpdateProduct(product);

            TempData["SuccessMessage"] = "Ürün başarıyla güncellendi!";
            return RedirectToAction("Index", "Seller");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Ürün güncellenirken bir hata oluştu: {ex.Message}");
        }
        
        return Redirect("/Seller");
    }

    [HttpPost]
    [Route("/Product/AddPromotionCode")]
    public async Task<IActionResult> AddPromotionCode(string code, double discount)
    {
        await _productDatabaseController.AddPromotionCode(code, discount);
        return new JsonResult(new { message = "Succesful!" });
    }

    [HttpPost]
    [Route("/Product/RemovePromotionCode")]
    public async Task<IActionResult> RemovePromotionCode(string code)
    {
        await _productDatabaseController.RemovePromotionCode(code);
        return new JsonResult(new { message = "Succesful!" });
    }

    [HttpPost]
    [Route("/Product/CheckPromotionCode")]
    public async Task<IActionResult> CheckPromotionCode(string code)
    {
        var registerResult = await _productDatabaseController.CheckPromotionCode(code);
        return new JsonResult(new { message = registerResult });
    }
}
