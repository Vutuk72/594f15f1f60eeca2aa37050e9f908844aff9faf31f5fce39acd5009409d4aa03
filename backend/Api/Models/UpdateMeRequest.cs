using System.ComponentModel.DataAnnotations;

namespace Api.Models;

public record UpdateMeRequest(
    [MaxLength(100, ErrorMessage = "Jméno může mít nejvýš 100 znaků.")]
    string? Name,

    [Required(ErrorMessage = "Telefon je povinný.")]
    [MaxLength(20, ErrorMessage = "Telefon může mít nejvýš 20 znaků.")]
    [RegularExpression(@"^\+[1-9]\d{6,14}$",
        ErrorMessage = "Telefon zadejte v mezinárodním formátu bez mezer, např. +420777123456.")]
    string TelNumbers
);