namespace Diploma.Contracts;

public class ProductDto
{
    public Guid Id { get; set; }

    public string ProductName { get; set; }
    public string Description { get; set; }

    public decimal Price { get; set; }

    public double Calories { get; set; }
    public double Protein { get; set; }
    public double Fat { get; set; }
    public double Carbs { get; set; }
    public bool IsVegan { get; set; }

    public List<string> Restrictions { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }

    public Guid SellerId { get; set; }
    public string SellerName { get; set; }

    public double AverageRating { get; set; }
}
