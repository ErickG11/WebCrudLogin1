using System.ComponentModel.DataAnnotations;

namespace WebCrudLogin.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, StringLength(30)]
        public string Username { get; set; } = string.Empty;

        // DATO SENSIBLE: cédula ecuatoriana
        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "La cédula debe tener 10 dígitos.")]
        public string Cedula { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Roles: Admin, Conductor, User
        [Required, StringLength(20)]
        public string Role { get; set; } = "User";
    }
}

