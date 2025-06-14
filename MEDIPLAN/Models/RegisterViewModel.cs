using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Korisničko ime je obavezno")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Korisničko ime mora imati 3-20 karaktera")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Ime je obavezno")]
    public string Ime { get; set; }

    [Required(ErrorMessage = "Prezime je obavezno")]
    public string Prezime { get; set; }

    [Required(ErrorMessage = "Email je obavezan")]
    [EmailAddress(ErrorMessage = "Neispravan format emaila")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Lozinka je obavezna")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Lozinka mora imati najmanje 6 karaktera")]
    public string Lozinka { get; set; }

    [DataType(DataType.Password)]
    [Compare("Lozinka", ErrorMessage = "Lozinke se ne podudaraju")]
    public string PotvrdiLozinku { get; set; }

    [Required(ErrorMessage = "Datum rođenja je obavezan")]
    [DataType(DataType.Date)]
    public DateTime DatumRodjenja { get; set; }
}

public class ResendVerificationModel
{
    [Required(ErrorMessage = "Email je obavezan")]
    [EmailAddress(ErrorMessage = "Neispravan format emaila")]
    public string Email { get; set; }
}