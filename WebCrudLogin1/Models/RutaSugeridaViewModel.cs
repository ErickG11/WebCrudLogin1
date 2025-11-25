namespace WebCrudLogin.Models
{
    public class RutaSugeridaViewModel
    {
        public string CampusOrigenNombre { get; set; } = string.Empty;
        public string SectorDestinoNombre { get; set; } = string.Empty;

        // Nuevo: día de la semana en el que más se busca esta ruta
        public string DiaSemana { get; set; } = string.Empty;

        // Hora del día en la que más se busca
        public TimeSpan HoraSalidaSugerida { get; set; }

        // Número total de búsquedas de esa combinación en el año
        public int TotalBusquedas { get; set; }
    }
}
