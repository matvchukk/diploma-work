using Diploma.Extensions;

namespace Diploma.Entities;

public class Order
{
    public Order()
    {
        OrderItems = new List<OrderItem>();
    }

    public Order(Guid userId, string shippingAddress)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;
        TotalPrice = 0;
        OrderItems = new List<OrderItem>();
    }
    public Guid Id { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public string ShippingAddress { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }


    public ICollection<OrderItem> OrderItems { get; set; }
}
