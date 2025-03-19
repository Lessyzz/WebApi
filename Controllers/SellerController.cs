using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebApi.Data;
using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Controllers;

public class SellerController : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpGet]
    [Route("/Seller")]
    public async Task<IActionResult> Index()
    {
        return View();
    }
}