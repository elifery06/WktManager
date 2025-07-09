using Microsoft.EntityFrameworkCore;
using WktManager.Models;
using NetTopologySuite;

namespace WktManager.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<WktCoordinate> WktCoordinates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WktCoordinate>(entity =>
            {
                entity.ToTable("WktCoordinates");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.WKT)
                      .HasColumnName("WKT")
                      .HasColumnType("geometry")   // PostGIS tipi geometry
                      .IsRequired();
            });
        }
    }
}
