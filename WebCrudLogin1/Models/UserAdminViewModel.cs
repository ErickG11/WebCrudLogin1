using System.ComponentModel.DataAnnotations;

namespace WebCrudLogin.Models
{
    public class UserAdminViewModel
    {
        public int? Id { get; set; }

        [Required, StringLength(30)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "La cédula debe tener exactamente 10 dígitos.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "La cédula debe contener solo números.")]
        public string Cedula { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Role { get; set; } = "User";
    }
}

