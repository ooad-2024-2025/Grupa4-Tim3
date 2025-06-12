using System;
public class Korisnik
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Ime { get; set; }
    public string Prezime { get; set; }
    public string Email { get; set; }
    public string Lozinka { get; set; }
    public DateTime DatumRodjenja { get; set; }
    public Uloga Uloga { get; set; }
    public string QrKod { get; set; }
    public Odjel Odjel { get; set; }
}
