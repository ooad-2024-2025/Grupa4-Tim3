using System.Collections.Generic;
using MEDIPLAN.Models;

namespace MEDIPLAN.ViewModels
{
    public class DoktorDetaljiViewModel
    {
        public Korisnici Doktor { get; set; } = null!;
        public List<Recenzija> Recenzije { get; set; } = new List<Recenzija>();

        public double ProsjekOcjenaDoktor { get; set; }
        public double ProsjekOcjenaKlinika { get; set; }
    }
}
