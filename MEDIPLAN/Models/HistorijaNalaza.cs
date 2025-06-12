using System;
using System.Collections.Generic;

namespace MEDIPLAN.Models;

public partial class HistorijaNalaza
{
    public int Id { get; set; }

    public int Terminid { get; set; }

    public string Opis { get; set; } = null!;

    public bool JeLiUspjesnoPoslano { get; set; }

    public DateTime DatumSlanja { get; set; }
}
