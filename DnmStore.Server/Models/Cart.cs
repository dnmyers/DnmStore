namespace DnmStore.Server.Models;

public class Cart {
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int ProductId { get; set; }

    public ApplicationUser User { get; set; } = new ApplicationUser();
    public ICollection<CartItem> CartItems { get; set; } = [];
}