﻿namespace MEDIPLAN.Models
{
    public class ProduktivnostDoktoraDetaljno
    {
        public int DoktorId { get; set; }
        public string ImeDoktora { get; set; } = string.Empty;
        public int Godina { get; set; }
        public int Mjesec { get; set; }
        public int BrojPregleda { get; set; }
    }
}
