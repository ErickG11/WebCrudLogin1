using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebCrudLogin.Data;
using WebCrudLogin.Models;

namespace WebCrudLogin.Controllers
{
    [Authorize(Roles = "Admin")]
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
            var vehiculos = await _context.Vehiculos
                .Include(v => v.Conductor)
                .OrderBy(v => v.Placa)
                .ToListAsync();

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
            // VALIDACIÓN EN BACK-END DEL DATO SENSIBLE (PLACA)

            // 1) Validaciones por data annotations (formato, required, etc.)
            if (!ModelState.IsValid)
            {
                CargarConductoresDropDown(vehiculo.ConductorId);
                return View(vehiculo);
            }

            // 2) Validar en servidor que la placa NO esté repetida en la BD
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

            // Validar placa duplicada, excluyendo el propio vehículo
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
                _context.Vehiculos.Remove(vehiculo);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ===== Helper para el dropdown de conductores =====
        private void CargarConductoresDropDown(int? conductorIdSeleccionado = null)
        {
            // Aquí podrías filtrar por Role = "User" o "Conductor"
            var conductores = _context.Users
                .OrderBy(u => u.Username)
                .Select(u => new
                {
                    u.Id,
                    Nombre = u.Username + " (" + u.Role + ")"
                })
                .ToList();

            ViewData["ConductorId"] = new SelectList(conductores, "Id", "Nombre", conductorIdSeleccionado);
        }
    }
}
