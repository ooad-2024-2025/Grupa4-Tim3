using System.Collections.Generic;
using MEDIPLAN.Models;

namespace MEDIPLAN.Models // Updated namespace to match folder structure
{
    public class DoktorDetaljiViewModel
    {
        public Korisnici Doktor { get; set; } = null!;
        public List<Recenzija> Recenzije { get; set; } = new List<Recenzija>();

        public double ProsjekOcjenaDoktor { get; set; }
        public double ProsjekOcjenaKlinika { get; set; }
    }
}
