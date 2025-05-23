namespace Diploma.Contracts;

public class SellerDetailsResponse
{
    public Guid Id { get; set; }
    public string StoreName { get; set; } = null!;
    public string StoreDescription { get; set; } = null!;
    public string ContactAddress { get; set; } = null!;
    public string ContactPhone { get; set; } = null!;
    public List<SellerProductResponse> Products { get; set; } = new();
}

public class SellerProductResponse
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ProductReviewResponse> Reviews { get; set; } = new();
}

public class ProductReviewResponse
{
    public int Rating { get; set; }
    public string Comment { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string UserName { get; set; } = null!;
}

