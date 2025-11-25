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
        public DbSet<Ruta> Rutas => Set<Ruta>();
        public DbSet<BusquedaRuta> BusquedasRuta => Set<BusquedaRuta>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Username único
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Cédula única (dato sensible)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Cedula)
                .IsUnique();

            // Sector -> Campus
            modelBuilder.Entity<Sector>()
                .HasOne(s => s.Campus)
                .WithMany()
                .HasForeignKey(s => s.CampusId)
                .OnDelete(DeleteBehavior.Cascade);

            // Vehículo -> Conductor
            modelBuilder.Entity<Vehiculo>()
                .HasOne(v => v.Conductor)
                .WithMany()
                .HasForeignKey(v => v.ConductorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Placa única en Vehículos (dato sensible)
            modelBuilder.Entity<Vehiculo>()
                .HasIndex(v => v.Placa)
                .IsUnique();

            // Ruta -> Conductor
            modelBuilder.Entity<Ruta>()
                .HasOne(r => r.Conductor)
                .WithMany()
                .HasForeignKey(r => r.ConductorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ruta -> Vehículo
            modelBuilder.Entity<Ruta>()
                .HasOne(r => r.Vehiculo)
                .WithMany()
                .HasForeignKey(r => r.VehiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ruta -> Campus origen
            modelBuilder.Entity<Ruta>()
                .HasOne(r => r.CampusOrigen)
                .WithMany()
                .HasForeignKey(r => r.CampusOrigenId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ruta -> Sector destino
            modelBuilder.Entity<Ruta>()
                .HasOne(r => r.Sector)
                .WithMany()
                .HasForeignKey(r => r.SectorId)
                .OnDelete(DeleteBehavior.Restrict);

            // BusquedaRuta -> Usuario
            modelBuilder.Entity<BusquedaRuta>()
                .HasOne(b => b.Usuario)
                .WithMany()
                .HasForeignKey(b => b.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // BusquedaRuta -> Campus origen
            modelBuilder.Entity<BusquedaRuta>()
                .HasOne(b => b.CampusOrigen)
                .WithMany()
                .HasForeignKey(b => b.CampusOrigenId)
                .OnDelete(DeleteBehavior.Restrict);

            // BusquedaRuta -> Sector destino
            modelBuilder.Entity<BusquedaRuta>()
                .HasOne(b => b.Sector)
                .WithMany()
                .HasForeignKey(b => b.SectorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
