using System;
using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Obavezno polje")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Korisničko ime mora imati između 3 i 50 karaktera")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Obavezno polje")]
    public string Ime { get; set; }

    [Required(ErrorMessage = "Obavezno polje")]
    public string Prezime { get; set; }

    [Required(ErrorMessage = "Obavezno polje")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "Obavezno polje")]
    [DataType(DataType.Password)]
    public string Lozinka { get; set; }

    [Required(ErrorMessage = "Obavezno polje")]
    [DataType(DataType.Password)]
    [Compare("Lozinka", ErrorMessage = "Lozinke se ne podudaraju.")]
    public string PotvrdiLozinku { get; set; }

    [Required(ErrorMessage = "Obavezno polje")]
    [DataType(DataType.Date)]
    public DateTime DatumRodjenja { get; set; }
}



