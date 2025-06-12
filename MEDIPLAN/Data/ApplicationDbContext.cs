using Microsoft.EntityFrameworkCore;
using MEDIPLAN.Models;

namespace MEDIPLAN.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Korisnik> Korisnici { get; set; }
        public DbSet<Termin> Termini { get; set; }
        public DbSet<Recenzija> Recenzije { get; set; }
        public DbSet<HistorijaGenerisanihNalaza> HistorijaNalaza { get; set; }
        public DbSet<MedicinskeUsluge> MedicinskeUsluge { get; set; }
        public DbSet<Usluga> Usluge { get; set; }
        // Dodaj ostale DbSet-ove ako imaš još entiteta
    }
}
