namespace Diploma.Contracts;

public class MakeReviewRequest
{
    public string ProductId { get; set; } = null!;
    public int Rating { get; set; }
    public string Comment { get; set; } = null!;
}
