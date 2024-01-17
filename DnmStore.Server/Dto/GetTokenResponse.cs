namespace DnmStore.Server.Dto;

public class GetTokenRequest {
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
}