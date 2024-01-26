namespace DnmStore.Server.Dto;

/// <summary>
/// Represents a request to refresh a token.
/// </summary>
public class RefreshTokenRequest {
    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    public string RefreshToken { get; set; } = null!;
}