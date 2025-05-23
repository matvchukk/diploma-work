namespace Diploma.Entities;

public class Product
{
    public Product()
    {
        Reviews = new List<Review>();
        ShoppingCartItems = new List<CartItem>();
        OrderedItems = new List<OrderItem>();
        WishedInLists = new List<WishedItem>();
        Restrictions = new List<string>();
    }

    public Product(string productName, string description, decimal price, Guid categoryId, Guid sellerId,
                   double calories, double protein, double fat, double carbs, List<string>? restrictions, bool isVegan)
    {
        Id = Guid.NewGuid();
        ProductName = productName;
        Description = description;
        Price = price;
        CreatedAt = DateTime.UtcNow;

        CategoryId = categoryId;
        SellerId = sellerId;

        Calories = calories;
        Protein = protein;
        Fat = fat;
        Carbs = carbs;
        IsVegan = isVegan;
        Restrictions = restrictions ?? new List<string>();

        Reviews = new List<Review>();
        ShoppingCartItems = new List<CartItem>();
        OrderedItems = new List<OrderItem>();
        WishedInLists = new List<WishedItem>();
    }

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
    public Guid SellerId { get; set; }

    public Category Category { get; set; }
    public Seller Seller { get; set; }

    public ICollection<Review> Reviews { get; set; }
    public ICollection<CartItem> ShoppingCartItems { get; set; }
    public ICollection<WishedItem> WishedInLists { get; set; }
    public ICollection<OrderItem> OrderedItems { get; set; }
}