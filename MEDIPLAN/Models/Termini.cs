using System;
using System.Collections.Generic;

namespace MEDIPLAN.Models;

public partial class Termini
{
    public int Id { get; set; }

    public int PacijentId { get; set; }

    public int DoktorId { get; set; }

    public DateTime DatumVrijemePocetak { get; set; }

    public DateTime DatumVrijemeKraj { get; set; }

    public int Lokacija { get; set; }

    public int MedicinskeUslugeId { get; set; }

    public virtual Korisnici Doktor { get; set; } = null!;

    public virtual Korisnici Pacijent { get; set; } = null!;
}
