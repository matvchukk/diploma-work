namespace Diploma.Contracts;

public class UpdateOrderStatusRequest
{
    public Guid OrderId { get; set; }
    public string Status { get; set; } = null!;
}