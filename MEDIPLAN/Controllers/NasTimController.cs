using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            // Uzmi doktore koji imaju ulogu Doktor
            var doktori = _context.Korisnici
                .Where(k => k.Uloga == (int)Uloga.Doktor)
                .ToList();

            // Grupisanje doktora po enumu Odjel
            var doktoriPoOdjelima = doktori
                .GroupBy(d => (Odjel)d.Odjel)  // cast int to enum
                .ToDictionary(g => g.Key, g => g.ToList());

            return View(doktoriPoOdjelima);
        }
    }
}
