namespace Diploma.Entities;

public class WishedItem
{
    public Guid ProductId { get; set; }
    public Guid WishListId { get; set; }
    public Product Product { get; set; }
    public WishList WishList { get; set; }
}
