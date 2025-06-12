using Microsoft.AspNetCore.Mvc;
using MEDIPLAN.Data;
using MEDIPLAN.Models;
using System.Linq;
using System.Collections.Generic;

namespace MEDIPLAN.Controllers
{
    public class NasTimController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NasTimController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Tim()
        {
            // Uzmi doktore iz baze
            var doktori = _context.Korisnici
                .Where(k => k.Uloga == Uloga.Doktor)
                .ToList();

            // Grupisanje doktora po odjelu u Dictionary<Odjel, List<Korisnik>>
            var doktoriPoOdjelima = doktori
                .GroupBy(d => d.Odjel)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Prosledi grupu doktor po odjelu u View
            return View(doktoriPoOdjelima);
        }
    }
}
