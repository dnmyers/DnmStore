namespace DnmStore.Server.Dto;

public class AuthorizationResponse {
    public string UserId { get; set; } = null!;
    public string AuthorizationToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}