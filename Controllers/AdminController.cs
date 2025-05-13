using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Controllers;

[Route("Admin")]
public class AdminController(EfContext context) : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpGet("Dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        // Get counts for dashboard
        ViewBag.UserCount = await context.Users.CountAsync();
        ViewBag.ProductCount = await context.Products.CountAsync();
        ViewBag.CategoryCount = await context.Categories.CountAsync();
        ViewBag.OrderCount = await context.PaidProducts.Select(p => p.PaymentId).Distinct().CountAsync();
        
        // Recent orders
        ViewBag.RecentOrders = await context.PaidProducts
            .Include(p => p.Product)
            .OrderByDescending(p => p.Id)
            .Take(5)
            .ToListAsync();
        
        return View();
    }

    [HttpGet("Login")]
    public IActionResult Login()
    {
        return View();
    }
    

    #region Category Management
    [HttpGet("Categories")]
    public async Task<IActionResult> Categories()
    {
        var categories = await context.Categories.ToListAsync();
        return View(categories);
    }

    [HttpGet("Categories/Create")]
    public IActionResult CreateCategory()
    {
        ViewBag.ParentCategories = context.Categories.ToList();
        return View();
    }

    [HttpPost("Categories/Create")]
    public async Task<IActionResult> CreateCategory(Category category)
    {
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        ViewBag.ParentCategories = context.Categories.ToList();
        return View(category);
    }

    [HttpGet("Categories/Edit/{id}")]
    public async Task<IActionResult> EditCategory(int id)
    {
        var category = await context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        ViewBag.ParentCategories = context.Categories.Where(c => c.Id != id).ToList();
        return View(category);
    }

    [HttpPost("Categories/Edit/{id}")]
    public async Task<IActionResult> EditCategory(int id, Category category)
    {
        if (id != category.Id)
        {
            return NotFound();
        }

        context.Update(category);
        await context.SaveChangesAsync();
        ViewBag.ParentCategories = context.Categories.Where(c => c.Id != id).ToList();
        return View(category);
    }

    [HttpPost("Categories/Delete/{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await context.Categories.FindAsync(id);
        if (category != null)
        {
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Categories));
    }

    private bool CategoryExists(int id)
    {
        return context.Categories.Any(e => e.Id == id);
    }
    #endregion

    #region Product Management
    [HttpGet("Products")]
    public async Task<IActionResult> Products()
    {
        var products = await context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductSeller)
            .ToListAsync();
        return View(products);
    }

    [HttpGet("Products/Create")]
    public IActionResult CreateProduct()
    {
        ViewBag.Categories = context.Categories.ToList();
        ViewBag.Sellers = context.Users.Where(u => u.Roles.Contains("Seller")).ToList();
        return View();
    }

    [HttpPost("Products/Create")]
    public async Task<IActionResult> CreateProduct(Product product)
    {
        product.Id = Guid.NewGuid().ToString();
        context.Products.Add(product);
        await context.SaveChangesAsync();
        ViewBag.Categories = context.Categories.ToList();
        ViewBag.Sellers = context.Users.Where(u => u.Roles.Contains("Seller")).ToList();
        // return products edit pag
        return View(product);
    }

    [HttpGet("Products/Edit/{id}")]
    public async Task<IActionResult> EditProduct(string id)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        ViewBag.ProductId = id;
        ViewBag.Categories = context.Categories.ToList();
        ViewBag.Sellers = context.Users.Where(u => u.Roles.Contains("Seller")).ToList();
        ViewBag.Name = product.Name;
        ViewBag.Description = product.Description;
        ViewBag.Price = product.Price;
        ViewBag.Images = product.Images;
        
        return View(product);
    }

    [HttpPost("Products/Edit/{id}")]
    public async Task<IActionResult> EditProduct([FromForm] Product product)
    {
        Product mainProduct = await context.Products.FindAsync(product.Id);

        if (product.Name != mainProduct.Name) mainProduct.Name = product.Name;
        if (product.Description != mainProduct.Description) mainProduct.Description = product.Description;
        if (product.Price != mainProduct.Price) mainProduct.Price = product.Price;
        if (product.Images != mainProduct.Images) mainProduct.Images = product.Images;
        if (product.CategoryId != mainProduct.CategoryId) mainProduct.CategoryId = product.CategoryId;

        await context.SaveChangesAsync();
        
        ViewBag.ProductId = mainProduct.Id;
        ViewBag.Name = mainProduct.Name;
        ViewBag.Description = mainProduct.Description;
        ViewBag.Price = mainProduct.Price;
        ViewBag.Images = mainProduct.Images;
        ViewBag.Categories = context.Categories.ToList();
        ViewBag.Sellers = context.Users.Where(u => u.Roles.Contains("Seller")).ToList();
        return View(product);
    }

    [HttpPost("Products/Delete/{id}")]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        var product = await context.Products.FindAsync(id);
        if (product != null)
        {
            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Products));
    }

    private bool ProductExists(string id)
    {
        return context.Products.Any(e => e.Id == id);
    }
    #endregion

    #region User Management
    [HttpGet("Users")]
    public async Task<IActionResult> Users()
    {
        var users = await context.Users.ToListAsync();
        return View(users);
    }

    [HttpGet("Users/Create")]
    public IActionResult CreateUser()
    {
        return View();
    }

    [HttpGet("Users/Edit/{id}")]
    public async Task<IActionResult> EditUser(string id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        ViewBag.CurrentRoles = user.Roles?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
        return View(user);
    }

    [HttpPost("Users/Delete/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await context.Users.FindAsync(id);
        if (user != null)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Users));
    }

    private bool UserExists(string id)
    {
        return context.Users.Any(e => e.Id == id);
    }
    #endregion

    #region Order Management
    [HttpGet("Orders")]
    public async Task<IActionResult> Orders()
    {
        var orders = await context.PaidProducts
            .Include(p => p.Product)
            .GroupBy(p => p.PaymentId)
            .Select(g => new OrderDto
            {
                PaymentId = g.Key,
                UserId = g.First().UserId,
                TotalAmount = g.Sum(p => p.TotalPrice),
                ItemCount = g.Count(),
                OrderDate = DateTime.Now // This should ideally come from a payment date field
            })
            .ToListAsync();

        return View(orders);
    }

    [HttpGet("Orders/Details/{paymentId}")]
    public async Task<IActionResult> OrderDetails(string paymentId)
    {
        var orderItems = await context.PaidProducts
            .Include(p => p.Product)
            .Where(p => p.PaymentId == paymentId)
            .ToListAsync();

        if (orderItems == null || !orderItems.Any())
        {
            return NotFound();
        }

        var user = await context.Users.FindAsync(orderItems.First().UserId);
        
        ViewBag.User = user;
        ViewBag.PaymentId = paymentId;
        ViewBag.TotalAmount = orderItems.Sum(p => p.TotalPrice);

        return View(orderItems);
    }
    #endregion
}

