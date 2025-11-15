using System.ComponentModel.DataAnnotations;

namespace WebCrudLogin.Models
{
    public class Sector
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        // FK hacia Campus
        [Required]
        public int CampusId { get; set; }
        public Campus? Campus { get; set; }
    }
}

