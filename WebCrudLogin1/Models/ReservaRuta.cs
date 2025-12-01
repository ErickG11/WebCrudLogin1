using System;
using System.ComponentModel.DataAnnotations;

namespace WebCrudLogin.Models
{
    public class ReservaRuta
    {
        public int Id { get; set; }

        [Required]
        public int RutaId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        public DateTime FechaUnion { get; set; } = DateTime.Now;
    }
}
