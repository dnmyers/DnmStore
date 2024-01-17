using DnmStore.Server.Models;

public class ExternalLogin {
	public int Id { get; set; }
    public string LoginProvider { get; set; } = string.Empty;
    public string ProviderKey { get; set; } = string.Empty;
    public string ProviderDisplayName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = new ApplicationUser();
}