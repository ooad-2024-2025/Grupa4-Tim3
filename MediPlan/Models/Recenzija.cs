using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Recenzija
{
    [Key]
    public int Id { get; set; }

    public string Tekst { get; set; }

    public Ocjena Ocjena { get; set; }

    [ForeignKey("Korisnik")]
    public int KorisnikId { get; set; }

    public Korisnik Korisnik { get; set; } // Navigation property
}
