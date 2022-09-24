using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.VisualBasic;

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

        private static string connectionString;
        
        public databaseContext(string connectionString_)
        {
            connectionString = connectionString_;
        }

        public virtual DbSet<ConstantsQualitymethod> ConstantsQualitymethods { get; set; } = null!;
        public virtual DbSet<ConstantsStatus> ConstantsStatuses { get; set; } = null!;
        public virtual DbSet<EncodeJob> EncodeJobs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(connectionString);
                optionsBuilder.UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConstantsQualitymethod>(entity =>
            {
                entity.ToTable("constants_qualitymethod");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Maxqualityvalue).HasColumnName("maxqualityvalue");

                entity.Property(e => e.Minqualityvalue).HasColumnName("minqualityvalue");
            });

            modelBuilder.Entity<ConstantsStatus>(entity =>
            {
                entity.ToTable("constants_status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description).HasColumnName("description");
            });

            modelBuilder.Entity<EncodeJob>(entity =>
            {
                entity.HasKey(e => e.Jobid)
                    .HasName("encode_jobs_pkey");

                entity.ToTable("encode_jobs");

                entity.Property(e => e.Jobid).HasColumnName("jobid");

                entity.Property(e => e.Codec).HasColumnName("codec");

                entity.Property(e => e.Path).HasColumnName("path");

                entity.Property(e => e.Qualitymethod).HasColumnName("qualitymethod");

                entity.Property(e => e.Qualityvalue).HasColumnName("qualityvalue");

                entity.Property(e => e.Status)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("status");

                entity.HasOne(d => d.QualitymethodNavigation)
                    .WithMany(p => p.EncodeJobs)
                    .HasForeignKey(d => d.Qualitymethod)
                    .HasConstraintName("fk_qualitymethod");

                entity.HasOne(d => d.StatusNavigation)
                    .WithMany(p => p.EncodeJobs)
                    .HasForeignKey(d => d.Status)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_status");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
