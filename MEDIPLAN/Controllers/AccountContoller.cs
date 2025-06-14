using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using QRCoder;
using System.IO;
using System.Threading.Tasks;
using MEDIPLAN.Data;
using MEDIPLAN.Models;


public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Ispisi sve validacijske greške u konzolu
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"VALIDACIJSKA GREŠKA: {error.ErrorMessage}");
            }
            return View(model);
        }

        try
        {
            // Provjeri da li korisničko ime već postoji
            if (await _context.Korisnici.AnyAsync(x => x.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Korisničko ime već postoji");
                return View(model);
            }

            // Generisanje QR koda
            string qrBase64;
            try
            {
                qrBase64 = GenerisiQrKod(model.Username);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Greška pri generisanju QR koda: {ex.Message}");
                return View(model);
            }

            // Kreiranje novog korisnika
            var korisnik = new Korisnici
            {
                Username = model.Username,
                Lozinka = HashLozinka(model.Lozinka),
                QrKod = qrBase64,
                Email = model.Email,
                Ime = model.Ime,
                Prezime = model.Prezime,
                DatumRodjenja = model.DatumRodjenja,
                Uloga = (int)Uloga.Pacijent,
                Odjel = 0 // obavezno ako nije nullable u bazi!
            };

            _context.Korisnici.Add(korisnik);
            await _context.SaveChangesAsync();

            // Postavi sesiju i preusmjeri na Home
            HttpContext.Session.SetString("KorisniciId", korisnik.Id.ToString());
            return RedirectToAction("Index", "Home");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Database error: {ex.InnerException?.Message ?? ex.Message}");
            ModelState.AddModelError("", "Došlo je do greške pri čuvanju podataka");
            return View(model);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Neočekivana greška: {ex.Message}");
            ModelState.AddModelError("", "Došlo je do neočekivane greške");
            return View(model);
        }
    }



    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

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
                HttpContext.Session.SetString("KorisniciId", Korisnici.Id.ToString());
                HttpContext.Session.SetString("Username", Korisnici.Username);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Pogrešno korisničko ime ili lozinka.");
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    // Heširanje lozinke
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

    // Generisanje QR koda kao Base64 string koristeći SkiaSharp

    private string GenerisiQrKod(string tekst)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tekst))
                throw new ArgumentException("Tekst ne sme biti prazan");

            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(tekst, QRCodeGenerator.ECCLevel.Q, forceUtf8: true))
            using (var qrCode = new PngByteQRCode(qrCodeData))
            {
                byte[] pngBytes = qrCode.GetGraphic(20);
                string base64 = Convert.ToBase64String(pngBytes);

                return base64;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Greška pri generisanju QR koda: {ex}");
            throw;
        }
    }

}
