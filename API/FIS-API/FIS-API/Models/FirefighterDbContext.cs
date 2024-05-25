using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FIS_API.Models;

public partial class FirefighterDbContext : DbContext
{
    public FirefighterDbContext()
    {
    }

    public FirefighterDbContext(DbContextOptions<FirefighterDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FireDepartment> FireDepartments { get; set; }

    public virtual DbSet<Firefighter> Firefighters { get; set; }

    public virtual DbSet<FirefighterIntervention> FirefighterInterventions { get; set; }

    public virtual DbSet<Intervention> Interventions { get; set; }

    public virtual DbSet<InterventionType> InterventionTypes { get; set; }

    public virtual DbSet<Login> Logins { get; set; }

    public virtual DbSet<Rank> Ranks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:ffDB");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FireDepartment>(entity =>
        {
            entity.HasKey(e => e.IdFd).HasName("PK__FireDepa__8B622763022EDD6D");

            entity.ToTable("FireDepartment");

            entity.Property(e => e.IdFd)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("ID_FD");
            entity.Property(e => e.CmdrId).HasColumnName("Cmdr_ID");
            entity.Property(e => e.Location).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Cmdr).WithMany(p => p.FireDepartments)
                .HasForeignKey(d => d.CmdrId)
                .HasConstraintName("FK_FirefighterCommander");
        });

        modelBuilder.Entity<Firefighter>(entity =>
        {
            entity.HasKey(e => e.IdFf).HasName("PK__Firefigh__8B62276124A22E48");

            entity.ToTable("Firefighter");

            entity.Property(e => e.IdFf)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("ID_FF");
            entity.Property(e => e.ActiveDate).HasColumnType("datetime");
            entity.Property(e => e.FdId).HasColumnName("FD_ID");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.RankId).HasColumnName("Rank_ID");
            entity.Property(e => e.Surname).HasMaxLength(50);

            entity.HasOne(d => d.Fd).WithMany(p => p.Firefighters)
                .HasForeignKey(d => d.FdId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FireDepartment");

            entity.HasOne(d => d.Rank).WithMany(p => p.Firefighters)
                .HasForeignKey(d => d.RankId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Rank");
        });

        modelBuilder.Entity<FirefighterIntervention>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Firefigh__3214EC270DF8B1D2");

            entity.ToTable("Firefighter-Intervention");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.FfId).HasColumnName("FF_ID");
            entity.Property(e => e.IntId).HasColumnName("Int_ID");

            entity.HasOne(d => d.Ff).WithMany(p => p.FirefighterInterventions)
                .HasForeignKey(d => d.FfId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FI_Firefighter");

            entity.HasOne(d => d.Int).WithMany(p => p.FirefighterInterventions)
                .HasForeignKey(d => d.IntId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FI_Intervention");
        });

        modelBuilder.Entity<Intervention>(entity =>
        {
            entity.HasKey(e => e.IdInt).HasName("PK__Interven__2C7D41C24497F725");

            entity.ToTable("Intervention");

            entity.Property(e => e.IdInt).HasColumnName("ID_Int");
            entity.Property(e => e.CmdrId).HasColumnName("Cmdr_ID");
            entity.Property(e => e.Location).HasMaxLength(300);
            entity.Property(e => e.TypeId).HasColumnName("Type_ID");

            entity.HasOne(d => d.Cmdr).WithMany(p => p.Interventions)
                .HasForeignKey(d => d.CmdrId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InterventionCommander");

            entity.HasOne(d => d.Type).WithMany(p => p.Interventions)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InterventionType");
        });

        modelBuilder.Entity<InterventionType>(entity =>
        {
            entity.HasKey(e => e.IdType).HasName("PK__Interven__DF519A38BCE6C620");

            entity.ToTable("InterventionType");

            entity.Property(e => e.IdType).HasColumnName("ID_Type");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("PK__Login__536C85E588D70F4D");

            entity.ToTable("Login");

            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.Password)
                .HasMaxLength(64)
                .IsFixedLength();
            entity.Property(e => e.UserGuid).HasColumnName("UserGUID");

            entity.HasOne(d => d.User).WithMany(p => p.Logins)
                .HasForeignKey(d => d.UserGuid)
                .HasConstraintName("FK_FirefighterID");
        });

        modelBuilder.Entity<Rank>(entity =>
        {
            entity.HasKey(e => e.IdRank).HasName("PK__Rank__7F58156D0DB38DC6");

            entity.ToTable("Rank");

            entity.Property(e => e.IdRank).HasColumnName("ID_Rank");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
