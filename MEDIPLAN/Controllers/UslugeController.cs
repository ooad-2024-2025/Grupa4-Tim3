using MEDIPLAN.Models; // Ako imaš poseban model za uslugu, ili možeš napraviti ViewModel
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MEDIPLAN.Controllers
{
    public class UslugeController : Controller
    {
        // Enum sa uslugama
        public enum VrstaMedicinskihUsluge
        {
            InternaMedicina,
            Kardiologija,
            Oftamologija,
            Dermatologija,
            Endokrinologija,
            Ginekologija,
            Neurologija,
            Radiologija
        }

        public class UslugaViewModel
        {
            public string Naziv { get; set; }
            public string SlikaUrl { get; set; }
        }

        public IActionResult Index()
        {
            var usluge = Enum.GetNames(typeof(VrstaMedicinskihUsluge))
                .Select(u => new UslugaViewModel
                {
                    Naziv = u,
                    SlikaUrl = Url.Content($"~/images/usluge/{u}.jpg")  // Pretpostavka da slike imaš u wwwroot/images/usluge/
                })
                .ToList();

            return View(usluge);
        }

        public IActionResult Detalji(string naziv)
        {
            if (string.IsNullOrEmpty(naziv))
                return NotFound();

            // Ovde možeš učitati detalje iz baze ili samo prikazati info na osnovu naziva
            var usluga = Enum.GetNames(typeof(VrstaMedicinskihUsluge))
                .FirstOrDefault(u => u.Equals(naziv, StringComparison.OrdinalIgnoreCase));

            if (usluga == null)
                return NotFound();

            return View(model: usluga);
        }
    }
}
