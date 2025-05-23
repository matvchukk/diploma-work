namespace Diploma.Contracts;

public class GetProductsRequest
{
    public Guid? CategoryId { get; set; }
    public string? Sort { get; set; }
}
