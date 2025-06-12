using System;
using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Ime { get; set; }

    [Required]
    public string Prezime { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Lozinka { get; set; }

    [DataType(DataType.Password)]
    [Compare("Lozinka", ErrorMessage = "Lozinke se ne podudaraju.")]
    public string PotvrdiLozinku { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime DatumRodjenja { get; set; }
}
