using System;
using System.Collections.Generic;
using MEDIPLAN.Models;


namespace MEDIPLAN.Models;

public partial class Korisnici
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Ime { get; set; } = null!;

    public string Prezime { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Lozinka { get; set; } = null!;

    public DateTime DatumRodjenja { get; set; }

    public int Uloga { get; set; }

    public string? QrKod { get; set; }

    public int Odjel { get; set; }

    public int? MedicinskaUslugaId { get; set; }

    public string? PhotoFileName { get; set; }

    public string? VerificationToken { get; set; }

    public bool IsVerified { get; set; }
    public virtual ICollection<Notifikacije> Notifikacije { get; set; } = new List<Notifikacije>();


    public virtual MedicinskeUsluge? MedicinskaUsluga { get; set; }

    public virtual ICollection<Termini> TerminiDoktor { get; set; } = new List<Termini>();

    public virtual ICollection<Termini> TerminiPacijent { get; set; } = new List<Termini>();
}
