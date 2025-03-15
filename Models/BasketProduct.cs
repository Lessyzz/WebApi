using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class BasketProduct
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    public string ProductId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
}