namespace DnmStore.Server.Models;

public class OrderItem {
    public int Id { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public int OrderId { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
    public int ProductId { get; set; }
    // Preserve the ProductName for this order in case the ProductName changes in the future
    public string ProductName { get; set; } = string.Empty;
    // Preserve the ProductDescription for this order in case the ProductDescription changes in the future
    public string ProductDescription { get; set; } = string.Empty;

    public Order Order { get; set; } = new Order();
    public Product Product { get; set; } = new Product();
}