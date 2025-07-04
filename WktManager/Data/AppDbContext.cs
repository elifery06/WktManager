using Microsoft.EntityFrameworkCore;
using WktManager.Models;

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
                entity.ToTable("WktCoordinates");      // Veritabanındaki tablo adı
                entity.HasKey(e => e.Id);               // Birincil anahtar
                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(100);               // Name zorunlu ve max 100 karakter
                entity.Property(e => e.WKT)
                      .IsRequired();                    // WKT alanı zorunlu
            });

            
        }

    }
}
