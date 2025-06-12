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
            var KorisniciIdString = HttpContext.Session.GetString("KorisniciId");
            if (string.IsNullOrEmpty(KorisniciIdString) || !int.TryParse(KorisniciIdString, out int KorisniciId))
            {
                return RedirectToAction("Login", "Account");
            }

            var Korisnici = await _context.Korisnici.FindAsync(KorisniciId);
            if (Korisnici == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var sviTermini = await _context.Termini
                .Where(t => t.PacijentId == KorisniciId)
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
                })
                .ToList();

            var zavrseniTermini = sviTermini
                .Where(t => t.DatumVrijemePocetak <= danas)
                .Select(t => new TerminViewModel
                {
                    Terminid = t.Id,
                    ImeDoktora = doktori.ContainsKey(t.DoktorId)
                        ? doktori[t.DoktorId].Ime + " " + doktori[t.DoktorId].Prezime
                        : "Nepoznat",
                    DatumPregleda = t.DatumVrijemePocetak
                })
                .ToList();

            var model = new ProfilViewModel
            {
                Ime = Korisnici.Ime,
                Prezime = Korisnici.Prezime,
                DatumRodjenja = Korisnici.DatumRodjenja,
                ZakazaniTermini = zakazaniTermini,
                ZavrseniTermini = zavrseniTermini
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> OtkaziTermini(int Terminid)
        {
            var Termini = await _context.Termini.FindAsync(Terminid);
            if (Termini != null)
            {
                _context.Termini.Remove(Termini);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult IzmijeniTermini(int Terminid)
        {
            // Preusmjeri Korisnicia na "Zakazite" u Termini kontroleru
            return RedirectToAction("Zakazite", "Termini");
        }

        [HttpPost]
        public async Task<IActionResult> IzmijeniTermini(TerminiEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Termini = await _context.Termini.FindAsync(model.Id);
                if (Termini == null)
                    return NotFound();

                Termini.DatumVrijemePocetak = model.DatumVrijemePocetak;
                Termini.DatumVrijemeKraj = model.DatumVrijemeKraj;

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}