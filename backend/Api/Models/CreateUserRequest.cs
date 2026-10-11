using System.ComponentModel.DataAnnotations;

namespace Api.Models;

// Schválně BEZ kontroly formátu telefonu (žádný [RegularExpression]) – uvidíme, co projde.
public record CreateUserRequest(
    [MaxLength(100)] string? Name,
    [Required][MaxLength(20)] string TelNumbers,
    [EmailAddress][MaxLength(254)] string? Email
);