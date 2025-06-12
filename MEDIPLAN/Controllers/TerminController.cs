using MEDIPLAN.Data;
using MEDIPLAN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
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
            var doktori = await _context.Korisnici
                .Where(k => k.Uloga == Uloga.Doktor)
                .Select(k => new { k.Id, ImePrezime = k.Ime + " " + k.Prezime + " (" + k.Odjel.ToString() + ")" })
                .ToListAsync();

            ViewBag.Doktori = new SelectList(doktori, "Id", "ImePrezime");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Zakazi(TerminModel model)
        {
            if (!ModelState.IsValid)
            {
                var doktori = await _context.Korisnici
                    .Where(k => k.Uloga == Uloga.Doktor)
                    .Select(k => new { k.Id, ImePrezime = k.Ime + " " + k.Prezime + " (" + k.Odjel.ToString() + ")" })
                    .ToListAsync();

                ViewBag.Doktori = new SelectList(doktori, "Id", "ImePrezime");

                return View(model);
            }

            var pacijentIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int pacijentId = int.Parse(pacijentIdString);

            var termin = new Termin
            {
                DoktorId = model.DoktorId,
                PacijentId = pacijentId,
                DatumVrijemePocetak = model.Datum.Value,
                DatumVrijemeKraj = model.Datum.Value.AddHours(1)
            };

            _context.Termini.Add(termin);
            await _context.SaveChangesAsync();

            TempData["Poruka"] = $"Uspješno ste zakazali termin.";
            return RedirectToAction("Potvrda");
        }

        public IActionResult Potvrda()
        {
            ViewBag.Poruka = TempData["Poruka"];
            return View();
        }
    }
}
