using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DWL_CRM.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ansprechperson> Ansprechpeople { get; set; }

    public virtual DbSet<Firma> Firmas { get; set; }

    public virtual DbSet<Geschaeftsfuehrer> Geschaeftsfuehrers { get; set; }

    public virtual DbSet<Ort> Orts { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<Rechnungsdaten> Rechnungsdatens { get; set; }

    public virtual DbSet<StagingCsv> StagingCsvs { get; set; }

    public virtual DbSet<TmpPersonen> TmpPersonens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_german2_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Ansprechperson>(entity =>
        {
            entity.HasKey(e => e.AnsprechpersonId).HasName("PRIMARY");

            entity.ToTable("ansprechperson");

            entity.HasIndex(e => e.PersonId, "fk_ansprechperson_person");

            entity.Property(e => e.AnsprechpersonId).HasColumnName("Ansprechperson_ID");
            entity.Property(e => e.PersonId).HasColumnName("Person_ID");

            entity.HasOne(d => d.Person).WithMany(p => p.Ansprechpeople)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ansprechperson_person");
        });

        modelBuilder.Entity<Firma>(entity =>
        {
            entity.HasKey(e => e.FirmaId).HasName("PRIMARY");

            entity.ToTable("firma");

            entity.HasIndex(e => e.OrtId, "fk_firma_ort");

            entity.Property(e => e.FirmaId).HasColumnName("Firma_ID");
            entity.Property(e => e.Bemerkungen).HasColumnType("text");
            entity.Property(e => e.Branche).HasMaxLength(100);
            entity.Property(e => e.Firmenname).HasMaxLength(255);
            entity.Property(e => e.Jahresumsatz).HasPrecision(15, 2);
            entity.Property(e => e.OrtId).HasColumnName("Ort_ID");
            entity.Property(e => e.Strasse).HasMaxLength(255);

            entity.HasOne(d => d.Ort).WithMany(p => p.Firmas)
                .HasForeignKey(d => d.OrtId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_firma_ort");
        });

        modelBuilder.Entity<Geschaeftsfuehrer>(entity =>
        {
            entity.HasKey(e => e.GeschaeftsfuehrerId).HasName("PRIMARY");

            entity.ToTable("geschaeftsfuehrer");

            entity.HasIndex(e => e.PersonId, "fk_geschaeftsfuehrer_person");

            entity.Property(e => e.GeschaeftsfuehrerId).HasColumnName("Geschaeftsfuehrer_ID");
            entity.Property(e => e.PersonId).HasColumnName("Person_ID");

            entity.HasOne(d => d.Person).WithMany(p => p.Geschaeftsfuehrers)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_geschaeftsfuehrer_person");
        });

        modelBuilder.Entity<Ort>(entity =>
        {
            entity.HasKey(e => e.OrtId).HasName("PRIMARY");

            entity.ToTable("ort");

            entity.HasIndex(e => new { e.Plz, e.Ortsname }, "uq_ort_plz_ortsname").IsUnique();

            entity.Property(e => e.OrtId).HasColumnName("Ort_ID");
            entity.Property(e => e.Ortsname).HasMaxLength(100);
            entity.Property(e => e.Plz)
                .HasMaxLength(10)
                .HasColumnName("PLZ");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.PersonId).HasName("PRIMARY");

            entity.ToTable("person");

            entity.HasIndex(e => e.FirmaId, "fk_person_firma");

            entity.Property(e => e.PersonId).HasColumnName("Person_ID");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirmaId).HasColumnName("Firma_ID");
            entity.Property(e => e.Nachname).HasMaxLength(100);
            entity.Property(e => e.Telefon).HasMaxLength(255);
            entity.Property(e => e.Titel).HasMaxLength(50);
            entity.Property(e => e.Vorname).HasMaxLength(100);

            entity.HasOne(d => d.Firma).WithMany(p => p.People)
                .HasForeignKey(d => d.FirmaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_person_firma");
        });

        modelBuilder.Entity<Rechnungsdaten>(entity =>
        {
            entity.HasKey(e => e.RechnungsdatenId).HasName("PRIMARY");

            entity.ToTable("rechnungsdaten");

            entity.HasIndex(e => e.FirmaId, "uq_rechnungsdaten_firma").IsUnique();

            entity.Property(e => e.RechnungsdatenId).HasColumnName("Rechnungsdaten_ID");
            entity.Property(e => e.FirmaId).HasColumnName("Firma_ID");
            entity.Property(e => e.LetzterZahlungseingang).HasColumnName("Letzter_Zahlungseingang");
            entity.Property(e => e.RechnungenGesamt)
                .HasPrecision(15, 2)
                .HasColumnName("Rechnungen_Gesamt");
            entity.Property(e => e.RechnungenOffen)
                .HasPrecision(15, 2)
                .HasColumnName("Rechnungen_Offen");

            entity.HasOne(d => d.Firma).WithOne(p => p.Rechnungsdaten)
                .HasForeignKey<Rechnungsdaten>(d => d.FirmaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rechnungsdaten_firma");
        });

        modelBuilder.Entity<StagingCsv>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("staging_csv");

            entity.Property(e => e.Ansprechpartner).HasColumnType("text");
            entity.Property(e => e.Bemerkungen).HasColumnType("text");
            entity.Property(e => e.Branche).HasColumnType("text");
            entity.Property(e => e.Email).HasColumnType("text");
            entity.Property(e => e.Firma).HasColumnType("text");
            entity.Property(e => e.Geschaeftsfuehrer).HasColumnType("text");
            entity.Property(e => e.GfGeburtsdatum)
                .HasColumnType("text")
                .HasColumnName("GF_Geburtsdatum");
            entity.Property(e => e.Gruendungsdatum).HasColumnType("text");
            entity.Property(e => e.Jahresumsatz).HasColumnType("text");
            entity.Property(e => e.LetzterZahlungseingang)
                .HasColumnType("text")
                .HasColumnName("Letzter_Zahlungseingang");
            entity.Property(e => e.PlzOrt)
                .HasColumnType("text")
                .HasColumnName("PLZ_Ort");
            entity.Property(e => e.RechnungenGesamt)
                .HasColumnType("text")
                .HasColumnName("Rechnungen_Gesamt");
            entity.Property(e => e.RechnungenOffen)
                .HasColumnType("text")
                .HasColumnName("Rechnungen_Offen");
            entity.Property(e => e.Strasse).HasColumnType("text");
            entity.Property(e => e.Telefon).HasColumnType("text");
        });

        modelBuilder.Entity<TmpPersonen>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tmp_personen");

            entity.Property(e => e.Email).HasColumnType("text");
            entity.Property(e => e.Firma).HasMaxLength(255);
            entity.Property(e => e.Geburtsdatum).HasColumnType("text");
            entity.Property(e => e.Name).HasColumnType("text");
            entity.Property(e => e.Rolle).HasMaxLength(50);
            entity.Property(e => e.Telefon).HasColumnType("text");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
