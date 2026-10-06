using Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(AppDbContext db) : ControllerBase
{
    // GET api/users/me – vrátí záznam přihlášeného uživatele z tabulky users.
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        // "sub" = ID uživatele v Keycloaku, je v každém tokenu.
        var keycloakId = Guid.Parse(User.FindFirst("sub")!.Value);

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.KeycloakId == keycloakId);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var u = await db.Users.AsNoTracking().ToListAsync();
        return Ok(u);
    }
    // TODO (vy): GET api/users – seznam všech uživatelů, jen pro roli admin.

    // TODO (vy): PUT api/users/me – vytvoří nebo upraví záznam přihlášeného uživatele (upsert).
}
