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
    // Interna medicina
    new Korisnik { Id = 1, Ime = "Amar", Prezime = "Husic", Email = "amar.husic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.InternaMedicina },
    new Korisnik { Id = 2, Ime = "Lejla", Prezime = "Fazlic", Email = "lejla.fazlic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.InternaMedicina },
    new Korisnik { Id = 3, Ime = "Dino", Prezime = "Selimovic", Email = "dino.selimovic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.InternaMedicina },
    new Korisnik { Id = 4, Ime = "Sara", Prezime = "Softic", Email = "sara.softic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.InternaMedicina },

    // Kardiologija
    new Korisnik { Id = 5, Ime = "Ana", Prezime = "Anic", Email = "ana.anic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Kardiologija },
    new Korisnik { Id = 6, Ime = "Ivan", Prezime = "Ivanovic", Email = "ivan.ivanovic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Kardiologija },
    new Korisnik { Id = 7, Ime = "Mina", Prezime = "Milosavljevic", Email = "mina.milosavljevic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Kardiologija },
    new Korisnik { Id = 8, Ime = "Faruk", Prezime = "Dzafic", Email = "faruk.dzafic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Kardiologija },

    // Oftamologija
    new Korisnik { Id = 9, Ime = "Jelena", Prezime = "Jelic", Email = "jelena.jelic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Oftamologija },
    new Korisnik { Id = 10, Ime = "Tarik", Prezime = "Zulic", Email = "tarik.zulic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Oftamologija },
    new Korisnik { Id = 11, Ime = "Azra", Prezime = "Balic", Email = "azra.balic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Oftamologija },
    new Korisnik { Id = 12, Ime = "Edin", Prezime = "Kovacevic", Email = "edin.kovacevic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Oftamologija },

    // Dermatologija
    new Korisnik { Id = 13, Ime = "Emir", Prezime = "Emirovic", Email = "emir.emirovic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Dermatologija },
    new Korisnik { Id = 14, Ime = "Lejla", Prezime = "Lekic", Email = "lejla.lekic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Dermatologija },
    new Korisnik { Id = 15, Ime = "Selma", Prezime = "Hasanovic", Email = "selma.hasanovic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Dermatologija },
    new Korisnik { Id = 16, Ime = "Aldin", Prezime = "Alikadic", Email = "aldin.alikadic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Dermatologija },

    // Endokrinologija
    new Korisnik { Id = 17, Ime = "Sara", Prezime = "Saric", Email = "sara.saric@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Endokrinologija },
    new Korisnik { Id = 18, Ime = "Adnan", Prezime = "Ademovic", Email = "adnan.ademovic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Endokrinologija },
    new Korisnik { Id = 19, Ime = "Irma", Prezime = "Herceg", Email = "irma.herceg@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Endokrinologija },
    new Korisnik { Id = 20, Ime = "Nedim", Prezime = "Ibric", Email = "nedim.ibric@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Endokrinologija },

    // Ginekologija
    new Korisnik { Id = 21, Ime = "Maja", Prezime = "Majic", Email = "maja.majic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Ginekologija },
    new Korisnik { Id = 22, Ime = "Elma", Prezime = "Hadzic", Email = "elma.hadzic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Ginekologija },
    new Korisnik { Id = 23, Ime = "Aida", Prezime = "Brkic", Email = "aida.brkic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Ginekologija },
    new Korisnik { Id = 24, Ime = "Senad", Prezime = "Begovic", Email = "senad.begovic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Ginekologija },

    // Neurologija
    new Korisnik { Id = 25, Ime = "Elma", Prezime = "Elmaz", Email = "elma.elmaz@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Neurologija },
    new Korisnik { Id = 26, Ime = "Vedad", Prezime = "Velic", Email = "vedad.velic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Neurologija },
    new Korisnik { Id = 27, Ime = "Selma", Prezime = "Mujic", Email = "selma.mujic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Neurologija },
    new Korisnik { Id = 28, Ime = "Dzenan", Prezime = "Begic", Email = "dzenan.begic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Neurologija },

    // Radiologija
    new Korisnik { Id = 29, Ime = "Tarik", Prezime = "Tarikovic", Email = "tarik.tarikovic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Radiologija },
    new Korisnik { Id = 30, Ime = "Anesa", Prezime = "Dedic", Email = "anesa.dedic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Radiologija },
    new Korisnik { Id = 31, Ime = "Edina", Prezime = "Zukic", Email = "edina.zukic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Radiologija },
    new Korisnik { Id = 32, Ime = "Benjamin", Prezime = "Osmanovic", Email = "benjamin.osmanovic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.Radiologija },

    // Laboratorijske usluge
    new Korisnik { Id = 33, Ime = "Ines", Prezime = "Mahmutovic", Email = "ines.mahmutovic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.LaboratorijskeUsluge },
    new Korisnik { Id = 34, Ime = "Haris", Prezime = "Memic", Email = "haris.memic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.LaboratorijskeUsluge },
    new Korisnik { Id = 35, Ime = "Amila", Prezime = "Sinanovic", Email = "amila.sinanovic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.LaboratorijskeUsluge },
    new Korisnik { Id = 36, Ime = "Damir", Prezime = "Basic", Email = "damir.basic@mediplan.ba", Uloga = Uloga.Doktor, Odjel = Odjel.LaboratorijskeUsluge },
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

        

        public IActionResult Kontakt() => View();
       
        [HttpPost]
        public IActionResult Kontakt(string ime, string email, string poruka)
        {
            TempData["Poruka"] = "Hvala na poruci, " + ime + "! Odgovorit ćemo vam uskoro.";
            return RedirectToAction("Kontakt");
        }


        public IActionResult Zakazi() => View();
        public IActionResult NasTim()
        {
            var model = _korisnici
                .Where(k => k.Uloga == Uloga.Doktor)
                .GroupBy(k => k.Odjel)
                .ToDictionary(g => g.Key, g => g.ToList());

            return View(model);
        }




        public IActionResult Privacy() => View();
    }
}
