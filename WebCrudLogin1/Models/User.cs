using System.ComponentModel.DataAnnotations;

namespace WebCrudLogin.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, StringLength(30)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        
        [Required, StringLength(20)]
        public string Role { get; set; } = "User";
    }
}
