namespace Diploma.Contracts;

public class RegisterUserRequest
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public int Age { get; set; }
    public string Sex { get; set; } = null!;
    public double WeightKg { get; set; }
    public double HeightCm { get; set; }

    public string Goal { get; set; } = null!;
    public decimal BudgetPerWeek { get; set; }
    public bool IsVegan { get; set; }

    public List<string>? Restrictions { get; set; }
}
