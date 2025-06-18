using MEDIPLAN.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using MEDIPLAN.Models;

namespace MEDIPLAN.Controllers
{
    public class DoktorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoktorController(ApplicationDbContext context)
        {
            _context = context;
        }




        public IActionResult Dashboard()
        {
            return View();
        }


        public IActionResult Termini()
        {
            int? korisnikId = HttpContext.Session.GetInt32("KorisniciId");
            string? uloga = HttpContext.Session.GetString("Uloga");

            if (korisnikId == null || uloga != ((int)Uloga.Doktor).ToString())
                return RedirectToAction("Login", "Account");

            var termini = _context.Termini
                .Include(t => t.Pacijent)
                .Where(t => t.DoktorId == korisnikId)
                .OrderBy(t => t.DatumVrijemePocetak)
                .ToList();

            var usluge = _context.Usluge.ToList();

            var viewModel = termini.Select(t => new TerminPrikazViewModel
            {
                PacijentImePrezime = $"{t.Pacijent?.Ime} {t.Pacijent?.Prezime}",
                Datum = t.DatumVrijemePocetak
            }).ToList();

            return View(viewModel);
        }




        public async Task<IActionResult> Recenzije()
        {
            int? doktorId = HttpContext.Session.GetInt32("KorisniciId");
            string? uloga = HttpContext.Session.GetString("Uloga");

            if (doktorId == null || uloga != ((int)Uloga.Doktor).ToString())
            {
                TempData["Greska"] = "Morate biti prijavljeni kao doktor.";
                return RedirectToAction("Login", "Account");
            }

            var recenzije = await _context.Recenzije
                .Include(r => r.Korisnik) // pacijent koji je ostavio recenziju
                .Include(r => r.Termin)
                .Where(r => r.DoktorId == doktorId)
                .OrderByDescending(r => r.Datum)
                .ToListAsync();

            return View(recenzije);
        }


        public IActionResult FormaNalaza()
        {
            return View(); // za sada prazan
        }

        [HttpGet]
        public IActionResult PreuzmiNalaz(string naziv)
        {
            if (string.IsNullOrEmpty(naziv))
                return NotFound();

            var putanja = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "nalazi", naziv);

            if (!System.IO.File.Exists(putanja))
                return NotFound();

            var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; // .docx MIME tip
            return PhysicalFile(putanja, contentType, naziv);
        }

        public IActionResult Notifikacije()
        {
            int? doktorId = HttpContext.Session.GetInt32("KorisniciId");
            string? uloga = HttpContext.Session.GetString("Uloga");

            if (doktorId == null || uloga != ((int)Uloga.Doktor).ToString())
                return RedirectToAction("Login", "Account");

            return View(); // Prazan view za sada
        }

    }
}