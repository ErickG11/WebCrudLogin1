using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WebCrudLogin.Data;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// DbContext usando SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));

    // Ignorar warning de múltiples service providers de EF Core
    options.ConfigureWarnings(w => w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning));
});

// Autenticación por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Denied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Crear BD y aplicar migraciones automáticamente al arrancar
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    ctx.Database.Migrate();

    // Seed del usuario Admin 
    if (!ctx.Users.Any(u => u.Username == "admin"))
    {
        ctx.Users.Add(new WebCrudLogin.Models.User
        {
            Username = "admin",
            Cedula = "0102030405", // 10 dígitos para cumplir con Required + Unique
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123"),
            Role = "Admin"
        });
        ctx.SaveChanges();
    }

    // Seed de campuses predefinidos
    if (!ctx.Campuses.Any())
    {
        ctx.Campuses.AddRange(
            new WebCrudLogin.Models.Campus { Nombre = "UDLA Park" },
            new WebCrudLogin.Models.Campus { Nombre = "UDLA Granados" },
            new WebCrudLogin.Models.Campus { Nombre = "UDLA Colón" }
        );
        ctx.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
