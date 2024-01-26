namespace DnmStore.Server.Dto;

/// <summary>
/// Represents the response returned after a successful authorization process.
/// </summary>
public class AuthorizationResponse {
    /// <summary>
    /// Gets or sets the user ID associated with the authorization.
    /// </summary>
    public string UserId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the authorization token generated for the user.
    /// </summary>
    public string AuthorizationToken { get; set; } = null!;

    /// <summary>
    /// Gets or sets the refresh token used for refreshing the authorization token.
    /// </summary>
    public string RefreshToken { get; set; } = null!;
}