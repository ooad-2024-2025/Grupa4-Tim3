namespace MEDIPLAN.Models
{
    public class StatistikaViewModel
    {
        public List<PregledPoMjesecu> PreglediPoMjesecima { get; set; } = new();
        public List<ProduktivnostDoktora> ProduktivnostDoktora { get; set; } = new();
        public int UkupnoPacijenata { get; set; }
    }
}
