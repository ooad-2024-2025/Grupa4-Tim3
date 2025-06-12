using System;
using System.Collections.Generic;

namespace MEDIPLAN.Models;

public partial class Recenzije
{
    public int Id { get; set; }

    public string ImeKorisnika { get; set; } = null!;

    public string Tekst { get; set; } = null!;

    public DateTime Datum { get; set; }
}
