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
            // Učitaj doktore zajedno sa njihovom MedicinskaUsluga (Include)
            var doktori = _context.Korisnici
                .Include(k => k.MedicinskaUsluga) // učitaj uslugu
                .Where(k => k.Uloga == (int)Uloga.Doktor)
                .ToList();

            // Grupisanje doktora po nazivu medicinske usluge
            var doktoriPoUslugama = doktori
                .Where(d => d.MedicinskaUsluga != null)
                .GroupBy(d => d.MedicinskaUsluga!.Napomena) // ili neki drugi property usluge
                .ToDictionary(g => g.Key, g => g.ToList());

            return View(doktoriPoUslugama);
        }

    }
}
