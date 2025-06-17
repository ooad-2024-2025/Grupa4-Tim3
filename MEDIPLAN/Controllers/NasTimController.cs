using MEDIPLAN.Data;
using MEDIPLAN.Models;
using MEDIPLAN.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MEDIPLAN.Controllers
{
    public class NasTimController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NasTimController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Tim()
        {
            var doktori = _context.Korisnici
                .Include(k => k.MedicinskaUsluga)
                .Where(k => k.Uloga == (int)Uloga.Doktor)
                .ToList();

            var doktoriPoUslugama = doktori
                .Where(d => d.MedicinskaUsluga != null)
                .GroupBy(d => d.MedicinskaUsluga!.Napomena)
                .ToDictionary(g => g.Key, g => g.ToList());

            return View(doktoriPoUslugama);
        }

        public IActionResult Detalji(int doktorId)
        {
            var doktor = _context.Korisnici
                .Where(d => d.Id == doktorId && d.Uloga == (int)Uloga.Doktor)
                .Select(d => new ViewModels.Doktor
                {
                    Ime = d.Ime,
                    Prezime = d.Prezime,
                    Email = d.Email,
                    MedicinskaUsluga = new ViewModels.MedicinskaUsluga
                    {
                        Napomena = d.MedicinskaUsluga!.Napomena
                    }
                })
                .FirstOrDefault();

            if (doktor == null)
                return NotFound();

            var recenzije = _context.Recenzije
                .Where(r => r.DoktorId == doktorId)
                .Include(r => r.Korisnik)
                .Select(r => new ViewModels.Recenzija
                {
                    Tekst = r.Tekst,
                    Korisnik = new ViewModels.Korisnik
                    {
                        Ime = r.Korisnik.Ime
                    },
                    OcjenaDoktor = r.OcjenaDoktor,
                    OcjenaKlinika = r.OcjenaKlinika
                })
                .ToList();

            double prosjekOcjenaDoktor = recenzije.Count > 0 ? recenzije.Average(r => r.OcjenaDoktor) : 0;
            double prosjekOcjenaKlinika = recenzije.Count > 0 ? recenzije.Average(r => r.OcjenaKlinika) : 0;

            var model = new ViewModels.DoktorDetaljiViewModel
            {
                Doktor = doktor,
                Recenzije = recenzije,
                ProsjekOcjenaDoktor = prosjekOcjenaDoktor,
                ProsjekOcjenaKlinika = prosjekOcjenaKlinika
            };

            return View(model);
        }
    }
}
