namespace WebApi.Dto;

public class OrderDto
{
    public string PaymentId { get; set; }
    public string UserId { get; set; }
    public double TotalAmount { get; set; }
    public int ItemCount { get; set; }
    public DateTime OrderDate { get; set; }
}
