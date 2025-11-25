using System.Collections.Generic;

namespace WebCrudLogin.Models
{
    public class DashboardViewModel
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public bool EsAdmin { get; set; }
        public bool EsConductor { get; set; }

        public List<RutaSugeridaViewModel> RutasSugeridas { get; set; } = new();
    }
}
