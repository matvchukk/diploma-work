using Diploma.Extensions;

namespace Diploma.Contracts;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int Age { get; set; }
    public string Role { get; set; }
    public string Sex { get; set; } = null!;
    public double WeightKg { get; set; }
    public double HeightCm { get; set; }
    public string Goal { get; set; } = null!;
    public decimal BudgetPerWeek { get; set; }
    public bool IsVegan { get; set; }
    public List<string> Restrictions { get; set; } = new();
    public DateTime RegisteredAt { get; set; }
    public List<WishListDto> WishLists { get; set; } = new();
    public List<OrderDto> Orders { get; set; } = new();
    public List<ReviewDto> Reviews { get; set; } = new();
}

public class SellerShortDto
{
    public Guid Id { get; set; }
    public string StoreName { get; set; }
}