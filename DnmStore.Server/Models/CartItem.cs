namespace DnmStore.Server.Models;

public class CartItem {
    public int Id { get; set; }
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    public Product Product { get; set; } = new Product();
    public Cart Cart { get; set; } = new Cart();
}