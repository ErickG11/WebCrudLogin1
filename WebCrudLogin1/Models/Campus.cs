using System.ComponentModel.DataAnnotations;

namespace WebCrudLogin.Models
{
    public class Campus
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

    }
}
