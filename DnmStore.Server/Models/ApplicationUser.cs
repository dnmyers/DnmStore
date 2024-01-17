using Microsoft.AspNetCore.Identity;

namespace DnmStore.Server.Models;

public class ApplicationUser : IdentityUser {
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime Updated { get; set; } = DateTime.UtcNow;

    public ICollection<ExternalLogin> ExternalLogins { get; set; } = [];
}