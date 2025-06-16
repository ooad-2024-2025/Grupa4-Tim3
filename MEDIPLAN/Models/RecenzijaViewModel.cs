using System.ComponentModel.DataAnnotations;

public class RecenzijaViewModel
{
    public int TerminId { get; set; }
    public int DoktorId { get; set; }
    public string DoktorIme { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ocjena doktora je obavezna.")]
    [Range(1, 5, ErrorMessage = "Ocjena doktora mora biti između 1 i 5.")]
    public int OcjenaDoktor { get; set; }

    [Required(ErrorMessage = "Ocjena klinike je obavezna.")]
    [Range(1, 5, ErrorMessage = "Ocjena klinike mora biti između 1 i 5.")]
    public int OcjenaKlinika { get; set; }

    [Required(ErrorMessage = "Komentar je obavezan.")]
    [StringLength(1000, ErrorMessage = "Komentar ne smije biti duži od 1000 karaktera.")]
    public string Tekst { get; set; } = string.Empty;
}
