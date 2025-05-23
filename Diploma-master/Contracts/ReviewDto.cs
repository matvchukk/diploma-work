namespace Diploma.Contracts;

public class ReviewDto
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
}
