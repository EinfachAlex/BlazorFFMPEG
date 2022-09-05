using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BlazorFFMPEG.Backend.Database
{
    public partial class databaseContext : DbContext
    {
        public databaseContext()
        {
        }

        public databaseContext(DbContextOptions<databaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ConstantsStatus> ConstantsStatuses { get; set; } = null!;
        public virtual DbSet<Encodejob> Encodejobs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Name=ConnectionStrings:blazorFFMPEG");
                optionsBuilder.UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConstantsStatus>(entity =>
            {
                entity.ToTable("constants_status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description).HasColumnName("description");
            });

            modelBuilder.Entity<Encodejob>(entity =>
            {
                entity.HasKey(e => e.Jobid)
                    .HasName("encodejobs_pkey");

                entity.ToTable("encodejobs");

                entity.Property(e => e.Jobid).HasColumnName("jobid");

                entity.Property(e => e.Status)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("status");

                entity.HasOne(d => d.StatusNavigation)
                    .WithMany(p => p.Encodejobs)
                    .HasForeignKey(d => d.Status)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_status");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
