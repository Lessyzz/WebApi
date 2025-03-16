using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Models;

public class UpdateCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ParentCategoryId { get; set; }
}