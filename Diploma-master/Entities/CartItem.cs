namespace Diploma.Entities;

public class CartItem
{
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }  
    public User User { get; set; }
    public Product Product { get; set; }
}
