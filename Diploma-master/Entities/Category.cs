namespace Diploma.Entities;

public class Category
{
    public Category()
    {
        Products = new List<Product>();
    }
    public Guid Id { get; set; }
    public string CategoryName { get; set; }


    public ICollection<Product> Products { get; set; }
}
