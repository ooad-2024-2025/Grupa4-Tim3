public class ProfilViewModel
{
    public string Ime { get; set; }
    public string Prezime { get; set; }
    public DateTime DatumRodjenja { get; set; }
    public List<TerminViewModel> ZakazaniTermini { get; set; }
    public List<TerminViewModel> ZavrseniTermini { get; set; }
}