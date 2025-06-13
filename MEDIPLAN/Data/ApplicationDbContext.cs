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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfiguracija 1:1 veza između Termini i Korisnici (Doktor)
            modelBuilder.Entity<Termini>()
                .HasOne(t => t.Doktor)
                .WithOne(k => k.TerminiDoktor)
                .HasForeignKey<Termini>(t => t.DoktorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Konfiguracija 1:1 veza između Termini i Korisnici (Pacijent)
            modelBuilder.Entity<Termini>()
                .HasOne(t => t.Pacijent)
                .WithOne(k => k.TerminiPacijent)
                .HasForeignKey<Termini>(t => t.PacijentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
