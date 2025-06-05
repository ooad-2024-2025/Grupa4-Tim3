using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace MediPlan.Controllers
{
    public class TerminController : Controller
    {
        // Pomoćna klasa za podatke
        public class TerminModel
        {
            public string Doktor { get; set; }
            public DateTime Datum { get; set; }
        }

        // GET metoda za prikaz forme
        [HttpGet]
        public IActionResult Zakazi()
        {
            return View();
        }

        // POST metoda za obradu forme
        [HttpPost]
        public IActionResult Zakazi(string doktor, DateTime datum)
        {
            if (string.IsNullOrEmpty(doktor) || datum == default)
            {
                ModelState.AddModelError("", "Svi podaci su obavezni.");
                return View();
            }

            // Ovdje bi išla logika za spremanje u bazu podataka
            // npr. dbContext.Termini.Add(new Termin { Doktor = doktor, Datum = datum });

            TempData["Poruka"] = $"Uspješno ste zakazali termin kod doktora {doktor} za {datum:dd.MM.yyyy}.";
            return RedirectToAction("Potvrda");
        }

        // GET metoda za potvrdu
        public IActionResult Potvrda()
        {
            ViewBag.Poruka = TempData["Poruka"];
            return View();
        }
    }
}
