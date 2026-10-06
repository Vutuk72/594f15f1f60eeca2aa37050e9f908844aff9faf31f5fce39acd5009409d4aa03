using System;
using System.Collections.Generic;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.TelNumbers, "users_telnumber_id").IsUnique();

            entity.HasIndex(e => e.KeycloakId, "users_keycloak_id_key").IsUnique();

            entity.HasIndex(e => e.Email, "users_email_id").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.KeycloakId)
                .HasComment("ID uživatele v Keycloaku (claim sub)")
                .HasColumnName("keycloak_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.TelNumbers)
                .HasMaxLength(20)
                .HasComment("telefonní číslo")
                .HasColumnName("tel_numbers");
            entity.Property(e => e.Email)
                .HasComment("E-mail uživatele (kopie z Keycloaku)")
                .HasColumnName("email")
                .HasMaxLength(254);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
