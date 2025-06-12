using Microsoft.AspNetCore.Mvc;
using MEDIPLAN.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using MEDIPLAN.Models;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Account/Register
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // POST: /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Provera da li već postoji
            var postoji = await _context.Korisnici
                .FirstOrDefaultAsync(x => x.Username == model.Username);
            if (postoji != null)
            {
                ModelState.AddModelError("", "Korisničko ime već postoji!");
                return View(model);
            }

            // Heširaj lozinku (preporuka!)
            var hashedLozinka = HashLozinka(model.Lozinka);

            var Korisnik = new Korisnici 
            {
                Username = model.Username,
                Ime = model.Ime,
                Prezime = model.Prezime,
                Email = model.Email,
                Lozinka = hashedLozinka,
                DatumRodjenja = model.DatumRodjenja,
                Uloga = (int)Uloga.Pacijent, // ili šta god ti treba kao default
                QrKod = "" // Postavi praznu vrednost ili generiši neki kod ako treba
            };

            _context.Korisnici.Add(Korisnik);
            await _context.SaveChangesAsync();

            // Nakon registracije - možeš ga automatski logovati
            HttpContext.Session.SetString("KorisniciId", Korisnik.Id.ToString());
            HttpContext.Session.SetString("Username", Korisnik.Username);

            return RedirectToAction("Index", "Home");
        }

        return View(model);
    }

    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var hashedLozinka = HashLozinka(model.Lozinka);

            var Korisnici = await _context.Korisnici
                .FirstOrDefaultAsync(x => x.Username == model.Username && x.Lozinka == hashedLozinka);

            if (Korisnici != null)
            {
                // Prijava - koristi Session
                HttpContext.Session.SetString("KorisniciId", Korisnici.Id.ToString());
                HttpContext.Session.SetString("Username", Korisnici.Username);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Pogrešno korisničko ime ili lozinka.");
        }

        return View(model);
    }

    // GET: /Account/Logout
    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    // Heširanje lozinke (MD5 primer, možeš koristiti i SHA256)
    private string HashLozinka(string lozinka)
    {
        using (var md5 = MD5.Create())
        {
            var inputBytes = Encoding.ASCII.GetBytes(lozinka);
            var hashBytes = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            foreach (var b in hashBytes)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }

}
