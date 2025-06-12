using System;
using System.Collections.Generic;

namespace MEDIPLAN.Models;

public partial class MedicinskeUsluge
{
    public int Id { get; set; }

    public int VrstaUsluge { get; set; }

    public int Cijena { get; set; }

    public string Napomena { get; set; } = null!;

    public int? IdDoktora { get; set; }
}
