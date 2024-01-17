namespace DnmStore.Server.Models;

public class Order {
    public int Id { get; set; }
    public double Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime Updated { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = new ApplicationUser();
    public List<OrderItem> OrderItems { get; set; } = [];
}