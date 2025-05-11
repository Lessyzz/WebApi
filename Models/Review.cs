using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class Review
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string ProductId { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; }

    [Required]
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    [StringLength(1000)]
    public string Comment { get; set; }

    [Display(Name = "Review Date")]
    public DateTime ReviewDate { get; set; } = DateTime.Now;

    public bool IsApproved { get; set; } = false;
}