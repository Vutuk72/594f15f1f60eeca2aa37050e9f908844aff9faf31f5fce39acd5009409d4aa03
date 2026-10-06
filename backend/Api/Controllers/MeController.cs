using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

// Jen pro přihlášené uživatele (jakákoli role). Bez platného tokenu vrátí 401.
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Username = User.Identity?.Name,
            Roles = User.FindAll("roles").Select(c => c.Value).ToArray()
        });
    }
}
