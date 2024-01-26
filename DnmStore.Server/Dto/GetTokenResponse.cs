namespace DnmStore.Server.Dto;

/// <summary>
/// Represents a request to get a token.
/// </summary>
public class GetTokenRequest {
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string UserName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; set; } = null!;
}