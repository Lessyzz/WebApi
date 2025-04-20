using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Models;

[Index(nameof(Email), IsUnique = true)]
[Index(nameof(Number), IsUnique = true)]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    
    public string Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Number { get; set; }
    public string? Address { get; set; } // For sellers
    public bool Verified { get; set; } // Verify sellers for security
    public string Roles { get; set; } // List. a,b,c
    
    public List<Product> Products { get; set; }
    
    public string? ResetCode { get; set; }
    
    public DateTime? ResetCodeExpires { get; set; }
}