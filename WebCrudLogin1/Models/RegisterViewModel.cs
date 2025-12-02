using System.ComponentModel.DataAnnotations;

namespace WebCrudLogin.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(30)]
        [Display(Name = "Nombre de usuario")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(10, MinimumLength = 10,
            ErrorMessage = "La cédula debe tener exactamente 10 dígitos.")]
        [RegularExpression(@"^\d{10}$",
            ErrorMessage = "La cédula debe contener solo números (10 dígitos).")]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Tipo de cuenta")]
        public string Role { get; set; } = "User";  // "User" o "Conductor"
    }
}
