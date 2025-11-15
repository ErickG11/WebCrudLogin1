using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebCrudLogin.Data;
using WebCrudLogin.Models;

namespace WebCrudLogin.Controllers
{
    // Admin y Conductores pueden gestionar Vehículos
    [Authorize(Roles = "Admin,Conductor")]
    public class VehiculosController : Controller
    {
        private readonly AppDbContext _context;

        public VehiculosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Vehiculos
        public async Task<IActionResult> Index()
        {
            IQueryable<Vehiculo> query = _context.Vehiculos
                .Include(v => v.Conductor)
                .OrderBy(v => v.Placa);

            // Si es Conductor (no Admin), solo ve sus propios vehículos
            if (User.IsInRole("Conductor") && !User.IsInRole("Admin"))
            {
                var myId = await GetCurrentUserIdAsync();
                if (myId != null)
                {
                    query = query.Where(v => v.ConductorId == myId.Value);
                }
            }

            var vehiculos = await query.ToListAsync();
            return View(vehiculos);
        }

        // GET: Vehiculos/Create
        public IActionResult Create()
        {
            CargarConductoresDropDown();
            return View();
        }

        // POST: Vehiculos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vehiculo vehiculo)
        {
            if (!ModelState.IsValid)
            {
                CargarConductoresDropDown(vehiculo.ConductorId);
                return View(vehiculo);
            }

            // Si es Conductor (no Admin), se fuerza a que el vehículo sea suyo
            if (User.IsInRole("Conductor") && !User.IsInRole("Admin"))
            {
                var myId = await GetCurrentUserIdAsync();
                if (myId == null)
                {
                    ModelState.AddModelError(string.Empty, "No se pudo identificar al conductor actual.");
                    CargarConductoresDropDown();
                    return View(vehiculo);
                }

                vehiculo.ConductorId = myId.Value;
            }

            // Validar placa duplicada
            bool placaExiste = await _context.Vehiculos
                .AnyAsync(v => v.Placa == vehiculo.Placa);

            if (placaExiste)
            {
                ModelState.AddModelError(nameof(vehiculo.Placa), "Ya existe un vehículo registrado con esta placa.");
                CargarConductoresDropDown(vehiculo.ConductorId);
                return View(vehiculo);
            }

            _context.Add(vehiculo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Vehiculos/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null) return NotFound();

            // Conductor solo edita sus propios vehículos
            if (User.IsInRole("Conductor") && !User.IsInRole("Admin"))
            {
                var myId = await GetCurrentUserIdAsync();
                if (myId == null || vehiculo.ConductorId != myId.Value)
                    return Forbid();
            }

            CargarConductoresDropDown(vehiculo.ConductorId);
            return View(vehiculo);
        }

        // POST: Vehiculos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vehiculo vehiculo)
        {
            if (id != vehiculo.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                CargarConductoresDropDown(vehiculo.ConductorId);
                return View(vehiculo);
            }

            // Si es Conductor (no Admin), se asegura que quede asignado a él
            if (User.IsInRole("Conductor") && !User.IsInRole("Admin"))
            {
                var myId = await GetCurrentUserIdAsync();
                if (myId == null)
                    return Forbid();

                vehiculo.ConductorId = myId.Value;
            }

            // Validar placa duplicada excluyendo el propio vehículo
            bool placaExiste = await _context.Vehiculos
                .AnyAsync(v => v.Placa == vehiculo.Placa && v.Id != vehiculo.Id);

            if (placaExiste)
            {
                ModelState.AddModelError(nameof(vehiculo.Placa), "Ya existe otro vehículo con esta placa.");
                CargarConductoresDropDown(vehiculo.ConductorId);
                return View(vehiculo);
            }

            _context.Update(vehiculo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Vehiculos/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var vehiculo = await _context.Vehiculos
                .Include(v => v.Conductor)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vehiculo == null) return NotFound();

            // Conductor solo borra sus propios vehículos
            if (User.IsInRole("Conductor") && !User.IsInRole("Admin"))
            {
                var myId = await GetCurrentUserIdAsync();
                if (myId == null || vehiculo.ConductorId != myId.Value)
                    return Forbid();
            }

            return View(vehiculo);
        }

        // POST: Vehiculos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo != null)
            {
                if (User.IsInRole("Conductor") && !User.IsInRole("Admin"))
                {
                    var myId = await GetCurrentUserIdAsync();
                    if (myId == null || vehiculo.ConductorId != myId.Value)
                        return Forbid();
                }

                _context.Vehiculos.Remove(vehiculo);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ===== Helper para el dropdown de conductores =====
        private void CargarConductoresDropDown(int? conductorIdSeleccionado = null)
        {
            IQueryable<User> query = _context.Users;

            if (User.IsInRole("Admin"))
            {
                // Admin ve todos los conductores
                query = query.Where(u => u.Role == "Conductor");
            }
            else if (User.IsInRole("Conductor"))
            {
                // Conductor solo se ve a sí mismo
                var username = User.Identity?.Name;
                if (!string.IsNullOrEmpty(username))
                {
                    query = query.Where(u => u.Username == username);
                }
                else
                {
                    query = query.Where(u => false);
                }
            }
            else
            {
                query = query.Where(u => false);
            }

            var conductores = query
                .OrderBy(u => u.Username)
                .Select(u => new
                {
                    u.Id,
                    Nombre = u.Username
                })
                .ToList();

            ViewData["ConductorId"] = new SelectList(conductores, "Id", "Nombre", conductorIdSeleccionado);
        }

        private async Task<int?> GetCurrentUserIdAsync()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return null;

            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Username == username);

            return user?.Id;
        }
    }
}
