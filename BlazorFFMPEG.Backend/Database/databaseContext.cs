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

        private static string connectionString;
        
        public databaseContext(string connectionString_)
        {
            connectionString = connectionString_;
        }
        
        public virtual DbSet<AutoEncodeFolder> AutoEncodeFolders { get; set; } = null!;
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
            modelBuilder.Entity<AutoEncodeFolder>(entity =>
            {
                entity.HasKey(e => e.Folderid)
                    .HasName("auto_encode_folder_pkey");

                entity.ToTable("auto_encode_folder");

                entity.Property(e => e.Folderid).HasColumnName("folderid");

                entity.Property(e => e.Inputpath).HasColumnName("inputpath");

                entity.Property(e => e.Outputpath).HasColumnName("outputpath");
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

                entity.Property(e => e.Isautoencodejob)
                    .HasColumnName("isautoencodejob")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Path).HasColumnName("path");

                entity.Property(e => e.Qualitymethod).HasColumnName("qualitymethod");

                entity.Property(e => e.Qualityvalue).HasColumnName("qualityvalue");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("0");

                entity.HasOne(d => d.StatusNavigation)
                    .WithMany(p => p.EncodeJobs)
                    .HasForeignKey(d => d.Status)
                    .HasConstraintName("fk_status");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
