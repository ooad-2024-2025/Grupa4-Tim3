using Microsoft.AspNetCore.Mvc;
using MediPlan.Models;
using System.Collections.Generic;
using System.Linq;

namespace MediPlan.Controllers
{
    public class HomeController : Controller
    {
        private static readonly List<Usluga> _usluge = new List<Usluga>
        {
            new Usluga { Naziv = "Interna medicina", Opis = "Opis za internu medicinu...", Ikona = "ikone/internal-medicine.png", Vrsta = VrstaMedicinskihUsluga.InternaMedicina },
            new Usluga { Naziv = "Kardiologija", Opis = "Opis za kardiologiju...", Ikona = "ikone/cardiology.png", Vrsta = VrstaMedicinskihUsluga.Kardiologija },
            new Usluga { Naziv = "Oftamologija", Opis = "Opis za oftamologiju...", Ikona = "ikone/ophthalmology.png", Vrsta = VrstaMedicinskihUsluga.Oftamologija },
            new Usluga { Naziv = "Dermatologija", Opis = "Opis za dermatologiju...", Ikona = "ikone/dermatology.png", Vrsta = VrstaMedicinskihUsluga.Dermatologija },
            new Usluga { Naziv = "Endokrinologija", Opis = "Opis za endokrinologiju...", Ikona = "ikone/endocrinology.png", Vrsta = VrstaMedicinskihUsluga.Endokrinologija },
            new Usluga { Naziv = "Ginekologija", Opis = "Opis za ginekologiju...", Ikona = "images/uterus-icon.svg", Vrsta = VrstaMedicinskihUsluga.Ginekologija },
            new Usluga { Naziv = "Neurologija", Opis = "Opis za neurologiju...", Ikona = "ikone/neurology.png", Vrsta = VrstaMedicinskihUsluga.Neurologija },
            new Usluga { Naziv = "Radiologija", Opis = "Opis za radiologiju...", Ikona = "ikone/radiology.png", Vrsta = VrstaMedicinskihUsluga.Radiologija }
        };

        public IActionResult Index() => View();

        public IActionResult About() => View();

        public IActionResult Usluge()
        {
            return View(_usluge);
        }

        public IActionResult DetaljiUsluge(string naziv)
        {
            if (string.IsNullOrEmpty(naziv))
                return RedirectToAction("Usluge");

            var usluga = _usluge.FirstOrDefault(u => u.Naziv == naziv);
            if (usluga == null)
                return NotFound();

            return View(usluga);
        }

        public IActionResult Tim() => View();

        public IActionResult Kontakt() => View();

        public IActionResult Zakazi() => View();

        public IActionResult Privacy() => View();
    }
}
