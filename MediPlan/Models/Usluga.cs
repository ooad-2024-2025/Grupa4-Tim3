using Microsoft.AspNetCore.Mvc;

namespace MediPlan.Models
{
    public class Usluga
    {
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public string Ikona { get; set; } // putanja do slike ili ikone
        public VrstaMedicinskihUsluga Vrsta { get; set; } // tvoj enum
    }

}
