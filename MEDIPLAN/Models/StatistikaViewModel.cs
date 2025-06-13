namespace MEDIPLAN.Models
{
    public class StatistikaViewModel
    {
        public List<PregledPoMjesecu> PreglediPoMjesecima { get; set; } = new List<PregledPoMjesecu>();
        public int UkupnoPacijenata { get; set; }
    }
}
