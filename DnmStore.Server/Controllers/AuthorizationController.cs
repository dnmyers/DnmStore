using System.Data.SqlTypes;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text;
using DnmStore.Server.Data;
using DnmStore.Server.Dto;
using DnmStore.Server.Exceptions;
using DnmStore.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Octokit;

namespace eCommerce.Server.Controllers;

[ApiController]
public class AuthorizationController : ControllerBase
{
    private readonly ApplicationDbContext _appContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationController"/> class.
    /// </summary>
    /// <param name="userManager">The user manager.</param>
    /// <param name="authContext">The authentication context.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="signInManager">The sign-in manager.</param>
    /// <param name="userStore">The user store.</param>
    public AuthorizationController(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext appContext,
        IConfiguration configuration,
        SignInManager<ApplicationUser> signInManager,
        IUserStore<ApplicationUser> userStore
    )
    {
        _userManager = userManager;
        _configuration = configuration;
        _signInManager = signInManager;
        _appContext = appContext;
        _emailStore = (IUserEmailStore<ApplicationUser>)userStore;
        _userStore = userStore;
    }

    /// <summary>
    /// Retrieves an authorization token for the specified user.
    /// </summary>
    /// <param name="request">The token request.</param>
    /// <returns>The authorization token.</returns>
    [HttpPost("authorization/token")]
    public async Task<IActionResult> GetTokenAsync([FromBody] GetTokenRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);

        if (user is null)
        {
            // 401 or 404
            return Unauthorized();
            throw new UserNotFoundException(request.UserName);
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordValid)
        {
            // 401 or 400
            return Unauthorized();
            throw new InvalidPasswordException(request.UserName);
        }

#pragma warning disable CS8604 // Possible null reference argument.
        var token = GenerateAuthorizationTokenAsync(user.Id, user.UserName);
#pragma warning restore CS8604 // Possible null reference argument.

        return Ok(token);
    }

    [HttpPost("authorization/refresh")]
    public async Task<ActionResult<AuthorizationResponse>> GetAuthorizationTokenFromRefreshAsync(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken
    )
    {
        var now = DateTime.UtcNow;

        var refreshToken =
            await _appContext
                .RefreshTokens
                .Include(x => x.User)
                .SingleOrDefaultAsync(r => r.Value == request.RefreshToken, cancellationToken)
            ?? throw new Exception("Refresh token not found");

        var token = await GenerateAuthorizationTokenAsync(
            refreshToken.User.Id,
            refreshToken.User.UserName
        );

        return Ok(token);
    }

    [HttpGet, AllowAnonymous, Route("authorization/external-login")]
    public IActionResult ExternalLogin(
        string provider,
        string returnUrl,
        CancellationToken cancellationToken
    )
    {
        var redirectUrl =
            $"https://localhost:7197/authorization/external-auth-callback?returnUrl={returnUrl}";
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(
            provider,
            redirectUrl
        );

        properties.AllowRefresh = true;
        properties.RedirectUri = redirectUrl;

        return Challenge(properties, provider);
    }

    [HttpGet, Route("authorization/external-auth-callback")]
    public async Task<IActionResult> ExternalLoginCallback(
        string returnUrl,
        string? remoteError = null
    )
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();

        if (info is null)
        {
            return BadRequest();
        }

        var emailClaim = info.Principal.HasClaim(c => c.Type == ClaimTypes.Email)
            ? info.Principal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Email).Value
            : string.Empty;

        if (string.IsNullOrEmpty(emailClaim))
        {
            return BadRequest();
        }

        var uri = new Uri(returnUrl);

        IdentityUser user;

        try
        {
            user = await ExternalSignInAsync(emailClaim, info.LoginProvider, info.ProviderKey);
        }
        catch (ExternalUserDoesNotExistException)
        {
            try
            {
                await CreateUserFromExternalAuthAsync(emailClaim, info);

                user = await ExternalSignInAsync(emailClaim, info.LoginProvider, info.ProviderKey);
            }
            catch (Exception ex)
            {
                returnUrl = AddQueryParameter(returnUrl, $"errorCode={ex.Message}");
                return Redirect(returnUrl);
            }
        }

        var token = await GenerateAuthorizationTokenAsync(user.Id, user.UserName);

        var tokenJson = JsonConvert.SerializeObject(token);
        var bytes = Encoding.UTF8.GetBytes(tokenJson);
        var encodedToken = Convert.ToBase64String(bytes);
        returnUrl = AddQueryParameter(returnUrl, $"token={encodedToken}");

        return Redirect(returnUrl);

        string AddQueryParameter(string url, string parameter)
        {
            return uri.Query.Length == 0 ? $"{url}?{parameter}" : $"{url}&{parameter}";
        }
    }

    private async Task CreateUserFromExternalAuthAsync(string email, UserLoginInfo info)
    {
        var user = new ApplicationUser();

        await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, email, CancellationToken.None);

        var createResult = await _userManager.CreateAsync(user);

        if (!createResult.Succeeded)
        {
            throw new Exception($"User manager cannot create user");
        }

        await _userManager.AddClaimAsync(user, new Claim("email", email));

        var result = await _userManager.AddLoginAsync(user, info);

        if (!result.Succeeded)
        {
            throw new Exception($"Cannot add external login for user");
        }
    }

    private async Task<ApplicationUser> ExternalSignInAsync(
        string email,
        string loginProvider,
        string providerKey
    )
    {
        var result = await _signInManager.ExternalLoginSignInAsync(
            loginProvider,
            providerKey,
            false
        );

        if (!result.Succeeded)
        {
            throw new ExternalUserDoesNotExistException();
        }

        var user =
            await _userManager.FindByEmailAsync(email)
            ?? throw new Exception("User's email not found");

        return user;
    }

    /// <summary>
    /// Generates an authorization token for the specified user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="userName">The user name.</param>
    /// <returns>The generated authorization token.</returns>
    private async Task<AuthorizationResponse> GenerateAuthorizationTokenAsync(
        string userId,
        string userName
    )
    {
        // This method creates a JWT and returns it in a well-defined response DTO.
        // Creates the claims, puts them into a JWT object, and signs it with the secret defined in appsettings.json

        var now = DateTime.UtcNow;
        var secret = _configuration.GetValue<string>("Secret");
#pragma warning disable CS8604 // Possible null reference argument.
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
#pragma warning restore CS8604 // Possible null reference argument.

        var userClaims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
            new Claim(ClaimTypes.NameIdentifier, userId),
        };

        var jwt = new JwtSecurityToken(
            notBefore: now,
            claims: userClaims,
            expires: now.Add(TimeSpan.FromMinutes(60)),
            audience: "https://localhost:7197/",
            issuer: "https://localhost:7197/",
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        var refreshToken = await _appContext
            .RefreshTokens
            .SingleOrDefaultAsync(r => r.ApplicationUserId == userId);

        if (refreshToken is not null)
        {
            _appContext.RefreshTokens.Remove(refreshToken);
        }

        var user = await _appContext.Users.SingleOrDefaultAsync(u => u.Id == userId);

        var newRefreshToken = new RefreshToken(
            Guid.NewGuid().ToString(),
            TimeSpan.FromDays(1000),
            userId
        )
        {
            User = user
        };

        _appContext.RefreshTokens.Add(newRefreshToken);

        await _appContext.SaveChangesAsync();

        var resp = new AuthorizationResponse
        {
            UserId = userId,
            AuthorizationToken = encodedJwt,
            RefreshToken = newRefreshToken.Value
        };

        return resp;
    }
}
