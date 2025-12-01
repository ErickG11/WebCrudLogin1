using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        [Required]
        [Display(Name = "Día de la semana")]
        public DayOfWeek DiaSemana { get; set; } = DayOfWeek.Monday;

        [Required]
        [Display(Name = "Hora aproximada de salida")]
        public TimeSpan HoraDelDia { get; set; } = new TimeSpan(8, 0, 0);

        public List<Ruta> Resultados { get; set; } = new();
    }
}
