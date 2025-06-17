using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MEDIPLAN.Models;


namespace MEDIPLAN.Models;

public partial class DbAba416MediplanContext : DbContext
{
    public DbAba416MediplanContext()
    {
    }

    public DbAba416MediplanContext(DbContextOptions<DbAba416MediplanContext> options)
        : base(options)
    {
    }

    public virtual DbSet<HistorijaNalaza> HistorijaNalazas { get; set; }

    public virtual DbSet<Korisnici> Korisnicis { get; set; }

    public virtual DbSet<MedicinskeUsluge> MedicinskeUsluges { get; set; }

    public virtual DbSet<Recenzija> Recenzijes { get; set; }

    public virtual DbSet<Termini> Terminis { get; set; }

    public virtual DbSet<Usluge> Usluges { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = Environment.GetEnvironmentVariable("MEDIPLAN_DB_CONNECTION");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string is not configured. Please set the MEDIPLAN_DB_CONNECTION environment variable.");
            }
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HistorijaNalaza>(entity =>
        {
            entity.ToTable("HistorijaNalaza");

            entity.HasIndex(e => e.TerminId, "IX_HistorijaNalaza_TerminId");
        });

        modelBuilder.Entity<Korisnici>(entity =>
        {
            entity.ToTable("Korisnici");

            entity.Property(e => e.PhotoFileName).HasMaxLength(255);

            entity.HasOne(d => d.MedicinskaUsluga).WithMany(p => p.Korisnicis)
                .HasForeignKey(d => d.MedicinskaUslugaId)
                .HasConstraintName("FK_Korisnici_MedicinskeUsluge");
        });

        modelBuilder.Entity<MedicinskeUsluge>(entity =>
        {
            entity.ToTable("MedicinskeUsluge");
        });

        modelBuilder.Entity<Recenzija>(entity =>
        {
            entity.ToTable("Recenzije");
        });

        modelBuilder.Entity<Termini>(entity =>
        {
            entity.ToTable("Termini");

            entity.HasOne(d => d.Doktor).WithMany(p => p.TerminiDoktor)
                .HasForeignKey(d => d.DoktorId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Pacijent).WithMany(p => p.TerminiPacijent)
                .HasForeignKey(d => d.PacijentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Usluge>(entity =>
        {
            entity.ToTable("Usluge");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
