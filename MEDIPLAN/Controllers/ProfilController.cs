using MEDIPLAN.Data;
using MEDIPLAN.Models;
using MEDIPLAN.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MEDIPLAN.Controllers
{
    public class ProfilController : Controller
    {
         
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public ProfilController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsUserLoggedIn())
            {
                TempData["Greska"] = "Morate biti prijavljeni da biste pristupili profilu.";
                return RedirectToAction("Login", "Account");
            }

            var korisniciId = HttpContext.Session.GetInt32("KorisniciId");
            if (!korisniciId.HasValue)
            {
                TempData["Greska"] = "Korisnički podaci nisu pronađeni.";
                return RedirectToAction("Login", "Account");
            }

            var korisnik = await _context.Korisnici.FindAsync(korisniciId.Value);
            if (korisnik == null)
            {
                TempData["Greska"] = "Korisnik nije pronađen.";
                return RedirectToAction("Login", "Account");
            }

            var sviTermini = await _context.Termini
                .Where(t => t.PacijentId == korisniciId.Value)
                .Include(t => t.Doktor)
                .ToListAsync();

            var danas = DateTime.Now;

            var model = new ProfilViewModel
            {
                Ime = korisnik.Ime,
                Prezime = korisnik.Prezime,
                DatumRodjenja = korisnik.DatumRodjenja,
                QrKodBase64 = korisnik.QrKod,
                ZakazaniTermini = sviTermini
                    .Where(t => t.DatumVrijemePocetak > danas)
                    .Select(t => new TerminViewModel
                    {
                        Terminid = t.Id,
                        ImeDoktora = $"{t.Doktor.Ime} {t.Doktor.Prezime}",
                        DatumPregleda = t.DatumVrijemePocetak,
                        JeRecenziran = t.JeRecenziran
                    }).ToList(),
                ZavrseniTermini = sviTermini
                    .Where(t => t.DatumVrijemePocetak <= danas)
                    .Select(t => new TerminViewModel
                    {
                        Terminid = t.Id,
                        ImeDoktora = $"{t.Doktor.Ime} {t.Doktor.Prezime}",
                        DatumPregleda = t.DatumVrijemePocetak,
                        JeRecenziran = t.JeRecenziran
                    }).ToList()
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> OtkaziTermini(int Terminid)
        {
            int? pacijentId = HttpContext.Session.GetInt32("KorisniciId");

            if (pacijentId == null)
            {
                TempData["Greska"] = "Morate biti prijavljeni.";
                return RedirectToAction("Login", "Account");
            }

            var termin = await _context.Termini
                .FirstOrDefaultAsync(t => t.Id == Terminid && t.PacijentId == pacijentId);

            if (termin == null)
            {
                TempData["Greska"] = "Termin nije pronađen.";
                return RedirectToAction("Index", "Profil");
            }

            if ((termin.DatumVrijemePocetak - DateTime.Now).TotalHours < 24)
            {
                TempData["Greska"] = "Termin se ne može otkazati unutar 24 sata.";
                return RedirectToAction("Index", "Profil");
            }

            _context.Termini.Remove(termin);
            await _context.SaveChangesAsync();

            TempData["Poruka"] = "Termin je uspješno otkazan.";
            return RedirectToAction("Index", "Profil");
        }

        [HttpPost]
        public async Task<IActionResult> PreuzmiQrPdf()
        {
            try
            {
                if (!IsUserLoggedIn())
                {
                    TempData["Greska"] = "Morate biti prijavljeni da biste preuzeli QR kod.";
                    return RedirectToAction("Login", "Account");
                }

                var korisniciId = HttpContext.Session.GetInt32("KorisniciId");
                var korisnik = await _context.Korisnici.FindAsync(korisniciId);
                if (korisnik == null)
                {
                    TempData["Greska"] = "Korisnik nije pronađen.";
                    return RedirectToAction("Index");
                }

                if (string.IsNullOrEmpty(korisnik.QrKod))
                {
                    TempData["Greska"] = "QR kod nije dostupan.";
                    return RedirectToAction("Index");
                }

                byte[] qrBytes = Convert.FromBase64String(korisnik.QrKod);
                using var document = new PdfDocument();
                var page = document.AddPage();
                page.Size = PdfSharpCore.PageSize.A4;

                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    var image = XImage.FromStream(() => new MemoryStream(qrBytes));
                    gfx.DrawImage(image, 50, 50, 200, 200);

                    var titleFont = new XFont("Arial", 16, XFontStyle.Bold);
                    var textFont = new XFont("Arial", 12, XFontStyle.Regular);

                    gfx.DrawString("MediPlan - Vaš QR Kod", titleFont, XBrushes.Black,
                        new XRect(0, 20, page.Width, 20), XStringFormats.TopCenter);

                    gfx.DrawString($"Ime: {korisnik.Ime}", textFont, XBrushes.Black,
                        new XPoint(50, 300));
                    gfx.DrawString($"Prezime: {korisnik.Prezime}", textFont, XBrushes.Black,
                        new XPoint(50, 320));
                    gfx.DrawString($"Datum izdavanja: {DateTime.Now:dd.MM.yyyy}", textFont, XBrushes.Black,
                        new XPoint(50, 340));
                }

                using var msPdf = new MemoryStream();
                document.Save(msPdf);
                msPdf.Position = 0;

                return File(msPdf.ToArray(), "application/pdf",
                    $"QR_Kod_{korisnik.Ime}_{korisnik.Prezime}.pdf");
            }
            catch (Exception ex)
            {
                TempData["Greska"] = $"Došlo je do greške: {ex.Message}";
                return RedirectToAction("Index");
            }
        }




        [HttpPost]
        public async Task<IActionResult> PosaljiQrPdfEmail()
        {
            try
            {
                if (!IsUserLoggedIn())
                {
                    TempData["Greska"] = "Morate biti prijavljeni da biste poslali QR kod.";
                    return RedirectToAction("Login", "Account");
                }

                var korisniciId = HttpContext.Session.GetInt32("KorisniciId");
                var korisnik = await _context.Korisnici.FindAsync(korisniciId);
                if (korisnik == null)
                {
                    TempData["Greska"] = "Korisnik nije pronađen.";
                    return RedirectToAction("Index");
                }

                if (string.IsNullOrEmpty(korisnik.QrKod))
                {
                    TempData["Greska"] = "QR kod nije dostupan.";
                    return RedirectToAction("Index");
                }

                byte[] qrBytes = Convert.FromBase64String(korisnik.QrKod);
                using var document = new PdfDocument();
                var page = document.AddPage();
                page.Size = PdfSharpCore.PageSize.A4;

                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    var image = XImage.FromStream(() => new MemoryStream(qrBytes));
                    gfx.DrawImage(image, 50, 50, 200, 200);

                    var titleFont = new XFont("Arial", 16, XFontStyle.Bold);
                    var textFont = new XFont("Arial", 12, XFontStyle.Regular);

                    gfx.DrawString("MediPlan - Vaš QR Kod", titleFont, XBrushes.Black,
                        new XRect(0, 20, page.Width, 20), XStringFormats.TopCenter);

                    gfx.DrawString($"Ime: {korisnik.Ime}", textFont, XBrushes.Black,
                        new XPoint(50, 300));
                    gfx.DrawString($"Prezime: {korisnik.Prezime}", textFont, XBrushes.Black,
                        new XPoint(50, 320));
                    gfx.DrawString($"Datum izdavanja: {DateTime.Now:dd.MM.yyyy}", textFont, XBrushes.Black,
                        new XPoint(50, 340));
                }

                using var msPdf = new MemoryStream();
                document.Save(msPdf);
                msPdf.Position = 0;

                var pdfBytes = msPdf.ToArray();

                var subject = "Vaš MediPlan QR Kod";
                var body = $"Poštovani {korisnik.Ime},<br/><br/>U prilogu vam šaljemo vaš QR kod.<br/><br/>Srdačan pozdrav,<br/>MediPlan tim";

                await _emailService.SendEmailWithAttachmentAsync(
                    korisnik.Email,
                    subject,
                    body,
                    pdfBytes,
                    $"QR_Kod_{korisnik.Ime}_{korisnik.Prezime}.pdf"
                );

                TempData["Poruka"] = "PDF sa QR kodom je uspješno poslan na vaš email.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Greska"] = $"Došlo je do greške prilikom slanja emaila: {ex.Message}";
                return RedirectToAction("Index");
            }
        }



        private bool IsUserLoggedIn()
        {
            var korisniciId = HttpContext.Session.GetInt32("KorisniciId");
            var username = HttpContext.Session.GetString("Username");

            return korisniciId.HasValue && !string.IsNullOrEmpty(username);
        }
    }
}
