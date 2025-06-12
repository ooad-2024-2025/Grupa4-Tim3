using System;
using System.Collections.Generic;

namespace MEDIPLAN.Models;

public partial class MedicinskeUsluge
{
    public int Id { get; set; }

    public string Napomena { get; set; } = null!;

    public virtual ICollection<Korisnici> Korisnicis { get; set; } = new List<Korisnici>();
}
