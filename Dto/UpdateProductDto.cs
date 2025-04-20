namespace WebApi.Dto;

public class UpdateProductDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public int CategoryId { get; set; }
    public Dictionary<string, string> FeatureValues { get; set; } = new Dictionary<string, string>();
}