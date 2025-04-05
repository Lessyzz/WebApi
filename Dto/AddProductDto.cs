namespace WebApi.Dto;

public class AddProductDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public string Images { get; set; }
    public int CategoryId { get; set; }
    public float Discount { get; set; }
    public int Quantity { get; set; }

    public string ProductSellerId { get; set; }
}