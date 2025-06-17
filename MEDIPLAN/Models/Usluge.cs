using System;
using System.Collections.Generic;
using MEDIPLAN.Models;


namespace MEDIPLAN.Models;

public partial class Usluge
{
    public int Id { get; set; }

    public required string Naziv { get; set; }
    public required string Opis { get; set; }
    public required string Ikona { get; set; }


    public int Odjel { get; set; }
}
