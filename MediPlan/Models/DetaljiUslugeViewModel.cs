using Microsoft.AspNetCore.Mvc;

namespace MediPlan.Models
{
    public class DetaljiUslugeViewModel
    {
        public Usluga Usluga { get; set; }
        public List<Korisnik> Doktori { get; set; }
    }

}