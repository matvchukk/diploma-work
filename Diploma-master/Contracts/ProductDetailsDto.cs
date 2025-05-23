namespace Diploma.Contracts;

public class ProductDetailsDto
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }

    public double Calories { get; set; }
    public double Protein { get; set; }
    public double Fat { get; set; }
    public double Carbs { get; set; }
    public bool IsVegan { get; set; }  

    public List<string> Restrictions { get; set; } = new();
    public DateTime CreatedAt { get; set; }

    public Guid SellerId { get; set; }
    public string SellerName { get; set; } = null!;
    public string SellerContactPhone { get; set; } = null!;
    public string CategoryName { get; set; } = null!;

    public double AverageRating { get; set; }
    public int ReviewsCount { get; set; }
    public List<ProductReviewDto> Reviews { get; set; } = new();
}

public class ProductReviewDto
{
    public int Rating { get; set; }
    public string Comment { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string UserName { get; set; } = null!;
}
