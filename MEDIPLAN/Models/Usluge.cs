using System;
using System.Collections.Generic;

namespace MEDIPLAN.Models;

public partial class Usluge
{
    public int Id { get; set; }

    public string Naziv { get; set; } = null!;

    public string Opis { get; set; } = null!;

    public string Ikona { get; set; } = null!;

    public int Odjel { get; set; }
}
