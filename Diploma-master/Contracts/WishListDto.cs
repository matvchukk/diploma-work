namespace Diploma.Contracts;

public class WishListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public List<WishListProductDto> Products { get; set; } = new();
}

public class WishListProductDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
}
