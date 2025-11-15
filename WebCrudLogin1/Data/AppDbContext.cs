using Microsoft.EntityFrameworkCore;
using WebCrudLogin.Models;

namespace WebCrudLogin.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();

        public DbSet<Campus> Campuses => Set<Campus>();
        public DbSet<Sector> Sectores => Set<Sector>();
        public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Sector>()
                .HasOne(s => s.Campus)
                .WithMany()
                .HasForeignKey(s => s.CampusId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vehiculo>()
                .HasOne(v => v.Conductor)
                .WithMany()
                .HasForeignKey(v => v.ConductorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Dato sensible: placa única en BD
            modelBuilder.Entity<Vehiculo>()
                .HasIndex(v => v.Placa)
                .IsUnique();
        }
    }
}
