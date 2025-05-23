namespace Diploma.Contracts;

public class SellerProductOrderResponse
{
    public Guid OrderId { get; set; }
    public string BuyerEmail { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string ShippingAddress { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public List<SellerOrderedItemResponse> Items { get; set; } = new();
}

public class SellerOrderedItemResponse
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}