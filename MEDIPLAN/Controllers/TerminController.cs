using MEDIPLAN.Data;
using MEDIPLAN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;  // za Session
using System.Linq;
using System.Threading.Tasks;

namespace MEDIPLAN.Controllers
{
    public class TerminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TerminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Zakazi()
        {
            // Provjera je li korisnik prijavljen u sesiji
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("KorisniciId")))
            {
                return RedirectToAction("Login", "Account");
            }

            var doktori = await _context.Korisnici
                .Where(k => k.Uloga == (int)Uloga.Doktor)
                .Select(k => new { k.Id, ImePrezime = k.Ime + " " + k.Prezime + " (" + k.Odjel.ToString() + ")" })
                .ToListAsync();

            ViewBag.Doktori = new SelectList(doktori, "Id", "ImePrezime");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Zakazi(TerminModel model)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("KorisniciId")))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                var doktori = await _context.Korisnici
                    .Where(k => k.Uloga == (int)Uloga.Doktor)
                    .Select(k => new { k.Id, ImePrezime = k.Ime + " " + k.Prezime + " (" + k.Odjel.ToString() + ")" })
                    .ToListAsync();

                ViewBag.Doktori = new SelectList(doktori, "Id", "ImePrezime");

                return View(model);
            }

            var pacijentIdString = HttpContext.Session.GetString("KorisniciId");
            if (!int.TryParse(pacijentIdString, out int pacijentId))
            {
                return RedirectToAction("Login", "Account");
            }

            var termini = new Termini
            {
                DoktorId = model.DoktorId,
                PacijentId = pacijentId,
                DatumVrijemePocetak = model.Datum.Value,
                DatumVrijemeKraj = model.Datum.Value.AddHours(1)
            };

            _context.Termini.Add(termini);
            await _context.SaveChangesAsync();

            TempData["Poruka"] = "Uspješno ste zakazali termin.";
            return RedirectToAction("Potvrda");
        }
    }
}
