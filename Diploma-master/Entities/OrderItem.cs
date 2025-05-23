namespace Diploma.Entities;

public class OrderItem
{
    public Guid ProductId { get; set; }
    public Guid OrderId { get; set; }
    public int Quantity { get; set; }


    public Order Order { get; set; }
    public Product Product { get; set; }
}
