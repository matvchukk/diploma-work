namespace Diploma.Contracts;

public class RegisterSellerRequest
{
    public string StoreName { get; set; } = null!;
    public string StoreDescription { get; set; } = null!;
    public string ContactAddress { get; set; } = null!;
    public string ContactPhone { get; set; } = null!;
}

