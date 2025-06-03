using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediPlan.Data; // OVO PROMIJENI ako ti se projekat drugačije zove
using MediPlan.Models;


public class TerminController : Controller
{
    private readonly ApplicationDbContext _context;

    public TerminController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Prikaz svih termina
    public async Task<IActionResult> Index()
    {
        var termini = await _context.Termini.ToListAsync();
        return View(termini);
    }

    // Forma za zakazivanje termina
    public IActionResult Zakazi()
    {
        return View();
    }

    // Obrada forme kad korisnik klikne "Zakaži"
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Zakazi(Termin termin)
    {
        if (ModelState.IsValid)
        {
            _context.Add(termin);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(termin);
    }
}
