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
        public DbSet<Recenzija> Recenzije { get; set; }
        public DbSet<HistorijaNalaza> HistorijaNalaza { get; set; }
        public DbSet<MedicinskeUsluge> MedicinskeUsluge { get; set; }
        public DbSet<Usluge> Usluge { get; set; }

        // Dodaj DbSet za Notifikacije
        public DbSet<Notifikacije> Notifikacije { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfigurišemo odnos za doktora u Termini
            modelBuilder.Entity<Termini>()
                .HasOne(t => t.Doktor)
                .WithMany(k => k.TerminiDoktor)
                .HasForeignKey(t => t.DoktorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Konfigurišemo odnos za pacijenta u Termini
            modelBuilder.Entity<Termini>()
                .HasOne(t => t.Pacijent)
                .WithMany(k => k.TerminiPacijent)
                .HasForeignKey(t => t.PacijentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Konfigurišemo odnos za Notifikacije -> Doktor (Korisnici)
            modelBuilder.Entity<Notifikacije>()
                .HasOne(n => n.Doktor)
                .WithMany() // Ako ne želiš kolekciju Notifikacija u Korisnici, ostavi ovako
                .HasForeignKey(n => n.DoktorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
