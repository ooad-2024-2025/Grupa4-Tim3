using System;
using System.Collections.Generic;

namespace MEDIPLAN.Models;

public partial class Korisnici
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Ime { get; set; } = null!;

    public string Prezime { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Lozinka { get; set; } = null!;

    public DateTime DatumRodjenja { get; set; }

    public int Uloga { get; set; }

    public string QrKod { get; set; } = null!;

    public int Odjel { get; set; }

    public int? MedicinskaUslugaId { get; set; }

    public virtual MedicinskeUsluge? MedicinskaUsluga { get; set; }
}
