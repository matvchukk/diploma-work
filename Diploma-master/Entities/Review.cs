namespace Diploma.Entities;

public class Review
{
    public Review()
    {
        CreatedDate = DateTime.UtcNow;
    }

    public Review(Guid userId, Guid productId, int rating, string comment)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        ProductId = productId;
        Rating = rating;
        Comment = comment;
        CreatedDate = DateTime.UtcNow;
    }
    public Guid Id { get; set; } 
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedDate { get; set; }

    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }

    public User User { get; set; }
    public Product Product { get; set; }
}
