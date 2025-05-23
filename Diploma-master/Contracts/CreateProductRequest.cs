namespace Diploma.Contracts;

public class CreateProductRequest
{
    public string ProductName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public double Calories { get; set; }
    public double Protein { get; set; }
    public double Fat { get; set; }
    public double Carbs { get; set; }
    public bool IsVegan { get; set; }
    public Guid CategoryId { get; set; }
    public List<string> Restrictions { get; set; } = new();
}

public class UpdateProductRequest : CreateProductRequest
{
    public Guid ProductId { get; set; }
}
