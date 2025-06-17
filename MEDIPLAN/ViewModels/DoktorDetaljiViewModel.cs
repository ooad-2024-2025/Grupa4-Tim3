namespace MEDIPLAN.ViewModels
{
    public class DoktorDetaljiViewModel
    {
        public required Doktor Doktor { get; set; }
        public double ProsjekOcjenaDoktor { get; set; }
        public double ProsjekOcjenaKlinika { get; set; }
        public required List<Recenzija> Recenzije { get; set; } = new List<Recenzija>();
    }

    public class Doktor
    {
        public required string Ime { get; set; }
        public required string Prezime { get; set; }
        public required string Email { get; set; }
        public required MedicinskaUsluga MedicinskaUsluga { get; set; }
    }

    public class MedicinskaUsluga
    {
        public required string Napomena { get; set; }
    }

    public class Recenzija
    {
        public required Korisnik Korisnik { get; set; }
        public double OcjenaDoktor { get; set; }
        public double OcjenaKlinika { get; set; }
        public required string Tekst { get; set; }
    }

    public class Korisnik
    {
        public required string Ime { get; set; }
    }
}