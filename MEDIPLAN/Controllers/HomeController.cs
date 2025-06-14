using Microsoft.AspNetCore.Mvc;
using MEDIPLAN.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MEDIPLAN.Data;
using Microsoft.EntityFrameworkCore;

namespace MEDIPLAN.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() => View();

        public IActionResult About() => View();

        public IActionResult Usluge()
        {
            var usluge = new List<Usluge>
            {
                new Usluge { Naziv = "Interna Medicina", Ikona = "images/interna-medicina.png", Opis = "Opis za Internu Medicinu" },
                new Usluge { Naziv = "Kardiologija", Ikona = "images/kardiologija.png", Opis = "Opis za Kardiologiju" },
                new Usluge { Naziv = "Oftamologija", Ikona = "images/oftamologija.png", Opis = "Opis za Oftamologiju" },
                new Usluge { Naziv = "Dermatologija", Ikona = "images/dermatologija.png", Opis = "Opis za Dermatologiju" },
                new Usluge { Naziv = "Endokrinologija", Ikona = "images/endokrinologija.png", Opis = "Opis za Endokrinologiju" },
                new Usluge { Naziv = "Ginekologija", Ikona = "images/gynecology.png", Opis = "Opis za Ginekologiju" },
                new Usluge { Naziv = "Neurologija", Ikona = "images/neurologija.png", Opis = "Opis za Neurologiju" },
                new Usluge { Naziv = "Radiologija", Ikona = "images/radiology.png", Opis = "Opis za Radiologiju" }
            };

            return View(usluge);
        }

        public IActionResult DetaljiUsluge(string naziv)
        {
            if (string.IsNullOrEmpty(naziv))
                return RedirectToAction("Usluge");

            var usluga = _context.Usluge
                .Select(u => new Models.Usluge
                { 

                    Id = u.Id,
                    Naziv = u.Naziv,
                    Opis = u.Opis,
                    Ikona = u.Ikona,
                    Odjel = u.Odjel
                })
                .FirstOrDefault(u => u.Naziv == naziv);

            if (usluga == null)
                return NotFound();

            var doktoriZaUslugu = _context.Korisnici
                .Where(k => k.Uloga == (int)Uloga.Doktor && k.Odjel == usluga.Odjel)
                .ToList();

            var model = new DetaljiUslugeViewModel
            {
                Usluge = usluga,
                Doktori = doktoriZaUslugu
            };

            return View(model);
        }

        public async Task<IActionResult> Statistika()
        {
            // Statistika 1: Pregledi po mjesecima i godinama
            var preglediPoMjesecima = await _context.Termini
                .GroupBy(t => new { t.DatumVrijemePocetak.Year, t.DatumVrijemePocetak.Month })
                .Select(g => new PregledPoMjesecu
                {
                    Godina = g.Key.Year,
                    Mjesec = g.Key.Month,
                    BrojPregleda = g.Count()
                })
                .OrderByDescending(x => x.Godina)
                .ThenByDescending(x => x.Mjesec)
                .ToListAsync();

            // Statistika 2: Produktivnost doktora
            var produktivnostDoktora = await _context.Termini
                .Include(t => t.Doktor)
                .GroupBy(t => new { t.DoktorId, t.Doktor.Ime, t.Doktor.Prezime })
                .Select(g => new ProduktivnostDoktora
                {
                    DoktorId = g.Key.DoktorId,
                    ImePrezime = g.Key.Ime + " " + g.Key.Prezime,
                    BrojPregleda = g.Count()
                })
                .OrderByDescending(x => x.BrojPregleda)
                .ToListAsync();

            // Ukupan broj pacijenata
            var ukupnoPacijenata = await _context.Termini
                .Select(t => t.PacijentId)
                .Distinct()
                .CountAsync();

            var model = new StatistikaViewModel
            {
                PreglediPoMjesecima = preglediPoMjesecima,
                ProduktivnostDoktora = produktivnostDoktora,
                UkupnoPacijenata = ukupnoPacijenata
            };

            return View(model);
        }

        public IActionResult Kontakt()
        {
            var lokacije = new List<LokacijaInfo>
            {
                new LokacijaInfo
                {
                    Grad = "Sarajevo",
                    Adresa = "Zmaja od Bosne 12",
                    Telefon = "+387 33 123 456",
                    Email = "info@MEDIPLAN.ba",
                    Slika = "images/grbavica.jpg"
                },
                new LokacijaInfo
                {
                    Grad = "Ilidža",
                    Adresa = "Butmirska Cesta 9",
                    Telefon = "+387 36 654 321",
                    Email = "info@MEDIPLAN.ba",
                    Slika = "images/butmir.jpg"
                }
            };

            return View(lokacije);
        }

        public IActionResult MojProfil() => View();

        public IActionResult Cjenovnik() => View();

        public IActionResult ZakaziTermini() => View();

        public IActionResult Privacy() => View();
    }
}
