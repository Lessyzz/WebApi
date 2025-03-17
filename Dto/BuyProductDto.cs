using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class BuyProductDto
{
    public string ProductId { get; set; }
    public string UserId { get; set; }
    public int Quantity { get; set; }
}