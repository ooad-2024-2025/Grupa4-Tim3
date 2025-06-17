using Microsoft.AspNetCore.Mvc;
using MEDIPLAN.Models;

namespace MEDIPLAN.Models
{
    public class DetaljiUslugeViewModel
    {
        public required Usluge Usluge { get; set; }
        public List<Korisnici> Doktori { get; set; } = new();
    }
}