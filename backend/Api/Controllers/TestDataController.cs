using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

// Testovací data pro vývoj – jen pro admina.
// Vygenerovaní uživatelé mají e-mail na doméně TestDomain, podle ní je jde poznat a zase smazat.
// Jejich keycloak_id je náhodné – v Keycloaku takoví uživatelé neexistují, přihlásit se nemohou.
[ApiController]
[Route("api/test-data")]
[Authorize(Roles = "admin")]
public class TestDataController(AppDbContext db) : ControllerBase
{
    private const string TestDomain = "@test.ekospol.local";

    // Mužská a ženská jména a příjmení zvlášť, ať nevznikne "Lucie Veselý".
    private static readonly string[] MaleFirstNames =
        ["Jan", "Petr", "Pavel", "Tomáš", "Martin", "Jakub", "Lukáš", "Ondřej", "David", "Michal"];

    private static readonly string[] FemaleFirstNames =
        ["Jana", "Eva", "Hana", "Lucie", "Kateřina", "Tereza", "Petra", "Markéta", "Veronika", "Lenka"];

    private static readonly string[] MaleLastNames =
        ["Novák", "Svoboda", "Novotný", "Dvořák", "Černý", "Procházka", "Kučera", "Veselý", "Horák", "Němec"];

    private static readonly string[] FemaleLastNames =
        ["Nováková", "Svobodová", "Novotná", "Dvořáková", "Černá", "Procházková", "Kučerová", "Veselá", "Horáková", "Němcová"];

    // POST api/test-data?count=50 – přidá zadaný počet náhodných uživatelů.
    [HttpPost]
    public async Task<IActionResult> Generate([FromQuery] int count = 50)
    {
        if (count is < 1 or > 1000)
        {
            return BadRequest("Počet musí být 1 až 1000.");
        }

        // Telefon i e-mail jsou v DB unikátní – načteme existující, ať nevygenerujeme duplicitu.
        var usedPhones = (await db.Users.Select(u => u.TelNumbers).ToListAsync()).ToHashSet();
        var usedEmails = (await db.Users.Where(u => u.Email != null).Select(u => u.Email!).ToListAsync()).ToHashSet();

        var users = new List<User>();
        while (users.Count < count)
        {
            var phone = $"+420{Random.Shared.Next(6, 8)}{Random.Shared.Next(0, 100_000_000):D8}";
            var female = Random.Shared.Next(2) == 0;
            var firstNames = female ? FemaleFirstNames : MaleFirstNames;
            var lastNames = female ? FemaleLastNames : MaleLastNames;
            var firstName = firstNames[Random.Shared.Next(firstNames.Length)];
            var lastName = lastNames[Random.Shared.Next(lastNames.Length)];
            var email = $"{RemoveDiacritics(firstName)}.{RemoveDiacritics(lastName)}{Random.Shared.Next(1000, 10000)}{TestDomain}".ToLowerInvariant();

            if (!usedPhones.Add(phone) || !usedEmails.Add(email))
            {
                continue; // náhodou duplicita – zkusit znovu
            }

            users.Add(new User
            {
                KeycloakId = Guid.NewGuid(),
                Name = $"{firstName} {lastName}",
                TelNumbers = phone,
                Email = email,
            });
        }

        // AddRange + jedno SaveChanges = všechny řádky v jedné transakci.
        db.Users.AddRange(users);
        await db.SaveChangesAsync();

        return Ok(new { Created = users.Count, Total = await db.Users.CountAsync() });
    }

    // DELETE api/test-data – smaže všechny vygenerované uživatele (skutečné nechá být).
    [HttpDelete]
    public async Task<IActionResult> DeleteAll()
    {
        // ExecuteDeleteAsync pošle jeden DELETE ... WHERE přímo do DB, bez načítání řádků do paměti.
        var deleted = await db.Users
            .Where(u => u.Email != null && u.Email.EndsWith(TestDomain))
            .ExecuteDeleteAsync();

        return Ok(new { Deleted = deleted, Total = await db.Users.CountAsync() });
    }

    private static string RemoveDiacritics(string text) =>
        new(text.Normalize(System.Text.NormalizationForm.FormD)
            .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
                != System.Globalization.UnicodeCategory.NonSpacingMark)
            .ToArray());
}
