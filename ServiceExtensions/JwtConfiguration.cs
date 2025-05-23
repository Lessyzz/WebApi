namespace WebApi.Models;

public class JwtConfiguration
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; } 
    public required string Key { get; set; } 
}