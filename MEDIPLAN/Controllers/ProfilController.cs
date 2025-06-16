using MEDIPLAN.Data;
using MEDIPLAN.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            if (!IsUserLoggedIn())
            {
                TempData["Greska"] = "Morate biti prijavljeni da biste pristupili profilu.";
                return RedirectToAction("Login", "Account");
            }

            var korisniciId = HttpContext.Session.GetInt32("KorisniciId");
            if (!korisniciId.HasValue)
            {
                TempData["Greska"] = "Korisnički podaci nisu pronađeni.";
                return RedirectToAction("Login", "Account");
            }

            var korisnik = await _context.Korisnici.FindAsync(korisniciId.Value);
            if (korisnik == null)
            {
                TempData["Greska"] = "Korisnik nije pronađen.";
                return RedirectToAction("Login", "Account");
            }

            var sviTermini = await _context.Termini
                .Where(t => t.PacijentId == korisniciId.Value)
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
                        DatumPregleda = t.DatumVrijemePocetak,
                        JeRecenziran = t.JeRecenziran
                    }).ToList(),
                ZavrseniTermini = sviTermini
                    .Where(t => t.DatumVrijemePocetak <= danas)
                    .Select(t => new TerminViewModel
                    {
                        Terminid = t.Id,
                        ImeDoktora = $"{t.Doktor.Ime} {t.Doktor.Prezime}",
                        DatumPregleda = t.DatumVrijemePocetak,
                        JeRecenziran = t.JeRecenziran
                    }).ToList()
            };

            return View(model);
        }

        private bool IsUserLoggedIn()
        {
            var korisniciId = HttpContext.Session.GetInt32("KorisniciId");
            var username = HttpContext.Session.GetString("Username");

            return korisniciId.HasValue && !string.IsNullOrEmpty(username);
        }
    }
}
