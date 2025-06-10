using Microsoft.AspNetCore.Mvc;

namespace MediPlan.Models
{
    public class Usluga
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public string Ikona { get; set; }
        public Odjel Odjel { get; set; }
    }

}