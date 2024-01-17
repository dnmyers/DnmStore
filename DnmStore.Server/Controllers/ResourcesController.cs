using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DnmStore.Server.Controllers;

[ApiController]
public class ResourcesController : ControllerBase {
    [HttpGet("api/resources"), Authorize]
    public IActionResult GetResources() {
        return Ok($"Protected resources, username: {User.Identity!.Name}");
    }
}