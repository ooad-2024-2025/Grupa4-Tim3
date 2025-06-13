using Microsoft.AspNetCore.Mvc;
using MEDIPLAN.Models;
using System.Collections.Generic;
using System.Linq;
using MEDIPLAN.Data;

namespace MEDIPLAN.Controllers
{
	public class HomeController : Controller
	{
		private readonly ApplicationDbContext _context;

		// Konstruktor za Dependency Injection DbContext-a
		public HomeController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Index() => View();

		public IActionResult About() => View();

		public IActionResult Usluge()
		{
			// Hardkodirana lista usluga
			var usluge = new List<Usluge>
			{
				new Usluge { Naziv = "Interna Medicina", Ikona = "images/interna-medicina.png" },
				new Usluge { Naziv = "Kardiologija", Ikona = "images/kardiologija.png" },
				new Usluge { Naziv = "Oftamologija", Ikona = "images/oftamologija.png" },
				new Usluge { Naziv = "Dermatologija", Ikona = "images/dermatologija.png" },
				new Usluge { Naziv = "Endokrinologija", Ikona = "images/endokrinologija.png" },
				new Usluge { Naziv = "Ginekologija", Ikona = "images/gynecology.png" },
				new Usluge { Naziv = "Neurologija", Ikona = "images/neurologija.png" },
				new Usluge { Naziv = "Radiologija", Ikona = "images/radiology.png" }
			};

			return View(usluge);
		}

		public IActionResult DetaljiUsluge(string naziv)
		{
			if (string.IsNullOrEmpty(naziv))
				return RedirectToAction("Usluge");

			var usluga = _context.Usluge.FirstOrDefault(u => u.Naziv == naziv);
			if (usluga == null)
				return NotFound();

			var doktoriZaUslugu = _context.Korisnici
				.Where(k => k.Uloga == (int)Uloga.Doktor && k.Odjel == usluga.Odjel)
				.ToList();

			var model = new DetaljiUslugeViewModel
			{
				Usluge = usluga,
				Doktori = doktoriZaUslugu
			};

			return View(model);
		}



       

            public IActionResult Statistika()
            {
                // Ovdje možeš pripremiti model sa podacima za statistiku ako treba
                return View();
            }
        

        public IActionResult Kontakt()
		{
			var lokacije = new List<LokacijaInfo>
			{
				new LokacijaInfo
				{
					Grad = "Sarajevo",
					Adresa = "Zmaja od Bosne 12",
					Telefon = "+387 33 123 456",
					Email = "info@MEDIPLAN.ba",
					Slika = "images/grbavica.jpg"
				},
				new LokacijaInfo
				{
					Grad = "Ilidža",
					Adresa = "Butmirska Cesta 9",
					Telefon = "+387 36 654 321",
					Email = "info@MEDIPLAN.ba",
					Slika = "images/butmir.jpg"
				}
			};

			return View(lokacije);
		}

		public IActionResult MojProfil()
		{
			return View();
		}

		public IActionResult Cjenovnik()
		{
			return View();
		}

		public IActionResult ZakaziTermini()
		{
			return View();
		}

		public IActionResult Privacy() => View();
	}
}
