
# MediPLan Klinika

![logo](https://github.com/user-attachments/assets/a95bea97-abe7-438d-92fc-48fb5c378c6e)

## O projektu

**MediPlan** je sistem za upravljanje radom medicinske klinike. Omogućava jednostavno zakazivanje termina, upravljanje pacijentima i doktorima, unos medicinskih nalaza, te pruža podršku za recenzije i obavještenja putem emaila.

---

## Testni podaci za pristup 

Ako je potrebno unijeti podatke prilikom pristupa aplikaciji, koristite sljedeće:

### Testni korisnik (Pacijent)

- **Ime i prezime:** Random Osoba  
- **Email:** `randomosoba86@gmail.com`  
- **Lozinka:** `ooadprojekat2025`  
- **Uloga:** Pacijent

### Doktori (username)

Ovo je lista username-a doktora iz baze podataka MediPlan projekta:

**Sifra svakog:** password1234
- doktor_im_1  
- doktor_im_2  
- doktor_kr_1  
- doktor_kr_2  
- doktor_of_1  
- doktor_of_2  
- doktor_de_1  
- doktor_de_2  
- doktor_en_1  
- doktor_en_2  
- doktor_gi_1  
- doktor_gi_2  
- doktor_ne_1  
- doktor_ne_2  
- doktor_ra_1  
- doktor_ra_2  
- doktor_la_1  
- doktor_la_2  
---
## Konekcijski stringovi za bazu

> **Lozinka baze:** `ooadprojekat2025`

### ASP.NET (.NET Core / EF Core)

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=SQL6032.site4now.net;Initial Catalog=db_aba416_mediplan;User Id=db_aba416_mediplan_admin;Password=ooadprojekat2025"
}
