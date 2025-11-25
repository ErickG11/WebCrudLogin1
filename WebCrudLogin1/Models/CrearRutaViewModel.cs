using System.ComponentModel.DataAnnotations;

namespace WebCrudLogin.Models
{
    public class CrearRutaViewModel
    {
        // ===== Datos de la ruta =====

        [Required]
        [Display(Name = "Campus de salida")]
        public int CampusOrigenId { get; set; }

        [Required]
        [Display(Name = "Sector de destino")]
        public int SectorId { get; set; }

        [Range(1, 8, ErrorMessage = "Los cupos deben estar entre 1 y 8.")]
        [Display(Name = "Cupos totales")]
        public int CuposTotales { get; set; }

        [Required]
        [Display(Name = "Día de la semana")]
        public DayOfWeek DiaSemana { get; set; } = DayOfWeek.Monday;

        [Required]
        [Display(Name = "Hora de salida")]
        public TimeSpan HoraDelDia { get; set; } = new TimeSpan(8, 0, 0);

        [Range(0.1, 500, ErrorMessage = "La distancia debe ser mayor a 0.")]
        [Display(Name = "Distancia aproximada (km)")]
        public double DistanciaKm { get; set; }

        // ===== Datos del vehículo =====

        [Required(ErrorMessage = "La placa es obligatoria.")]
        [RegularExpression("^[A-Z]{3}-?[0-9]{3,4}$",
            ErrorMessage = "La placa debe tener el formato ABC-123 o ABC-1234.")]
        [StringLength(10)]
        [Display(Name = "Placa")]
        public string Placa { get; set; } = string.Empty;

        [Required, StringLength(50)]
        [Display(Name = "Marca")]
        public string Marca { get; set; } = string.Empty;

        [Required, StringLength(50)]
        [Display(Name = "Modelo")]
        public string Modelo { get; set; } = string.Empty;

        [Range(1, 8, ErrorMessage = "El número de asientos debe estar entre 1 y 8.")]
        [Display(Name = "Número de asientos")]
        public int NumeroAsientos { get; set; }
    }
}
