using QRCoder;

public class ProfilViewModel
{
    public required string Ime { get; set; }
    public required string Prezime { get; set; }
    public required DateTime DatumRodjenja { get; set; }
    public List<TerminViewModel> ZakazaniTermini { get; set; } = new() { };
    public List<TerminViewModel> ZavrseniTermini { get; set; } = new() { };
    public string? QrKodBase64 { get; set; }
}
