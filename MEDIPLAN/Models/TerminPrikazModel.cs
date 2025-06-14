using System;
using System.Collections.Generic;


namespace MEDIPLAN.Models
{
    public class TerminPrikazViewModel
    {
        public int Terminid { get; set; }
        public string ImeDoktora { get; set; }
        public DateTime DatumPregleda { get; set; }

        // Možeš ostaviti i staro ako koristiš isti model i za doktora
        public string? PacijentImePrezime { get; set; }
        public string? NazivUsluge { get; set; }
        public DateTime? Datum { get; set; }
    }

}