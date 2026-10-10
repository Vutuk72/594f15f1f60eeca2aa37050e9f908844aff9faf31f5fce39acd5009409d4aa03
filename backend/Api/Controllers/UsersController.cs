using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Models;

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

    //GET api/users – seznam všech uživatelů, jen pro roli admin.
    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var u = await db.Users.AsNoTracking().ToListAsync();
        return Ok(u);
    }


    // TODO (vy): PUT api/users/me – vytvoří nebo upraví záznam přihlášeného uživatele (upsert).
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateMeRequest request)
    {
        // 1) Kdo to je – z tokenu
        var keycloakId = Guid.Parse(User.FindFirst("sub")!.Value);
        var email = User.FindFirst("email")?.Value;

        // 2) Najít jeho řádek (BEZ AsNoTracking – budeme ho měnit)
        var user = await db.Users.FirstOrDefaultAsync(u => u.KeycloakId == keycloakId);

        // 3) Neexistuje? Vytvořit nový.
        if (user is null)
        {
            user = new User { KeycloakId = keycloakId };
            db.Users.Add(user);
        }

        // 4) Přepsat hodnoty (u nového i existujícího stejně)
        user.Name = request.Name;
        user.TelNumbers = request.TelNumbers;
        user.Email = email;

        // 5) Uložit do databáze
        await db.SaveChangesAsync();

        return Ok(user);
    }
}
