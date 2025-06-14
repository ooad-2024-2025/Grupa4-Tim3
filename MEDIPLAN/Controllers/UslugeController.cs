using MEDIPLAN.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using MEDIPLAN.Models;
using MEDIPLAN.ViewModels; 



namespace MEDIPLAN.Controllers
{
    public class UslugeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UslugeController(ApplicationDbContext context)
        {
            _context = context;
        }
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
                    SlikaUrl = Url.Content($"~/images/usluge/{u}.jpg")
                })
                .ToList();

            return View(usluge);
        }


        public IActionResult Detalji(int id)
        {
            var usluga = _context.Usluge.FirstOrDefault(u => u.Id == id);
            if (usluga == null)
                return NotFound();

            // Funkcija koja mapira Odjel iz baze na enum offset (bez NemaOdjela)
            int OdjelIdBezOffseta(int odjel)
            {
                return odjel; // odjel iz baze je 0-based, dakle 0=InternaMedicina
            }

            int odjelId = OdjelIdBezOffseta(usluga.Odjel);

            var doktori = _context.Korisnici
                .Where(k => k.Uloga == (int)Uloga.Doktor && k.MedicinskaUslugaId == id)
                .ToList();

            var model = new DetaljiUslugeViewModel
            {
                Usluge = usluga,
                Doktori = doktori
            };

            return View(model);
        }


    }
}
