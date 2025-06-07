using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace MediPlan.Controllers
{
    public class TerminController : Controller
    {
        // Pomoćna klasa za podatke s validacijom
        public class TerminModel
        {
            [Required(ErrorMessage = "Morate odabrati doktora.")]
            public string Doktor { get; set; }

            [Required(ErrorMessage = "Morate odabrati datum.")]
            public DateTime? Datum { get; set; }
        }

        // GET metoda za prikaz forme
        [HttpGet]
        public IActionResult Zakazi()
        {
            return View();
        }

        // POST metoda za obradu forme
        [HttpPost]
        public IActionResult Zakazi(TerminModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Ovdje bi išla logika za spremanje u bazu podataka
            // npr. dbContext.Termini.Add(new Termin { Doktor = model.Doktor, Datum = model.Datum.Value });

            TempData["Poruka"] = $"Uspješno ste zakazali termin kod doktora {model.Doktor} za {model.Datum:dd.MM.yyyy}.";
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
