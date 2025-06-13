using Microsoft.AspNetCore.Mvc;
using MEDIPLAN.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using MEDIPLAN.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

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
                .ToListAsync();

            var doktorIds = sviTermini.Select(t => t.DoktorId).Distinct().ToList();

            var doktori = await _context.Korisnici
                .Where(k => doktorIds.Contains(k.Id))
                .ToDictionaryAsync(k => k.Id);

            var danas = DateTime.Now;

            var zakazaniTermini = sviTermini
                .Where(t => t.DatumVrijemePocetak > danas)
                .Select(t => new TerminViewModel
                {
                    Terminid = t.Id,
                    ImeDoktora = doktori.ContainsKey(t.DoktorId)
                        ? doktori[t.DoktorId].Ime + " " + doktori[t.DoktorId].Prezime
                        : "Nepoznat",
                    DatumPregleda = t.DatumVrijemePocetak
                }).ToList();

            var zavrseniTermini = sviTermini
                .Where(t => t.DatumVrijemePocetak <= danas)
                .Select(t => new TerminViewModel
                {
                    Terminid = t.Id,
                    ImeDoktora = doktori.ContainsKey(t.DoktorId)
                        ? doktori[t.DoktorId].Ime + " " + doktori[t.DoktorId].Prezime
                        : "Nepoznat",
                    DatumPregleda = t.DatumVrijemePocetak
                }).ToList();

            var model = new ProfilViewModel
            {
                Ime = korisnik.Ime,
                Prezime = korisnik.Prezime,
                DatumRodjenja = korisnik.DatumRodjenja,
                ZakazaniTermini = zakazaniTermini,
                ZavrseniTermini = zavrseniTermini
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> OtkaziTermini(int Terminid)
        {
            var termin = await _context.Termini.FindAsync(Terminid);
            if (termin != null)
            {
                var razlika = termin.DatumVrijemePocetak - DateTime.Now;

                if (razlika.TotalHours < 24)
                {
                    TempData["Greska"] = "Termin se može otkazati samo ako je udaljen više od 24 sata.";
                    return RedirectToAction("Index");
                }

                _context.Termini.Remove(termin);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> IzmijeniTermine(int Terminid)
        {
            var termin = await _context.Termini.FindAsync(Terminid);
            if (termin == null)
            {
                TempData["Greska"] = "Termin nije pronađen.";
                return RedirectToAction("Index");
            }

            if ((termin.DatumVrijemePocetak - DateTime.Now).TotalHours < 24)
            {
                TempData["Greska"] = "Termin se može mijenjati samo ako je udaljen više od 24 sata.";
                return RedirectToAction("Index");
            }

            ViewBag.Doktori = new SelectList(
                await _context.Korisnici
                    .Where(k => k.Uloga == (int)Uloga.Doktor)
                    .Select(k => new {
                        k.Id,
                        ImePrezime = k.Ime + " " + k.Prezime + " (" + k.Odjel.ToString() + ")"
                    }).ToListAsync(),
                "Id", "ImePrezime");

            ViewBag.Lokacije = new SelectList(
                Enum.GetValues(typeof(Lokacija)).Cast<Lokacija>().Select(l => new {
                    Value = (int)l,
                    Text = l.ToString()
                }),
                "Value", "Text");

            ViewBag.MedicinskeUsluge = new SelectList(
                await _context.Usluge.Select(u => new { u.Id, u.Naziv }).ToListAsync(),
                "Id", "Naziv");

            var model = new TerminiEditViewModel
            {
                Id = termin.Id,
                DoktorId = termin.DoktorId,
                Lokacija = (Lokacija)termin.Lokacija,
                MedicinskeUslugeId = termin.MedicinskeUslugeId,
                DatumVrijemePocetak = termin.DatumVrijemePocetak,
                DatumVrijemeKraj = termin.DatumVrijemeKraj
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> IzmijeniTermine(TerminiEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var stariTermin = await _context.Termini.FindAsync(model.Id);
            if (stariTermin == null)
                return NotFound();

            if ((stariTermin.DatumVrijemePocetak - DateTime.Now).TotalHours < 24)
            {
                TempData["Greska"] = "Termin se može mijenjati samo ako je udaljen više od 24 sata.";
                return RedirectToAction("Index");
            }

            // Provjeri da li je novi termin zauzet za odabranog doktora
            var noviPocetak = model.DatumVrijemePocetak;
            var noviKraj = model.DatumVrijemeKraj;

            bool zauzet = await _context.Termini.AnyAsync(t =>
                t.DoktorId == model.DoktorId &&
                t.Id != model.Id && // izuzmi trenutni (koji ćemo izbrisati)
                (
                    (noviPocetak >= t.DatumVrijemePocetak && noviPocetak < t.DatumVrijemeKraj) ||
                    (noviKraj > t.DatumVrijemePocetak && noviKraj <= t.DatumVrijemeKraj) ||
                    (noviPocetak <= t.DatumVrijemePocetak && noviKraj >= t.DatumVrijemeKraj)
                )
            );

            if (zauzet)
            {
                TempData["Greska"] = "Termin u to vrijeme kod izabranog doktora je zauzet.";
                return View(model);
            }

            int pacijentId = int.Parse(HttpContext.Session.GetString("KorisniciId"));

            // 1. Obriši stari termin
            _context.Termini.Remove(stariTermin);

            // 2. Dodaj novi termin
            var noviTermin = new Termini
            {
                DoktorId = model.DoktorId,
                PacijentId = pacijentId,
                DatumVrijemePocetak = noviPocetak,
                DatumVrijemeKraj = noviKraj,
                Lokacija = (int)model.Lokacija,
                MedicinskeUslugeId = model.MedicinskeUslugeId
            };

            _context.Termini.Add(noviTermin);
            await _context.SaveChangesAsync();

            TempData["Poruka"] = "Termin je uspješno izmijenjen.";
            return RedirectToAction("Index");
        }


        private async Task UcitajViewBagove()
        {
            ViewBag.Doktori = new SelectList(
                await _context.Korisnici
                    .Where(k => k.Uloga == (int)Uloga.Doktor)
                    .Select(k => new {
                        k.Id,
                        ImePrezime = k.Ime + " " + k.Prezime + " (" + k.Odjel.ToString() + ")"
                    }).ToListAsync(),
                "Id", "ImePrezime");

            ViewBag.Lokacije = new SelectList(
                Enum.GetValues(typeof(Lokacija)).Cast<Lokacija>().Select(l => new {
                    Value = (int)l,
                    Text = l.ToString()
                }),
                "Value", "Text");

            ViewBag.MedicinskeUsluge = new SelectList(
                await _context.Usluge.Select(u => new { u.Id, u.Naziv }).ToListAsync(),
                "Id", "Naziv");
        }
    }
}
