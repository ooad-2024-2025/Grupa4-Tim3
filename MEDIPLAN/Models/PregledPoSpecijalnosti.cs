namespace MEDIPLAN.Models
{
    public class PregledPoSpecijalnosti
    {
        public int Godina { get; set; }
        public int Mjesec { get; set; }
        public string NazivUsluge { get; set; } = string.Empty;
        public int BrojPregleda { get; set; }
    }
}
