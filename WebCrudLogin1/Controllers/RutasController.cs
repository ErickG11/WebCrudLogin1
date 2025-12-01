using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebCrudLogin.Data;
using WebCrudLogin.Models;

namespace WebCrudLogin.Controllers
{
    public class RutasController : Controller
    {
        private readonly AppDbContext _context;

        public RutasController(AppDbContext context)
        {
            _context = context;
        }

        // ========== LISTA DE RUTAS DEL CONDUCTOR ==========

        [Authorize(Roles = "Conductor,Admin")]
        public async Task<IActionResult> Index()
        {
            var username = User.Identity!.Name!;
            var conductor = await _context.Users
                .SingleAsync(u => u.Username == username);

            var rutas = await _context.Rutas
                .Include(r => r.CampusOrigen)
                .Include(r => r.Sector)
                .Include(r => r.Vehiculo)
                .Where(r => r.ConductorId == conductor.Id)
                .OrderBy(r => r.HoraSalida)
                .ToListAsync();

            return View(rutas);
        }

        // ========== CREAR RUTA ==========

        [Authorize(Roles = "Conductor,Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CargarDropdowns();
            return View(new CrearRutaViewModel());
        }

        [Authorize(Roles = "Conductor,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearRutaViewModel vm)
        {
            await CargarDropdowns();

            var username = User.Identity!.Name!;
            var conductor = await _context.Users
                .SingleAsync(u => u.Username == username);

            // Validación: cupos no pueden ser mayores que asientos
            if (vm.CuposTotales > vm.NumeroAsientos)
            {
                ModelState.AddModelError(nameof(vm.CuposTotales),
                    "Los cupos totales no pueden ser mayores que el número de asientos del vehículo.");
            }

            // Validación: sólo días de lunes a sábado
            if (vm.DiaSemana == DayOfWeek.Sunday)
            {
                ModelState.AddModelError(nameof(vm.DiaSemana),
                    "Solo se permiten rutas de lunes a sábado.");
            }

            // Validación: hora entre 08:00 y 23:00
            if (vm.HoraDelDia < new TimeSpan(8, 0, 0) || vm.HoraDelDia > new TimeSpan(23, 0, 0))
            {
                ModelState.AddModelError(nameof(vm.HoraDelDia),
                    "La hora debe estar entre las 08:00 y las 23:00.");
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            // Buscar si ya existe un vehículo con esa placa
            var placaMayus = vm.Placa.Trim().ToUpper();
            var vehiculo = await _context.Vehiculos
                .SingleOrDefaultAsync(v => v.Placa == placaMayus);

            if (vehiculo != null && vehiculo.ConductorId != conductor.Id)
            {
                ModelState.AddModelError(nameof(vm.Placa),
                    "Esta placa ya está registrada por otro conductor.");
                return View(vm);
            }

            if (vehiculo == null)
            {
                vehiculo = new Vehiculo
                {
                    Placa = placaMayus,
                    Marca = vm.Marca,
                    Modelo = vm.Modelo,
                    NumeroAsientos = vm.NumeroAsientos,
                    ConductorId = conductor.Id
                };

                _context.Vehiculos.Add(vehiculo);
                await _context.SaveChangesAsync();
            }

            // Calcular la próxima fecha que cae en ese día de la semana
            var proximaFecha = ObtenerProximaFecha(vm.DiaSemana);
            var horaSalida = proximaFecha.Date + vm.HoraDelDia;

            var ruta = new Ruta
            {
                ConductorId = conductor.Id,
                VehiculoId = vehiculo.Id,
                CampusOrigenId = vm.CampusOrigenId,
                SectorId = vm.SectorId,
                CuposTotales = vm.CuposTotales,
                CuposOcupados = 0,
                HoraSalida = horaSalida,
                DistanciaKm = vm.DistanciaKm,
                Precio = (decimal)vm.DistanciaKm * 0.90m
            };

            _context.Rutas.Add(ruta);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ========== 3) EDITAR RUTA ==========

        [Authorize(Roles = "Conductor,Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var ruta = await _context.Rutas
                .Include(r => r.CampusOrigen)
                .Include(r => r.Sector)
                .Include(r => r.Vehiculo)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (ruta == null) return NotFound();

            // Solo el dueño de la ruta o Admin
            if (!User.IsInRole("Admin"))
            {
                var username = User.Identity!.Name!;
                var conductor = await _context.Users.SingleAsync(u => u.Username == username);
                if (ruta.ConductorId != conductor.Id) return Forbid();
            }

            await CargarDropdowns();
            return View(ruta);
        }

        [Authorize(Roles = "Conductor,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ruta ruta)
        {
            if (id != ruta.Id) return BadRequest();

            await CargarDropdowns();

            var rutaDb = await _context.Rutas
                .Include(r => r.Vehiculo)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rutaDb == null) return NotFound();

            // Validar dueño si no es Admin
            if (!User.IsInRole("Admin"))
            {
                var username = User.Identity!.Name!;
                var conductor = await _context.Users.SingleAsync(u => u.Username == username);
                if (rutaDb.ConductorId != conductor.Id) return Forbid();
            }

            // Validación: no reducir cupos por debajo de los ya ocupados
            if (ruta.CuposTotales < rutaDb.CuposOcupados)
            {
                ModelState.AddModelError(nameof(ruta.CuposTotales),
                    "No puedes poner cupos totales menores a los cupos ya ocupados.");
            }

            if (!ModelState.IsValid)
            {
                return View(ruta);
            }

            // Actualizar campos editables
            rutaDb.CampusOrigenId = ruta.CampusOrigenId;
            rutaDb.SectorId = ruta.SectorId;
            rutaDb.CuposTotales = ruta.CuposTotales;
            rutaDb.HoraSalida = ruta.HoraSalida;
            rutaDb.DistanciaKm = ruta.DistanciaKm;
            rutaDb.Precio = (decimal)ruta.DistanciaKm * 0.90m; // recalcular precio

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ========== 4) ELIMINAR RUTA ==========

        [Authorize(Roles = "Conductor,Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var ruta = await _context.Rutas
                .Include(r => r.CampusOrigen)
                .Include(r => r.Sector)
                .Include(r => r.Vehiculo)
                .Include(r => r.Conductor)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (ruta == null) return NotFound();

            // Solo el dueño o Admin
            if (!User.IsInRole("Admin"))
            {
                var username = User.Identity!.Name!;
                var conductor = await _context.Users.SingleAsync(u => u.Username == username);
                if (ruta.ConductorId != conductor.Id) return Forbid();
            }

            return View(ruta);
        }

        [Authorize(Roles = "Conductor,Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ruta = await _context.Rutas.FindAsync(id);
            if (ruta == null) return NotFound();

            if (!User.IsInRole("Admin"))
            {
                var username = User.Identity!.Name!;
                var conductor = await _context.Users.SingleAsync(u => u.Username == username);
                if (ruta.ConductorId != conductor.Id) return Forbid();
            }

            _context.Rutas.Remove(ruta);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ========== 5) BUSCAR RUTAS (USUARIO NORMAL) ==========

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Buscar()
        {
            var vm = new BuscarRutaViewModel();
            await CargarDropdowns();
            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buscar(BuscarRutaViewModel vm)
        {
            await CargarDropdowns();

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            // Registrar la búsqueda en la BD
            var username = User.Identity!.Name!;
            var usuario = await _context.Users
                .SingleAsync(u => u.Username == username);

            var busqueda = new BusquedaRuta
            {
                UsuarioId = usuario.Id,
                CampusOrigenId = vm.CampusOrigenId,
                SectorId = vm.SectorId,
                FechaBusqueda = DateTime.Now
            };

            _context.BusquedasRuta.Add(busqueda);
            await _context.SaveChangesAsync();

            // Calcular rango horario ± 2 horas
            var horaBuscada = vm.HoraDelDia;
            var desde = horaBuscada - TimeSpan.FromHours(2);
            var hasta = horaBuscada + TimeSpan.FromHours(2);

            if (desde < TimeSpan.Zero) desde = TimeSpan.Zero;
            if (hasta > new TimeSpan(23, 59, 59)) hasta = new TimeSpan(23, 59, 59);

            var ahora = DateTime.Now;

            // 1) Filtro que SÍ puede ir a la BD
            var rutasDb = await _context.Rutas
                .Include(r => r.Conductor)
                .Include(r => r.Vehiculo)
                .Include(r => r.CampusOrigen)
                .Include(r => r.Sector)
                .Where(r =>
                    r.CampusOrigenId == vm.CampusOrigenId &&
                    r.SectorId == vm.SectorId &&
                    r.CuposOcupados < r.CuposTotales &&
                    r.HoraSalida > ahora)
                .ToListAsync();

            // 2) Filtro en memoria por día de la semana y rango horario
            var rutas = rutasDb
                .Where(r =>
                    r.HoraSalida.DayOfWeek == vm.DiaSemana &&
                    r.HoraSalida.TimeOfDay >= desde &&
                    r.HoraSalida.TimeOfDay <= hasta)
                .OrderBy(r => r.HoraSalida)
                .ToList();

            vm.Resultados = rutas;
            return View(vm);
        }


        // ========== 6) UNIRSE A UNA RUTA ==========

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unirse(int id)
        {
            var ruta = await _context.Rutas
                .Include(r => r.Vehiculo)
                .Include(r => r.CampusOrigen)
                .Include(r => r.Sector)
                .Include(r => r.Conductor)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (ruta == null) return NotFound();

            if (ruta.CuposOcupados >= ruta.CuposTotales)
            {
                TempData["JoinMessage"] = "Esta ruta ya está llena.";
            }
            else
            {
                ruta.CuposOcupados++;
                await _context.SaveChangesAsync();

                TempData["JoinMessage"] =
                    $"Te uniste a la ruta. Ahora hay {ruta.CuposOcupados} de {ruta.CuposTotales} puestos ocupados.";
            }

            return RedirectToAction(nameof(Buscar));
        }

        // ========== HELPERS ==========

        private async Task CargarDropdowns()
        {
            var campuses = await _context.Campuses
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            var sectores = await _context.Sectores
                .Include(s => s.Campus)
                .OrderBy(s => s.Campus!.Nombre)
                .ThenBy(s => s.Nombre)
                .ToListAsync();

            ViewData["CampusOrigenId"] = new SelectList(campuses, "Id", "Nombre");
            ViewData["SectorId"] = new SelectList(
                sectores.Select(s => new
                {
                    s.Id,
                    NombreCompleto = s.Campus!.Nombre + " - " + s.Nombre
                }),
                "Id",
                "NombreCompleto"
            );

            // Dropdown de horas: de 08:00 a 23:00
            var horas = Enumerable.Range(8, 16)
                .Select(h => new
                {
                    Valor = $"{h:00}:00",
                    Texto = $"{h:00}:00"
                })
                .ToList();

            ViewData["HoraDelDia"] = new SelectList(horas, "Valor", "Texto");
        }

        private DateTime ObtenerProximaFecha(DayOfWeek dia)
        {
            var hoy = DateTime.Today;
            int diasDiferencia = ((int)dia - (int)hoy.DayOfWeek + 7) % 7;
            if (diasDiferencia == 0)
            {
                // Si es el mismo día, lo mandamos a la próxima semana
                diasDiferencia = 7;
            }
            return hoy.AddDays(diasDiferencia);
        }
    }
}
