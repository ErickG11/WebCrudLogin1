using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WebCrudLogin.Models
{
    public class BuscarRutaViewModel
    {
        [Required]
        [Display(Name = "Campus de salida")]
        public int CampusOrigenId { get; set; }

        [Required]
        [Display(Name = "Sector de destino")]
        public int SectorId { get; set; }

        public List<Ruta> Resultados { get; set; } = new();
    }
}
