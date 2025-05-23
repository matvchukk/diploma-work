namespace Diploma.Entities;

public class User
{
    public User()
    {
        Orders = new List<Order>();
        Reviews = new List<Review>();
        ShoppingCart = new List<CartItem>();
        WishLists = new List<WishList>();
        Restrictions = new List<string>();
    }

    public User(string name, string email, string hashedPassword, int age, string sex, double weightKg, double heightCm, string goal, decimal budgetPerWeek, bool isVegan)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        HashedPassword = hashedPassword;
        RegisteredAt = DateTime.UtcNow;
        Age = age;
        Sex = sex;
        WeightKg = weightKg;
        HeightCm = heightCm;
        Goal = goal;
        BudgetPerWeek = budgetPerWeek;
        Restrictions = new List<string>();
        IsVegan = isVegan;

        Orders = new List<Order>();
        Reviews = new List<Review>();
        ShoppingCart = new List<CartItem>();
        WishLists = new List<WishList>();
        
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string HashedPassword { get; set; }
    public int Age { get; set; }

    public string Sex { get; set; }
    public double WeightKg { get; set; }
    public double HeightCm { get; set; }
    public string Goal { get; set; }
    public decimal BudgetPerWeek { get; set; }

    public bool IsVegan { get; set; }
    
    public List<string> Restrictions { get; set; }

    public DateTime RegisteredAt { get; set; }

    public Seller? Seller { get; set; }
    public ICollection<Order> Orders { get; set; }
    public ICollection<Review> Reviews { get; set; }
    public ICollection<CartItem> ShoppingCart { get; set; }
    public ICollection<WishList> WishLists { get; set; }
}
