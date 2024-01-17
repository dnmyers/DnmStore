using DnmStore.Server.Data;
using DnmStore.Server.Dto;
using DnmStore.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DnmStore.Server.Controllers;

[ApiController]
public class RegistrationController : ControllerBase {
    private readonly ApplicationDbContext _authContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;
    private readonly IConfiguration _configuration;

    public RegistrationController(UserManager<ApplicationUser> userManager,
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

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody]RegisterRequest request) {
        var userExists = await _userManager.FindByNameAsync(request.UserName);

        if (userExists is not null) {
            // 400
            return BadRequest("User already exists");
        }

        var user = new ApplicationUser {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded) {
            // 400
            return BadRequest("Failed to register user");
        }

        return Ok(result);
    }
}