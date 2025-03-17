using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public List<string> Image { get; set; }
    public List<int> Categories { get; set; }
    public float Discount { get; set; }
    public int Quantity { get; set; }
    
    public string ProductSellerId { get; set; }
    public User ProductSeller { get; set; }
}