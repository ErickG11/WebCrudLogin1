using System.ComponentModel.DataAnnotations;

namespace WebCrudLogin.Models
{
    public class RegisterViewModel
    {
        [Required, StringLength(30)]
        public string Username { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
