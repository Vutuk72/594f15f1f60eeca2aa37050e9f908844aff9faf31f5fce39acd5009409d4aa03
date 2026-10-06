using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

// Jen pro roli "admin". Nepřihlášený dostane 401, přihlášený bez role admin 403.
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class AdminController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { Message = $"Ahoj {User.Identity?.Name}, tohle vidí jen admin." });
    }
}
