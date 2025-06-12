using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

    public virtual DbSet<Recenzije> Recenzijes { get; set; }

    public virtual DbSet<Termini> Terminis { get; set; }

    public virtual DbSet<Usluge> Usluges { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=SQL6032.site4now.net;Initial Catalog=db_aba416_mediplan;User Id=db_aba416_mediplan_admin;Password=ooadprojekat2025");

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

            entity.HasOne(d => d.MedicinskaUsluga).WithMany(p => p.Korisnicis)
                .HasForeignKey(d => d.MedicinskaUslugaId)
                .HasConstraintName("FK_Korisnici_MedicinskeUsluge");
        });

        modelBuilder.Entity<MedicinskeUsluge>(entity =>
        {
            entity.ToTable("MedicinskeUsluge");
        });

        modelBuilder.Entity<Recenzije>(entity =>
        {
            entity.ToTable("Recenzije");
        });

        modelBuilder.Entity<Termini>(entity =>
        {
            entity.ToTable("Termini");
        });

        modelBuilder.Entity<Usluge>(entity =>
        {
            entity.ToTable("Usluge");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
