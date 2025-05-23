namespace Diploma.Entities;

public class WishList
{
    public WishList()
    {
        WishedProducts = new List<WishedItem>();
    }

    public WishList(Guid userId, string name)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Name = name;
        WishedProducts = new List<WishedItem>();
    }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public ICollection<WishedItem> WishedProducts { get; set; }
}
