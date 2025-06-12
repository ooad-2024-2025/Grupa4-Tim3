using Microsoft.AspNetCore.Mvc;

namespace MEDIPLAN.Models
{
    public class DetaljiUslugeViewModel
    {
        public Usluge Usluge { get; set; }
        public List<Korisnici> Doktori { get; set; }
    }

}