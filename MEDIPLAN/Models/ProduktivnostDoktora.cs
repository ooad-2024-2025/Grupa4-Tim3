namespace MEDIPLAN.Models
{
    public class ProduktivnostDoktora
    {
        public int DoktorId { get; set; }
        public string ImePrezime { get; set; } = string.Empty; // Default value added
        public int BrojPregleda { get; set; }
    }
}
