using System;
using MediPlan.Models;

public class Termin
{
    public int Id { get; set; }
    public int PacijentId { get; set; }
    public int DoktorId { get; set; }
    public DateTime DatumVrijemePocetak { get; set; }
    public DateTime DatumVrijemeKraj { get; set; }
    public Lokacija Lokacija { get; set; }
    public int MedicinskeUslugeId { get; set; }
}