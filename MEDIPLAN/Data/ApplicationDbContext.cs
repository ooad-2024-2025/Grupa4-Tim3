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

        public DbSet<Korisnici> Korisnici { get; set; }
        public DbSet<Termini> Termini { get; set; }
        public DbSet<Recenzije> Recenzije { get; set; }
        public DbSet<HistorijaNalaza> HistorijaNalaza { get; set; }
        public DbSet<MedicinskeUsluge> MedicinskeUsluge { get; set; }
        public DbSet<Usluge> Usluge { get; set; }
        // Dodaj ostale DbSet-ove ako imaš još entiteta
    }
}
