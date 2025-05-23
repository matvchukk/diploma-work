namespace Diploma.Entities;

public class Seller
{
    public Seller()
    {
        Products = new List<Product>();
    }

    public Seller(Guid userId, string storeName, string description, string address, string phone)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        StoreName = storeName;
        StoreDescription = description;
        ContactAddress = address;
        ContactPhone = phone;
        Products = new List<Product>();
    }

    public Guid Id { get; set; }
    public string StoreName { get; set; }
    public string StoreDescription { get; set; }
    public string ContactAddress { get; set; }
    public string ContactPhone { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    public ICollection<Product> Products { get; set; }
}
