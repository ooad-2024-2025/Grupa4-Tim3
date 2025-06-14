using MEDIPLAN.Data;
using MEDIPLAN.Models;
using MEDIPLAN.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public AccountController(ApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (_context.Korisnici.Any(x => x.Username == model.Username))
        {
            ModelState.AddModelError("Username", "Korisničko ime već postoji");
            return View(model);
        }

        if (_context.Korisnici.Any(x => x.Email == model.Email))
        {
            ModelState.AddModelError("Email", "Email već postoji");
            return View(model);
        }

        var token = Guid.NewGuid().ToString();
        var korisnik = new Korisnici
        {
            Username = model.Username,
            Ime = model.Ime,
            Prezime = model.Prezime,
            Email = model.Email,
            Lozinka = HashLozinka(model.Lozinka),
            DatumRodjenja = model.DatumRodjenja,
            Uloga = (int)Uloga.Pacijent,
            Odjel = 0,
            QrKod = GenerisiQrKod(model.Username),
            VerificationToken = token,
            IsVerified = false
        };

        _context.Korisnici.Add(korisnik);
        await _context.SaveChangesAsync();

        await PosaljiVerifikaciju(korisnik.Email, token);

        TempData["SuccessMessage"] = "Uspješna registracija! Provjerite email za verifikaciju.";
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var lozinkaHash = HashLozinka(model.Lozinka);
        var korisnik = await _context.Korisnici
            .FirstOrDefaultAsync(x => x.Username == model.Username && x.Lozinka == lozinkaHash);

        if (korisnik == null || (korisnik.Uloga != (int)Uloga.Doktor && !korisnik.IsVerified))
        {
            ModelState.AddModelError("", "Neispravni podaci ili nije verifikovan nalog.");
            return View(model);
        }

        // Postavi sesiju s int za Id i string za ostale
        HttpContext.Session.SetInt32("KorisniciId", korisnik.Id);
        HttpContext.Session.SetString("Username", korisnik.Username);
        HttpContext.Session.SetString("Uloga", korisnik.Uloga.ToString());

        // Ako je doktor, preusmjeri ga na Doktor/Dashboard
        if (korisnik.Uloga == (int)Uloga.Doktor)
        {
            return RedirectToAction("Dashboard", "Doktor");
        }

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> VerifyEmail(string email, string token)
    {
        var korisnik = await _context.Korisnici.FirstOrDefaultAsync(x => x.Email == email);

        if (korisnik == null || korisnik.VerificationToken != token)
            return View("VerificationError");

        korisnik.IsVerified = true;
        korisnik.VerificationToken = null;

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Email verifikovan. Možete se prijaviti.";
        return RedirectToAction("Login");
    }

    private async Task PosaljiVerifikaciju(string email, string token)
    {
        var link = Url.Action("VerifyEmail", "Account", new { email, token }, Request.Scheme);
        var body = $"Kliknite ovdje za verifikaciju naloga: <a href='{link}'>Verifikuj Email</a>";

        await _emailService.SendEmailAsync(email, "Verifikacija emaila", body);
    }

    private string HashLozinka(string lozinka)
    {
        using var md5 = MD5.Create();
        var inputBytes = Encoding.ASCII.GetBytes(lozinka);
        var hashBytes = md5.ComputeHash(inputBytes);
        return string.Concat(hashBytes.Select(b => b.ToString("X2")));
    }

    private string GenerisiQrKod(string tekst)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(tekst, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        return Convert.ToBase64String(qrCode.GetGraphic(20));
    }
}
