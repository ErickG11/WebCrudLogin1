using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebCrudLogin.Data;
using WebCrudLogin.Models;

namespace WebCrudLogin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel();

            if (User.Identity?.IsAuthenticated ?? false)
            {
                vm.NombreUsuario = User.Identity!.Name ?? "";
                vm.EsAdmin = User.IsInRole("Admin");
                vm.EsConductor = User.IsInRole("Conductor");

                if (vm.EsConductor)
                {
                    vm.RutasSugeridas = await ObtenerRutasSugeridasAnualesAsync();
                }
            }

            return View(vm);
        }

        // ===== CORE: rutas más solicitadas en todo el año =====
        private async Task<List<RutaSugeridaViewModel>> ObtenerRutasSugeridasAnualesAsync()
        {
            var anioActual = DateTime.Now.Year;

            // Traemos Todas las búsquedas del año actual,
            // incluyendo Campus y Sector (ya NO usamos SectorDestinoNombre)
            var busquedas = await _context.BusquedasRuta
                .Include(b => b.CampusOrigen)
                .Include(b => b.Sector)
                .Where(b => b.FechaBusqueda.Year == anioActual)
                .ToListAsync();

            // Preparamos los datos para agrupar:
            // Campus + Sector + DíaSemana + Hora
            var datos = busquedas.Select(b => new
            {
                b.CampusOrigenId,
                CampusNombre = b.CampusOrigen != null ? b.CampusOrigen.Nombre : "",
                b.SectorId,
                SectorNombre = b.Sector != null ? b.Sector.Nombre : "",
                DiaSemana = b.FechaBusqueda.DayOfWeek,
                Hora = b.FechaBusqueda.Hour
            });

            var grupos = datos
                .GroupBy(x => new
                {
                    x.CampusOrigenId,
                    x.CampusNombre,
                    x.SectorId,
                    x.SectorNombre,
                    x.DiaSemana,
                    x.Hora
                })
                .OrderByDescending(g => g.Count())   // más buscadas primero
                .Take(5)                             // top 5 rutas sugeridas del año
                .Select(g => new RutaSugeridaViewModel
                {
                    CampusOrigenNombre = g.Key.CampusNombre,
                    SectorDestinoNombre = g.Key.SectorNombre,
                    DiaSemana = TraducirDia(g.Key.DiaSemana),
                    HoraSalidaSugerida = new TimeSpan(g.Key.Hora, 0, 0),
                    TotalBusquedas = g.Count()
                })
                .ToList();

            return grupos;
        }

        // Traducir DayOfWeek a texto en español
        private string TraducirDia(DayOfWeek dia)
        {
            return dia switch
            {
                DayOfWeek.Monday => "Lunes",
                DayOfWeek.Tuesday => "Martes",
                DayOfWeek.Wednesday => "Miércoles",
                DayOfWeek.Thursday => "Jueves",
                DayOfWeek.Friday => "Viernes",
                DayOfWeek.Saturday => "Sábado",
                DayOfWeek.Sunday => "Domingo",
                _ => dia.ToString()
            };
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
