using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MediPlan.Models; // zamijeni sa stvarnim namespace-om

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // DbSetovi za tvoje entitete
    public DbSet<Korisnik> Korisnici { get; set; }
    public DbSet<Termin> Termini { get; set; }
    public DbSet<MedicinskeUsluge> MedicinskeUsluge { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Korisnik>().ToTable("Korisnik");
        modelBuilder.Entity<Termin>().ToTable("Termin");
        modelBuilder.Entity<MedicinskeUsluge>().ToTable("MedicinskeUsluge");

        base.OnModelCreating(modelBuilder);
    }
}
