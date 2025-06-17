using MEDIPLAN.Data;
using MEDIPLAN.Models;
using MEDIPLAN.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MEDIPLAN.Controllers
{
    public class RecenzijeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecenzijeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Recenzije/OstaviRecenziju?terminId=5
        [HttpGet]
        public async Task<IActionResult> OstaviRecenziju(int terminId)
        {
            var termin = await _context.Termini
                .Include(t => t.Doktor)
                .FirstOrDefaultAsync(t => t.Id == terminId);

            if (termin == null)
                return NotFound();

            var model = new RecenzijaViewModel
            {
                TerminId = termin.Id,
                DoktorId = termin.DoktorId,
                DoktorIme = $"{termin.Doktor.Ime} {termin.Doktor.Prezime}"
            };

            return View(model);
        }

        // POST: Recenzije/OstaviRecenziju
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OstaviRecenziju(RecenzijaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var termin = await _context.Termini.FindAsync(model.TerminId);
            if (termin == null)
                return NotFound();

            if (termin.JeRecenziran)
            {
                ModelState.AddModelError("", "Za ovaj termin je već ostavljena recenzija.");
                return View(model);
            }

            int? korisnikId = HttpContext.Session.GetInt32("KorisniciId");
            if (korisnikId == null)
                return RedirectToAction("Login", "Account");

            var recenzija = new MEDIPLAN.Models.Recenzija // Fully qualify the namespace
            {
                TerminId = model.TerminId,
                DoktorId = model.DoktorId,
                KorisnikId = korisnikId.Value,
                Tekst = model.Tekst,
                OcjenaDoktor = model.OcjenaDoktor,
                OcjenaKlinika = model.OcjenaKlinika,
                Datum = DateTime.Now
            };

            _context.Recenzije.Add(recenzija);
            termin.JeRecenziran = true;

            await _context.SaveChangesAsync();

            TempData["Poruka"] = "Recenzija je uspješno sačuvana.";
            return RedirectToAction("Index", "Profil");
        }

        // GET: Recenzije/Index - prikaz svih recenzija
        [HttpGet]
        public IActionResult Index()
        {
            var recenzije = _context.Recenzije
                .Where(r => r.OcjenaKlinika > 0)
                .Include(r => r.Korisnik)
                .Include(r => r.Doktor)
                .ToList();

            var prosjekKlinike = recenzije.Any() ? recenzije.Average(r => r.OcjenaKlinika) : 0;

            ViewBag.ProsjekKlinike = prosjekKlinike;

            return View(recenzije);
        }
    }
}
