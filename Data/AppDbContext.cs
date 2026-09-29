using Microsoft.EntityFrameworkCore;
using Grading.Models;

namespace Grading.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<ScreeningBatches> ScreeningBatches { get; set; }
        public DbSet<Screening> Screenings { get; set; }
        public DbSet<GradingSummary> GradingSummary { get; set; }

        public DbSet<Grade> Grades { get; set; }
        public DbSet<ProductVarieties> ProductVarieties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScreeningBatches>(entity =>
            {
                entity.ToTable("ScreeningBatches");
                entity.HasKey(e => e.BatchId);
                entity.Property(e => e.BatchDate)
      .HasColumnName("BatchDate")
      .HasColumnType("datetime");

                entity.Property(e => e.From)
                      .HasColumnName("From");

                entity.Property(e => e.VehicleNo)
                      .HasColumnName("Vechicle No");

                entity.Property(e => e.Center)
                      .HasColumnName("Center");

                entity.Property(e => e.HLHot)
                      .HasColumnName("HL/HOT");

                entity.Property(e => e.HLCount)
                      .HasColumnName("HL Count");

                entity.Property(e => e.SamCount)
                      .HasColumnName("Sam. Count");

                entity.Property(e => e.AvgCount)
                      .HasColumnName("Avg. Count")
                      .HasPrecision(18, 2);

                entity.Property(e => e.B2)
                      .HasColumnName("B2");

                entity.Property(e => e.B4)
                      .HasColumnName("B4");

                entity.Property(e => e.ShrimpPerSec)
                      .HasColumnName("Shrimp/Sec")
                      .HasPrecision(18, 2);

                entity.Property(e => e.G2)
                      .HasColumnName("G2");

                entity.Property(e => e.Remarks)
                      .HasColumnName("Remarks");
            });

            modelBuilder.Entity<Screening>()
                .HasOne(s => s.ScreeningBatches)
                .WithMany(b => b.Screenings)
                .HasForeignKey(s => s.BatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Screening>()
                .HasIndex(s => new { s.BatchId, s.BIN })
                .IsUnique();

            modelBuilder.Entity<Screening>()
                .Property(s => s.From)

                .HasPrecision(18, 2);

            modelBuilder.Entity<Screening>()
                .Property(s => s.To)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Screening>()
               .Property(s => s.Manual)
               .HasPrecision(18, 2);

            modelBuilder.Entity<Screening>()
                .Property(s => s.Screen)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Screening>()
                .Property(s => s.BS)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ScreeningBatches>(entity =>
            {
                entity.Property(e => e.SamCount).HasPrecision(18, 2);
                entity.Property(e => e.AvgCount).HasPrecision(18, 2);
                entity.Property(e => e.B2).HasPrecision(18, 2);
                entity.Property(e => e.B4).HasPrecision(18, 2);
                entity.Property(e => e.G2).HasPrecision(18, 2);
                entity.Property(e => e.ShrimpPerSec).HasPrecision(18, 2);
            });


            base.OnModelCreating(modelBuilder);
        }
    }
}
