using System;
using System.Collections.Generic;
using MEDIPLAN.Models;


namespace MEDIPLAN.Models
{
    public class Recenzija
    {
        public int Id { get; set; }

       
        public string Tekst { get; set; } = string.Empty;

        public DateTime Datum { get; set; }

        public int KorisnikId { get; set; }
        public Korisnici Korisnik { get; set; } = null!;  
        public int DoktorId { get; set; }
        public Korisnici Doktor { get; set; } = null!;

        public int TerminId { get; set; }
        public Termini Termin { get; set; } = null!;

        public int OcjenaDoktor { get; set; }
        public int OcjenaKlinika { get; set; }
    }
}
