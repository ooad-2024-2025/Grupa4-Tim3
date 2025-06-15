using MEDIPLAN.Data;
using MEDIPLAN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MEDIPLAN.Controllers
{
    public class RecenzijeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecenzijeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var recenzije = _context.Recenzije
                .Where(r => r.OcjenaKlinika > 0)
                .Include(r => r.Korisnik)
                .ToList();

            var prosjekKlinike = recenzije.Any() ? recenzije.Average(r => r.OcjenaKlinika) : 0;

            ViewBag.ProsjekKlinike = prosjekKlinike;
            return View(recenzije);
        }
    }
}
