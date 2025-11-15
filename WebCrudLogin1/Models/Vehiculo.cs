using System.ComponentModel.DataAnnotations;

namespace WebCrudLogin.Models
{
    public class Vehiculo
    {
        public int Id { get; set; }

        // DATO SENSIBLE: placa
        [Required(ErrorMessage = "La placa es obligatoria.")]
        [RegularExpression("^[A-Z]{3}-?[0-9]{3,4}$",
            ErrorMessage = "La placa debe tener el formato ABC-123 o ABC-1234.")]
        [StringLength(10)]
        public string Placa { get; set; } = string.Empty;

        [Required(ErrorMessage = "La marca es obligatoria.")]
        [StringLength(50)]
        public string Marca { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Modelo { get; set; } = string.Empty;

        [Range(1, 8, ErrorMessage = "El número de asientos debe estar entre 1 y 8.")]
        public int NumeroAsientos { get; set; }

        // Relación con User que actúa como conductor
        [Required]
        public int ConductorId { get; set; }
        public User? Conductor { get; set; }
    }
}
