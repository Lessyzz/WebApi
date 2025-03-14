using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebApi.Models;

public class Response : IActionResult
{
    public bool Success { get; set; }
    public object? Payload { get; set; }
    public string? Cause { get; set; }
    public int Code { get; set; } = 200;
    
    public Response(object? payload)
    {
        Success = true;
        Payload = payload;
    }

    public Response(string cause, int statusCode, object? payload = null)
    {
        Success = false;
        Cause = cause;
        Code = statusCode;
        Payload = payload;
    }
    
    public async Task ExecuteResultAsync(ActionContext context)
    {
        await ExecuteResultHttpAsync(context.HttpContext);
    }
    public async Task ExecuteResultHttpAsync(HttpContext context)
    {
        var json = JsonConvert.SerializeObject(this);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = Code;
        await context.Response.WriteAsync(json);
    }
}