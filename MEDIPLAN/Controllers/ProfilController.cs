// ProfilController.cs
using Microsoft.AspNetCore.Mvc;
using MEDIPLAN.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using MEDIPLAN.Models;

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
            var korisnikIdString = HttpContext.Session.GetString("KorisnikId");
            if (string.IsNullOrEmpty(korisnikIdString) || !int.TryParse(korisnikIdString, out int korisnikId))
            {
                return RedirectToAction("Login", "Account");
            }

            var korisnik = await _context.Korisnici.FindAsync(korisnikId);
            if (korisnik == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var sviTermini = await _context.Termini
                .Where(t => t.PacijentId == korisnikId)
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
                    TerminId = t.Id,
                    ImeDoktora = doktori.ContainsKey(t.DoktorId)
                        ? doktori[t.DoktorId].Ime + " " + doktori[t.DoktorId].Prezime
                        : "Nepoznat",
                    DatumPregleda = t.DatumVrijemePocetak
                })
                .ToList();

            var zavrseniTermini = sviTermini
                .Where(t => t.DatumVrijemePocetak <= danas)
                .Select(t => new TerminViewModel
                {
                    TerminId = t.Id,
                    ImeDoktora = doktori.ContainsKey(t.DoktorId)
                        ? doktori[t.DoktorId].Ime + " " + doktori[t.DoktorId].Prezime
                        : "Nepoznat",
                    DatumPregleda = t.DatumVrijemePocetak
                })
                .ToList();

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
        public async Task<IActionResult> OtkaziTermin(int terminId)
        {
            var termin = await _context.Termini.FindAsync(terminId);
            if (termin != null)
            {
                _context.Termini.Remove(termin);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult IzmijeniTermin(int terminId)
        {
            // Preusmjeri korisnika na "Zakazite" u Termin kontroleru
            return RedirectToAction("Zakazite", "Termin");
        }

        [HttpPost]
        public async Task<IActionResult> IzmijeniTermin(TerminEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var termin = await _context.Termini.FindAsync(model.Id);
                if (termin == null)
                    return NotFound();

                termin.DatumVrijemePocetak = model.DatumVrijemePocetak;
                termin.DatumVrijemeKraj = model.DatumVrijemeKraj;

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}