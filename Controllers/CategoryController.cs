using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Controllers;

public class CategoryController(CategoryDatabaseController _categoryDatabaseController, EfContext _context) : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpGet]
    [Route("/Category/GetCategories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _categoryDatabaseController.GetCategories();
        var categoryList = categories.Select(c => new { id = c.Id, name = c.Name }).ToList();
        return Json(categoryList);
    }
    

    [HttpPost]
    [Route("/Category/AddCategory")]
    public async Task<IActionResult> AddCategory([FromForm]AddCategoryDto addCategoryDto)
    {
        await _categoryDatabaseController.AddCategory(addCategoryDto);
        return new JsonResult(new { message = "Successful!" });
    }

    [HttpPost]
    [Route("/Category/UpdateCategory")]
    public async Task<IActionResult> UpdateCategory([FromForm]UpdateCategoryDto updateCategoryDto)
    {
        await _categoryDatabaseController.UpdateCategory(updateCategoryDto);
        return new JsonResult(new { message = "Successful!" });
    }
}