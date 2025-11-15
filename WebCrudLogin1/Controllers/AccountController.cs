using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebCrudLogin.Models;
using WebCrudLogin.Data;
using Microsoft.EntityFrameworkCore;

namespace WebCrudLogin.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        public AccountController(AppDbContext context) => _context = context;

        // ====== REGISTER ======
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var exists = await _context.Users.AnyAsync(u => u.Username == model.Username);
            if (exists)
            {
                ModelState.AddModelError(nameof(model.Username), "Ese usuario ya existe.");
                return View(model);
            }

            var user = new User
            {
                Username = model.Username,
                // Si quieres, aquí podrías pedir cédula al registrarse normal, pero en este diseño
                // el Admin crea usuarios con cédula. Así que usamos un valor temporal vacío.
                Cedula = "0000000000",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = "User" // usuarios nuevos = rol User
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await SignIn(user.Username, user.Role);

            // Después de registrarse, ir al Home
            return RedirectToAction("Index", "Home");
        }

        // ====== LOGIN ======
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
            => View(new LoginViewModel { ReturnUrl = returnUrl });

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == model.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña inválidos.");
                return View(model);
            }

            await SignIn(user.Username, user.Role);

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            // Después de iniciar sesión, ir siempre al Home
            return RedirectToAction("Index", "Home");
        }

        // ====== LOGOUT ======
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Logout(string? returnUrl = null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Login", "Account");
        }

        // ====== ACCESS DENIED opcional ======
        [AllowAnonymous]
        public IActionResult Denied() => Content("Acceso denegado");

        // ====== helper ======
        private async Task SignIn(string username, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
