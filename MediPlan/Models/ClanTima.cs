using System;

namespace MediPlan.Models
{
    public class ClanTima
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public Odjel Odjel { get; set; }
        // Dodaj još svojstava ako treba, npr. Telefon, Slika, Uloga itd.
    }
}
