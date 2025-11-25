using System.ComponentModel.DataAnnotations;

namespace WebCrudLogin.Models
{
    public class Ruta
    {
        public int Id { get; set; }

        // Conductor dueño de la ruta
        [Required]
        public int ConductorId { get; set; }
        public User? Conductor { get; set; }

        // Vehículo usado en la ruta
        [Required]
        public int VehiculoId { get; set; }
        public Vehiculo? Vehiculo { get; set; }

        // Campus UDLA desde donde sale la ruta
        [Required]
        public int CampusOrigenId { get; set; }
        public Campus? CampusOrigen { get; set; }

        // Sector de destino (pre-creado por Admin)
        [Required]
        public int SectorId { get; set; }
        public Sector? Sector { get; set; }

        // Cupos que ofrece el conductor
        [Range(1, 8, ErrorMessage = "Los cupos deben estar entre 1 y 8.")]
        public int CuposTotales { get; set; }

        [Range(0, 8)]
        public int CuposOcupados { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime HoraSalida { get; set; }

        [Range(0.1, 500, ErrorMessage = "La distancia debe ser mayor a 0.")]
        public double DistanciaKm { get; set; }

        [Range(0, 9999)]
        public decimal Precio { get; set; }
    }
}
