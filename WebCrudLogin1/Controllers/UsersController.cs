using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebCrudLogin.Data;
using WebCrudLogin.Models;

namespace WebCrudLogin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .OrderBy(u => u.Username)
                .ToListAsync();

            return View(users);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            CargarRolesDropDown();
            return View(new UserAdminViewModel());
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserAdminViewModel model)
        {
            CargarRolesDropDown(model.Role);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validar que el username NO esté repetido
            bool usernameExiste = await _context.Users
                .AnyAsync(u => u.Username == model.Username);

            if (usernameExiste)
            {
                ModelState.AddModelError(nameof(model.Username), "Ese nombre de usuario ya existe.");
                return View(model);
            }

            // Validar que la cédula NO esté repetida
            bool cedulaExiste = await _context.Users
                .AnyAsync(u => u.Cedula == model.Cedula);

            if (cedulaExiste)
            {
                ModelState.AddModelError(nameof(model.Cedula), "Ya existe un usuario registrado con esta cédula.");
                return View(model);
            }

            // VALIDACIÓN BACK-END DEL DATO SENSIBLE: CÉDULA ECUATORIANA
            if (!CedulaEcuatorianaValida(model.Cedula))
            {
                ModelState.AddModelError(nameof(model.Cedula), "La cédula ingresada no es válida.");
                return View(model);
            }

            var user = new User
            {
                Username = model.Username,
                Cedula = model.Cedula,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = model.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===== Helpers =====

        private void CargarRolesDropDown(string? rolSeleccionado = null)
        {
            // Roles que maneja el sistema:
            // - Admin: administra todo
            // - Conductor: puede crear sectores y vehículos
            // - User: usuario normal/pasajero
            var roles = new List<string> { "Admin", "Conductor", "User" };

            ViewData["Roles"] = new SelectList(
                roles.Select(r => new { Value = r, Text = r }),
                "Value",
                "Text",
                rolSeleccionado
            );
        }

        /// <summary>
        /// Valida una cédula ecuatoriana (10 dígitos, algoritmo oficial).
        /// </summary>
        private bool CedulaEcuatorianaValida(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula))
                return false;

            if (cedula.Length != 10 || !cedula.All(char.IsDigit))
                return false;

            // Código de provincia (01-24 o 30)
            int provincia = int.Parse(cedula.Substring(0, 2));
            if (provincia < 1 || (provincia > 24 && provincia != 30))
                return false;

            // Tercer dígito (para persona natural < 6)
            int tercerDigito = cedula[2] - '0';
            if (tercerDigito >= 6)
                return false;

            // Cálculo del dígito verificador
            int suma = 0;
            for (int i = 0; i < 9; i++)
            {
                int digito = cedula[i] - '0';

                // posiciones impares (1,3,5,7,9) → índices 0,2,4,6,8
                if (i % 2 == 0)
                {
                    digito *= 2;
                    if (digito >= 10)
                        digito -= 9;
                }

                suma += digito;
            }

            int modulo = suma % 10;
            int digitoVerificador = (modulo == 0) ? 0 : 10 - modulo;

            int ultimoDigito = cedula[9] - '0';

            return digitoVerificador == ultimoDigito;
        }
    }
}
