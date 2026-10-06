using System;
using System.Collections.Generic;

namespace Api.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Name { get; set; }

    /// <summary>
    /// telefonní čísla
    /// </summary>
    public string TelNumbers { get; set; } = null!;

    /// <summary>
    /// ID uživatele v Keycloaku (claim "sub" v tokenu)
    /// </summary>
    public Guid KeycloakId { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    public string? Email { get; set; }
}
