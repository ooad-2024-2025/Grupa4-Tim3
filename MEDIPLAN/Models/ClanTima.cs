using System;
using MEDIPLAN.Models;

namespace MEDIPLAN.Models
{
    public class ClanTima
    {
        public int Id { get; set; }
        public required string Ime { get; set; }
        public required string Prezime { get; set; }
        public required string Email { get; set; }
        public Odjel Odjel { get; set; }
        // Dodaj još svojstava ako treba, npr. Telefon, Slika, Uloga itd.
    }
}