using System.ComponentModel.DataAnnotations;

namespace WebCrudLogin.Models
{
    public class BusquedaRuta
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }
        public User? Usuario { get; set; }

        [Required]
        public int CampusOrigenId { get; set; }
        public Campus? CampusOrigen { get; set; }

        [Required]
        public int SectorId { get; set; }
        public Sector? Sector { get; set; }

        [Required]
        public DateTime FechaBusqueda { get; set; }
    }
}
