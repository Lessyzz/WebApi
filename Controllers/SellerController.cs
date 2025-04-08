using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebApi.Data;
using WebApi.DatabaseController;
using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Controllers;

public class SellerController(
    ProductDatabaseController productDatabaseController,
    SellerDatabaseController sellerDatabaseController,
    CategoryDatabaseController categoryDatabaseController
    ) : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpGet]
    [Route("/Seller")]
    [Authorize]
    public async Task<IActionResult> Index()
    {
        ViewBag.Products = await productDatabaseController.GetProductsAsSeller(User.Claims.First(c => c.Type == "sid").Value);
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
        var registerResult = await productDatabaseController.GetProductsAsSeller("9e2157e4-0fce-42d3-ab91-66ed36017b12");
        return View(registerResult);
    }
    
    [HttpGet("/Seller/AddProduct")]
    public async Task<IActionResult> AddProduct()
    {
        var categories = await categoryDatabaseController.GetCategories();
        var flatCategories = FlattenCategories(categories);
        ViewBag.Categories = flatCategories;
        return View();
    }
    
    [HttpGet("/Seller/EditProduct/{id}")]
    [Authorize]
    public async Task<IActionResult> EditProduct(string id)
    {
        var product = await productDatabaseController.GetProductById(id);
        ViewBag.Product = product;
        
        var categories = await categoryDatabaseController.GetCategories();
        var flatCategories = FlattenCategories(categories);
        ViewBag.Categories = flatCategories;
        
        return View();
    }

    private List<(int Id, string Name)> FlattenCategories(List<Category> categories, int? parentId = null, int level = 0)
    {
        var result = new List<(int, string)>();

        var children = categories.Where(c => c.ParentCategoryId == parentId).ToList();

        foreach (var child in children)
        {
            var indent = new string('-', level * 2);
            result.Add((child.Id, $"{indent} {child.Name}"));

            result.AddRange(FlattenCategories(categories, child.Id, level + 1));
        }

        return result;
    }
    
}