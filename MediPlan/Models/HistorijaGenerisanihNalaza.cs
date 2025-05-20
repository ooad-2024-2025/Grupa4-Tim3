using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class HistorijaGenerisanihNalaza
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Termin")]
    public int TerminId { get; set; }

    public string Opis { get; set; }
    public bool JeLiUspjesnoPoslano { get; set; }
    public DateTime DatumSlanja { get; set; }

    public Termin Termin { get; set; } // Navigation property
}
