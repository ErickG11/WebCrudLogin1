using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebCrudLogin.Data;
using WebCrudLogin.Models;

namespace WebCrudLogin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SectoresController : Controller
    {
        private readonly AppDbContext _context;

        public SectoresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Sectores
        public async Task<IActionResult> Index()
        {
            var sectores = await _context.Sectores
                .Include(s => s.Campus)
                .OrderBy(s => s.Campus!.Nombre)
                .ThenBy(s => s.Nombre)
                .ToListAsync();

            return View(sectores);
        }

        // GET: Sectores/Create
        public IActionResult Create()
        {
            ViewData["CampusId"] = new SelectList(_context.Campuses.OrderBy(c => c.Nombre), "Id", "Nombre");
            return View();
        }

        // POST: Sectores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sector sector)
        {
            if (!ModelState.IsValid)
            {
                ViewData["CampusId"] = new SelectList(_context.Campuses.OrderBy(c => c.Nombre), "Id", "Nombre", sector.CampusId);
                return View(sector);
            }

            _context.Add(sector);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Sectores/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var sector = await _context.Sectores.FindAsync(id);
            if (sector == null) return NotFound();

            ViewData["CampusId"] = new SelectList(_context.Campuses.OrderBy(c => c.Nombre), "Id", "Nombre", sector.CampusId);
            return View(sector);
        }

        // POST: Sectores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Sector sector)
        {
            if (id != sector.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewData["CampusId"] = new SelectList(_context.Campuses.OrderBy(c => c.Nombre), "Id", "Nombre", sector.CampusId);
                return View(sector);
            }

            _context.Update(sector);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Sectores/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var sector = await _context.Sectores
                .Include(s => s.Campus)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sector == null) return NotFound();
            return View(sector);
        }

        // POST: Sectores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sector = await _context.Sectores.FindAsync(id);
            if (sector != null)
            {
                _context.Sectores.Remove(sector);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
