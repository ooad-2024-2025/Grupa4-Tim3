using Microsoft.AspNetCore.Mvc;
using MEDIPLAN.Models;


namespace MEDIPLAN.Models
{
    public class DetaljiUslugeViewModel
    {
        public Usluge Usluge { get; set; }
        public List<Korisnici> Doktori { get; set; }
    }

}