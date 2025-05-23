namespace Diploma.Contracts;

public class UpdateCartQuantityRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; } 
}

