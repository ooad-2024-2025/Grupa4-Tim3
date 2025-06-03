using Microsoft.AspNetCore.Mvc;
using MediPlan.Models;
using System.Collections.Generic;
using System.Linq;

namespace MediPlan.Controllers
{
    public class HomeController : Controller
    {
        // Testni podaci za usluge
        private static readonly List<Usluga> _usluge = new List<Usluga>
        {
            new Usluga { Id = 1, Naziv = "Interna medicina", Opis = "Interna medicina je grana medicine koja se bavi dijagnostikom, liječenjem i prevencijom bolesti unutarnjih organa kao što su srce, pluća, bubrezi, probavni trakt i drugi vitalni sustavi. Naši internisti pružaju cjelovitu njegu i usmjeravaju pacijente kroz kompleksne zdravstvene izazove.", Ikona = "images/interna-medicina.png", Odjel = Odjel.InternaMedicina },

            new Usluga { Id = 2, Naziv = "Kardiologija", Opis = "Kardiologija je specijalizacija usmjerena na bolesti srca i krvnih žila. Naši kardiolozi pružaju dijagnostiku i liječenje kardiovaskularnih bolesti, uključujući aritmije, koronarne bolesti, srčane insuficijencije te primjenjuju najmodernije dijagnostičke i terapijske metode.", Ikona = "images/kardiologija.png", Odjel = Odjel.Kardiologija },

            new Usluga { Id = 3, Naziv = "Oftamologija", Opis = "Oftamologija se bavi pregledom, dijagnostikom i liječenjem bolesti oka i vidnog sustava. Naši oftamolozi pružaju usluge pregleda vida, liječenja raznih očnih bolesti, operacija katarakte i drugih intervencija za očuvanje i poboljšanje vida.", Ikona = "images/oftamologija.png", Odjel = Odjel.Oftamologija },

            new Usluga { Id = 4, Naziv = "Dermatologija", Opis = "Dermatologija je grana medicine koja se bavi dijagnostikom i liječenjem kožnih bolesti, kao i problema s kosom i noktima. Naši dermatolozi pružaju stručnu njegu za akne, ekceme, psorijazu, alergije, kožne tumore i druge dermatološke probleme.", Ikona = "images/dermatologija.png", Odjel = Odjel.Dermatologija },

            new Usluga { Id = 5, Naziv = "Endokrinologija", Opis = "Endokrinologija je specijalizacija koja proučava hormone i žlijezde s unutarnjim izlučivanjem, te bolesti poput dijabetesa, poremećaja štitnjače, hormona rasta i drugih endokrinih poremećaja. Naši endokrinolozi pružaju dijagnostiku, terapiju i kontinuiranu njegu pacijenata.", Ikona = "images/endokrinologija.png", Odjel = Odjel.Endokrinologija },

            new Usluga { Id = 6, Naziv = "Ginekologija", Opis = "Ginekologija je grana medicine koja se bavi zdravljem ženskog reproduktivnog sustava, uključujući maternicu, jajnike, jajovode i dojke. Naši stručnjaci pružaju sveobuhvatnu njegu ženama svih životnih dobi, uključujući preventivne preglede, dijagnostiku, liječenje ginekoloških bolesti, kao i savjetovanje vezano za kontracepciju, trudnoću i menopauzu.", Ikona = "images/gynecology.png", Odjel = Odjel.Ginekologija },

            new Usluga { Id = 7, Naziv = "Neurologija", Opis = "Neurologija se bavi bolestima živčanog sustava, uključujući mozak, kralježničnu moždinu i periferni živčani sustav. Naši neurologi dijagnosticiraju i liječe stanja poput moždanog udara, epilepsije, multipla skleroze, migrena i neurodegenerativnih bolesti.", Ikona = "images/neurologija.png", Odjel = Odjel.Neurologija },

            new Usluga { Id = 8, Naziv = "Radiologija", Opis = "Radiologija je medicinska grana koja koristi slikovne tehnike poput rendgena, ultrazvuka, CT-a i MR-a za dijagnostiku bolesti i ozljeda. Naši radiolozi provode detaljne preglede i surađuju s drugim specijalistima u postavljanju točne dijagnoze i planiranju terapije.", Ikona = "images/radiology.png", Odjel = Odjel.Radiologija }

        };

        // Testni podaci za korisnike (doktore)
        private static readonly List<Korisnik> _korisnici = new List<Korisnik>
        {
            new Korisnik { Id = 1, Ime = "Marko", Prezime = "Markovic", Email = "marko@primjer.com", Uloga = Uloga.Doktor, Odjel = Odjel.InternaMedicina },
            new Korisnik { Id = 2, Ime = "Ana", Prezime = "Anic", Email = "ana@primjer.com", Uloga = Uloga.Doktor, Odjel = Odjel.Kardiologija },
            new Korisnik { Id = 3, Ime = "Ivan", Prezime = "Ivanovic", Email = "ivan@primjer.com", Uloga = Uloga.Doktor, Odjel = Odjel.Kardiologija },
            new Korisnik { Id = 4, Ime = "Jelena", Prezime = "Jelic", Email = "jelena@primjer.com", Uloga = Uloga.Doktor, Odjel = Odjel.Oftamologija },
            // Dodaj još doktora po potrebi
        };

        public IActionResult Index() => View();

        public IActionResult About() => View();

        public IActionResult Usluge()
        {
            return View(_usluge);
        }

        public IActionResult DetaljiUsluge(string naziv)
        {
            if (string.IsNullOrEmpty(naziv))
                return RedirectToAction("Usluge");

            var usluga = _usluge.FirstOrDefault(u => u.Naziv == naziv);
            if (usluga == null)
                return NotFound();

            // Dohvati doktore za odjel te usluge
            var doktoriZaUslugu = _korisnici
                .Where(k => k.Uloga == Uloga.Doktor && k.Odjel == usluga.Odjel)
                .ToList();

            var model = new DetaljiUslugeViewModel
            {
                Usluga = usluga,
                Doktori = doktoriZaUslugu
            };

            return View(model);
        }

        public IActionResult Tim() => View();

        public IActionResult Kontakt() => View();

        public IActionResult Zakazi() => View();

        public IActionResult Privacy() => View();
    }
}
