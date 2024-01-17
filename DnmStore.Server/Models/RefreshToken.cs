namespace DnmStore.Server.Models;

public class RefreshToken {
    public string Id { get; private set; } = null!;

    public RefreshToken(string value, TimeSpan duration, string userId) {
        Id = Guid.NewGuid().ToString();
        Value = value;
        Expiration = DateTime.UtcNow.Add(duration);
        ApplicationUserId = userId;
    }

    private RefreshToken() { }

    public string Value { get; private set; } = string.Empty;
    public DateTime Expiration { get; private set; }
    public string ApplicationUserId { get; private set; } = string.Empty;
    public ApplicationUser User { get; set; } = new ApplicationUser();
}