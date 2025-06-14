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
            if (korisnikId == null)
                return RedirectToAction("Login", "Account");

            var termini = _context.Termini
                .Include(t => t.Pacijent)
                .Where(t => t.DoktorId == korisnikId)
                .ToList();

            var usluge = _context.Usluge.ToList(); // ručno učitavamo usluge

            // Spajamo usluge u termin
            var viewModel = termini.Select(t => new TerminPrikazViewModel
            {
                PacijentImePrezime = $"{t.Pacijent.Ime} {t.Pacijent.Prezime}",
                Datum = t.DatumVrijemePocetak,
                NazivUsluge = usluge.FirstOrDefault(u => u.Id == t.MedicinskeUslugeId)?.Naziv ?? "Nepoznato"
            }).ToList();

            return View(viewModel);
        }


        public IActionResult Recenzije()
        {
            return View(); // za sada prazan
        }

        public IActionResult FormaNalaza()
        {
            return View(); // za sada prazan
        }


    }
}