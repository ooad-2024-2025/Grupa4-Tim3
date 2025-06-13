using MEDIPLAN.Data;
using MEDIPLAN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using System;

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
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("KorisniciId")))
            {
                TempData["Greska"] = "Morate biti prijavljeni da biste zakazali termin.";
                return RedirectToAction("Login", "Account");
            }

            var doktori = await _context.Korisnici
                .Where(k => k.Uloga == (int)Uloga.Doktor)
                .Select(k => new
                {
                    k.Id,
                    ImePrezime = k.Ime + " " + k.Prezime + " (" + k.Odjel.ToString() + ")"
                }).ToListAsync();

            ViewBag.Doktori = new SelectList(doktori, "Id", "ImePrezime");

            ViewBag.Lokacije = new SelectList(
                Enum.GetValues(typeof(Lokacija))
                    .Cast<Lokacija>()
                    .Select(l => new
                    {
                        Value = (int)l,
                        Text = l.ToString().Replace("Cesta", " Cesta").Replace("Kapetanovica", " Kapetanovića")
                    }),
                "Value",
                "Text"
            );

            var usluge = await _context.Usluge
                .Select(u => new { u.Id, u.Naziv })
                .ToListAsync();

            ViewBag.MedicinskeUsluge = new SelectList(usluge, "Id", "Naziv");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Zakazi(TerminModel model)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("KorisniciId")))
            {
                TempData["Greska"] = "Morate biti prijavljeni da biste zakazali termin.";
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                // Popuni ViewBage kao i ranije
                var doktori = await _context.Korisnici
                    .Where(k => k.Uloga == (int)Uloga.Doktor)
                    .Select(k => new { k.Id, ImePrezime = k.Ime + " " + k.Prezime + " (" + k.Odjel.ToString() + ")" })
                    .ToListAsync();

                ViewBag.Doktori = new SelectList(doktori, "Id", "ImePrezime");

                ViewBag.Lokacije = new SelectList(
                    Enum.GetValues(typeof(Lokacija))
                        .Cast<Lokacija>()
                        .Select(l => new { Value = (int)l, Text = l.ToString() }),
                    "Value",
                    "Text"
                );

                var usluge = await _context.Usluge
                    .Select(u => new { u.Id, u.Naziv })
                    .ToListAsync();

                ViewBag.MedicinskeUsluge = new SelectList(usluge, "Id", "Naziv");

                return View(model);
            }

            int pacijentId = int.Parse(HttpContext.Session.GetString("KorisniciId"));

            var pocetakTermina = model.Datum.Value;
            var krajTermina = pocetakTermina.AddHours(1);

            // Provera zauzetosti termina za istog doktora
            bool terminZauzet = await _context.Termini.AnyAsync(t =>
                t.DoktorId == model.DoktorId &&
                (
                    (pocetakTermina >= t.DatumVrijemePocetak && pocetakTermina < t.DatumVrijemeKraj) ||
                    (krajTermina > t.DatumVrijemePocetak && krajTermina <= t.DatumVrijemeKraj) ||
                    (pocetakTermina <= t.DatumVrijemePocetak && krajTermina >= t.DatumVrijemeKraj)
                )
            );

            if (terminZauzet)
            {
                ModelState.AddModelError(string.Empty, "Termin kod odabranog doktora u to vrijeme je zauzet. Molimo odaberite drugi termin.");

                // Ponovo napuni ViewBage da bi view mogao da se renderuje ispravno
                var doktori = await _context.Korisnici
                    .Where(k => k.Uloga == (int)Uloga.Doktor)
                    .Select(k => new { k.Id, ImePrezime = k.Ime + " " + k.Prezime + " (" + k.Odjel.ToString() + ")" })
                    .ToListAsync();

                ViewBag.Doktori = new SelectList(doktori, "Id", "ImePrezime");

                ViewBag.Lokacije = new SelectList(
                    Enum.GetValues(typeof(Lokacija))
                        .Cast<Lokacija>()
                        .Select(l => new { Value = (int)l, Text = l.ToString() }),
                    "Value",
                    "Text"
                );

                var usluge = await _context.Usluge
                    .Select(u => new { u.Id, u.Naziv })
                    .ToListAsync();

                ViewBag.MedicinskeUsluge = new SelectList(usluge, "Id", "Naziv");

                return View(model);
            }

            var termini = new Termini
            {
                DoktorId = model.DoktorId,
                PacijentId = pacijentId,
                DatumVrijemePocetak = pocetakTermina,
                DatumVrijemeKraj = krajTermina,
                Lokacija = (int)model.Lokacija,
                MedicinskeUslugeId = model.MedicinskeUslugeId
            };

            _context.Termini.Add(termini);
            await _context.SaveChangesAsync();

            TempData["Poruka"] = "Uspješno ste zakazali termin.";
            return RedirectToAction("Potvrda");
        }

        // DODAJ OVU METODU:
        [HttpGet]
        public IActionResult Potvrda()
        {
            return View();
        }
    }
}
