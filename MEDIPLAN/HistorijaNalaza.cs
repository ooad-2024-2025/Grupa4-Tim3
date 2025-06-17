using System;
using System.Collections.Generic;

namespace MEDIPLAN;

public partial class HistorijaNalaza
{
    public int Id { get; set; }

    public int TerminId { get; set; }

    public string Opis { get; set; } = null!;

    public bool JeLiUspjesnoPoslano { get; set; }

    public DateTime DatumSlanja { get; set; }
}
