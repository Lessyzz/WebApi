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
    public List<Category> Category { get; set; }
    public float Discount { get; set; }
}