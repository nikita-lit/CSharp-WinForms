using Microsoft.EntityFrameworkCore;
using WinForms.CarsService.Models;

namespace WinForms.CarsService
{
    public class CarsServiceDbContext : DbContext
    {
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<CarService> CarServices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Databases/CarsService.db");
            optionsBuilder.UseSqlite($"Data Source={path}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CarService>()
                .HasKey(cs => new { cs.CarId, cs.ServiceId, cs.StartTime });

            modelBuilder.Entity<Car>()
                .HasOne(c => c.Owner)
                .WithMany(o => o.Cars)
                .HasForeignKey(c => c.OwnerId);

            modelBuilder.Entity<CarService>()
                .HasOne(cs => cs.Car)
                .WithMany(c => c.CarServices)
                .HasForeignKey(cs => cs.CarId);

            modelBuilder.Entity<CarService>()
                .HasOne(cs => cs.Service)
                .WithMany(s => s.CarServices)
                .HasForeignKey(cs => cs.ServiceId);
        }
    }
}
