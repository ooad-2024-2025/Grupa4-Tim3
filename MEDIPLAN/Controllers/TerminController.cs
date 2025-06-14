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
        public async Task<IActionResult> Zakazi(int? id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("KorisniciId")))
            {
                TempData["Greska"] = "Morate biti prijavljeni da biste zakazali termin.";
                return RedirectToAction("Login", "Account");
            }

            var pacijentId = int.Parse(HttpContext.Session.GetString("KorisniciId"));
            await PopuniViewBagove();

            var model = new TerminModel();

            if (id.HasValue)
            {
                var termin = await _context.Termini
                    .FirstOrDefaultAsync(t => t.Id == id && t.PacijentId == pacijentId);

                if (termin == null)
                {
                    TempData["Greska"] = "Termin nije pronađen ili nemate pravo pristupa.";
                    return RedirectToAction("Index", "Profil");
                }

                // ➕ Provjera 24h ograničenja
                if ((termin.DatumVrijemePocetak - DateTime.Now).TotalHours < 24)
                {
                    TempData["Greska"] = "Termin se ne može mijenjati unutar 24 sata.";
                    return RedirectToAction("Index", "Profil");
                }

                model.Id = termin.Id;
                model.DoktorId = termin.DoktorId;
                model.Datum = termin.DatumVrijemePocetak;
                model.Lokacija = termin.Lokacija;
                model.MedicinskeUslugeId = termin.MedicinskeUslugeId;

                TempData["IzmjenaPoruka"] = "Napomena: U pitanju je izmjena termina. Originalni termin će biti uklonjen.";
            }

            return View(model);
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

            var pacijentId = int.Parse(HttpContext.Session.GetString("KorisniciId"));

            if (!ModelState.IsValid)
            {
                await PopuniViewBagove();
                return View(model);
            }

            var pocetakTermina = model.Datum.Value;
            var krajTermina = pocetakTermina.AddHours(1);

            // ➕ Ako se radi o izmjeni, provjeri da li je više od 24h
            if (model.Id > 0)
            {
                var stariTermin = await _context.Termini
                    .FirstOrDefaultAsync(t => t.Id == model.Id && t.PacijentId == pacijentId);

                if (stariTermin == null)
                {
                    TempData["Greska"] = "Termin nije pronađen.";
                    return RedirectToAction("Index", "Profil");
                }

                if ((stariTermin.DatumVrijemePocetak - DateTime.Now).TotalHours < 24)
                {
                    TempData["Greska"] = "Termin se ne može mijenjati unutar 24 sata.";
                    return RedirectToAction("Index", "Profil");
                }
            }

            // Provjera zauzetosti termina
            bool terminZauzet = await _context.Termini
                .AnyAsync(t => t.DoktorId == model.DoktorId &&
                              t.Id != model.Id &&
                              ((pocetakTermina >= t.DatumVrijemePocetak && pocetakTermina < t.DatumVrijemeKraj) ||
                               (krajTermina > t.DatumVrijemePocetak && krajTermina <= t.DatumVrijemeKraj) ||
                               (pocetakTermina <= t.DatumVrijemePocetak && krajTermina >= t.DatumVrijemeKraj)));

            if (terminZauzet)
            {
                ModelState.AddModelError(string.Empty, "Termin kod odabranog doktora u to vrijeme je zauzet.");
                await PopuniViewBagove();
                return View(model);
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (model.Id > 0)
                    {
                        var stariTermin = await _context.Termini
                            .FirstOrDefaultAsync(t => t.Id == model.Id && t.PacijentId == pacijentId);

                        _context.Termini.Remove(stariTermin);
                        await _context.SaveChangesAsync();
                    }

                    var noviTermin = new Termini
                    {
                        DoktorId = model.DoktorId,
                        PacijentId = pacijentId,
                        DatumVrijemePocetak = pocetakTermina,
                        DatumVrijemeKraj = krajTermina,
                        Lokacija = model.Lokacija,
                        MedicinskeUslugeId = model.MedicinskeUslugeId
                    };

                    _context.Termini.Add(noviTermin);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Poruka"] = model.Id > 0 ? "Termin je uspješno izmijenjen." : "Termin je uspješno zakazan.";
                    return RedirectToAction("Potvrda");
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    TempData["Greska"] = "Došlo je do greške prilikom obrade zahtjeva.";
                    return RedirectToAction("Index", "Profil");
                }
            }
        }

        [HttpGet]
        public IActionResult Potvrda()
        {
            return View();
        }

        private async Task PopuniViewBagove()
        {
            var doktoriLista = await _context.Korisnici
                .Where(k => k.Uloga == (int)Uloga.Doktor)
                .ToListAsync();

            var doktoriSaOdjelom = doktoriLista
                .Select(k => new
                {
                    k.Id,
                    ImePrezime = $"{k.Ime} {k.Prezime} ({Enum.GetName(typeof(Odjel), k.Odjel) ?? "Nepoznat odjel"})"
                })
                .ToList();

            ViewBag.Doktori = new SelectList(doktoriSaOdjelom, "Id", "ImePrezime");

            ViewBag.Lokacije = new SelectList(
                Enum.GetValues(typeof(Lokacija))
                    .Cast<Lokacija>()
                    .Select(l => new { Value = (int)l, Text = l.ToString() }),
                "Value", "Text");

            ViewBag.MedicinskeUsluge = new SelectList(
                await _context.Usluge.ToListAsync(),
                "Id", "Naziv");
        }
    }
}