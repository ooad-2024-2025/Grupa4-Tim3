using Microsoft.AspNetCore.Mvc;
using MEDIPLAN.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System;
using MEDIPLAN.Models;
using QRCoder;


namespace MEDIPLAN.Controllers
{
    public class ProfilController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfilController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var korisniciIdString = HttpContext.Session.GetString("KorisniciId");
            if (string.IsNullOrEmpty(korisniciIdString) || !int.TryParse(korisniciIdString, out int korisniciId))
                return RedirectToAction("Login", "Account");

            var korisnik = await _context.Korisnici.FindAsync(korisniciId);
            if (korisnik == null)
                return RedirectToAction("Login", "Account");

            var sviTermini = await _context.Termini
                .Where(t => t.PacijentId == korisniciId)
                .Include(t => t.Doktor)
                .ToListAsync();

            var danas = DateTime.Now;

            var model = new ProfilViewModel
            {
                Ime = korisnik.Ime,
                Prezime = korisnik.Prezime,
                DatumRodjenja = korisnik.DatumRodjenja,
                QrKodBase64 = korisnik.QrKod,
                ZakazaniTermini = sviTermini
                    .Where(t => t.DatumVrijemePocetak > danas)
                    .Select(t => new TerminViewModel
                    {
                        Terminid = t.Id,
                        ImeDoktora = $"{t.Doktor.Ime} {t.Doktor.Prezime}",
                        DatumPregleda = t.DatumVrijemePocetak
                    }).ToList(),
                ZavrseniTermini = sviTermini
                    .Where(t => t.DatumVrijemePocetak <= danas)
                    .Select(t => new TerminViewModel
                    {
                        Terminid = t.Id,
                        ImeDoktora = $"{t.Doktor.Ime} {t.Doktor.Prezime}",
                        DatumPregleda = t.DatumVrijemePocetak
                    }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> OtkaziTermini(int Terminid)
        {
            var termin = await _context.Termini.FindAsync(Terminid);
            if (termin == null)
            {
                TempData["Greska"] = "Termin nije pronađen.";
                return RedirectToAction("Index");
            }

            var razlika = termin.DatumVrijemePocetak - DateTime.Now;
            if (razlika.TotalHours < 24)
            {
                TempData["Greska"] = "Termin se može otkazati samo ako je udaljen više od 24 sata.";
                return RedirectToAction("Index");
            }

            _context.Termini.Remove(termin);
            await _context.SaveChangesAsync();

            TempData["Poruka"] = "Termin je uspješno otkazan.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult PokreniIzmjenuTermina(int id)
        {
            return RedirectToAction("Zakazi", "Termin", new { id });
        }
    }
}

