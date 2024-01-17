using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using DnmStore.Server.Data;
using DnmStore.Server.Dto;
using DnmStore.Server.Exceptions;
using DnmStore.Server.Models;

namespace eCommerce.Server.Controllers;

[ApiController]
public class AuthorizationController : ControllerBase {
    private readonly ApplicationDbContext _authContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;
    private readonly IConfiguration _configuration;

    public AuthorizationController(UserManager<ApplicationUser> userManager,
        ApplicationDbContext authContext,
        IConfiguration configuration,
        SignInManager<ApplicationUser> signInManager,
        IUserStore<ApplicationUser> userStore) {
        _userManager = userManager;
        _configuration = configuration;
        _signInManager = signInManager;
        _authContext = authContext;
        _emailStore = (IUserEmailStore<ApplicationUser>)userStore;
        _userStore = userStore;
    }

    [HttpPost("authorization/token")]
    public async Task<IActionResult> GetTokenAsync([FromBody] GetTokenRequest request) {
        var user = await _userManager.FindByNameAsync(request.UserName);

        if (user is null) {
            // 401 or 404
            return Unauthorized();
            throw new UserNotFoundException(request.UserName);
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordValid) {
            // 401 or 400
            return Unauthorized();
            throw new InvalidPasswordException(request.UserName);
        }

#pragma warning disable CS8604 // Possible null reference argument.
        var token = GenerateAuthorizationToken(user.Id, user.UserName);
#pragma warning restore CS8604 // Possible null reference argument.

        return Ok(token);
    }

    private AuthorizationResponse GenerateAuthorizationToken(string userId, string userName) {
        // This method creates a JWT and returns it in a well-defined response DTO.
        // Creates the claims, puts them into a JWT object, and signs it with the secret defined in appsettings.json

        var now = DateTime.UtcNow;
        var secret = _configuration.GetValue<string>("Secret");
#pragma warning disable CS8604 // Possible null reference argument.
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
#pragma warning restore CS8604 // Possible null reference argument.

        var userClaims = new List<Claim> {
            new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
            new Claim(ClaimTypes.NameIdentifier, userId),
        };

        var expires = now.Add(TimeSpan.FromHours(3));

        var jwt = new JwtSecurityToken(
            notBefore: now,
            claims: userClaims,
            expires: expires,
            audience: "https://localhost:7197",
            issuer: "https://localhost:7197",
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        var resp = new AuthorizationResponse {
            UserId = userId,
            AuthorizationToken = encodedJwt,
            RefreshToken = string.Empty
        };

        return resp;
    }
}