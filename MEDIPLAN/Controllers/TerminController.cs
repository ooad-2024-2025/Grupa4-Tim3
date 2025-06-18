using MEDIPLAN.Data;
using MEDIPLAN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MEDIPLAN.Controllers
{
    public class TerminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TerminController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> Zakazi(int? id)
        {
            if (!IsUserLoggedIn())
            {
                TempData["Greska"] = "Morate biti prijavljeni da biste zakazali termin.";
                return RedirectToAction("Login", "Account");
            }

            var pacijentId = HttpContext.Session.GetInt32("KorisniciId");

            if (!pacijentId.HasValue)
            {
                TempData["Greska"] = "Pacijent nije prijavljen.";
                return RedirectToAction("Login", "Account");
            }

            var uloga = HttpContext.Session.GetInt32("Uloga");

            ViewBag.Layout = uloga == (int)Uloga.Doktor
                ? "~/Views/Shared/_LayoutDoktor.cshtml"
                : "~/Views/Shared/_LayoutPacijent.cshtml";

            await PopuniViewBagove();

            var model = new TerminModel();

            if (id.HasValue)
            {
                var termin = await _context.Termini
                    .FirstOrDefaultAsync(t => t.Id == id && t.PacijentId == pacijentId.Value);

                if (termin == null)
                {
                    TempData["Greska"] = "Termin nije pronađen ili nemate pravo pristupa.";
                    return RedirectToAction("Index", "Profil");
                }

                if ((termin.DatumVrijemePocetak - DateTime.Now).TotalHours < 24)
                {
                    TempData["Greska"] = "Termin se ne može mijenjati unutar 24 sata.";
                    return RedirectToAction("Index", "Profil");
                }

                model.Id = termin.Id;
                model.DoktorId = termin.DoktorId;
                model.Datum = termin.DatumVrijemePocetak;
                model.Lokacija = termin.Lokacija;

                TempData["Izmjena"] = "Napomena: U pitanju je izmjena termina. Originalni termin će biti uklonjen.";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Zakazi(TerminModel model)
        {
            if (!IsUserLoggedIn())
            {
                TempData["Greska"] = "Morate biti prijavljeni da biste zakazali termin.";
                return RedirectToAction("Login", "Account");
            }

            var pacijentId = HttpContext.Session.GetInt32("KorisniciId");

            if (!pacijentId.HasValue)
            {
                TempData["Greska"] = "Pacijent nije prijavljen.";
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                await PopuniViewBagove();
                return View(model);
            }

            var pocetakTermina = model.Datum ?? throw new InvalidOperationException("Datum termina nije definisan.");

            if (pocetakTermina.DayOfWeek == DayOfWeek.Sunday)
            {
                ModelState.AddModelError("Datum", "Nedjelja je neradni dan, molimo izaberite drugi datum.");
                await PopuniViewBagove();
                return View(model);
            }

            var krajTermina = pocetakTermina.AddHours(1);

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
                            .FirstOrDefaultAsync(t => t.Id == model.Id && t.PacijentId == pacijentId.Value);

                        if (stariTermin != null)
                        {
                            _context.Termini.Remove(stariTermin);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            TempData["Greska"] = "Termin nije pronađen.";
                            return RedirectToAction("Index", "Profil");
                        }
                    }

                    var noviTermin = new Termini
                    {
                        DoktorId = model.DoktorId,
                        PacijentId = pacijentId.Value,
                        DatumVrijemePocetak = pocetakTermina,
                        DatumVrijemeKraj = krajTermina,
                        Lokacija = model.Lokacija
                    };

                    _context.Termini.Add(noviTermin);
                    await _context.SaveChangesAsync();

                    
                    var doktor = await _context.Korisnici.FindAsync(model.DoktorId);
                    if (doktor != null)
                    {
                        var notifikacija = new Notifikacije
                        {
                            DoktorId = doktor.Id,
                            Poruka = $"Zakazan novi termin sa pacijentom {HttpContext.Session.GetString("Username")} za {pocetakTermina:dd.MM.yyyy. HH:mm} na lokaciji {((Lokacija)model.Lokacija)}"

                        };

                        _context.Notifikacije.Add(notifikacija);
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();

                    TempData["Poruka"] = model.Id > 0 ? "Termin je uspješno izmijenjen." : "Termin je uspješno zakazan.";
                    return RedirectToAction("Potvrda");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["Greska"] = $"Greška: {ex.Message}";
                    return RedirectToAction("Index", "Profil");
                }
            }
        }

        [HttpGet]
        public IActionResult Potvrda()
        {
            if (!IsUserLoggedIn())
            {
                TempData["Greska"] = "Morate biti prijavljeni da biste vidjeli potvrdu.";
                return RedirectToAction("Login", "Account");
            }
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

        private bool IsUserLoggedIn()
        {
            var korisniciId = HttpContext.Session.GetInt32("KorisniciId");
            var username = HttpContext.Session.GetString("Username");

            return korisniciId.HasValue && !string.IsNullOrEmpty(username);
        }
    }
}
