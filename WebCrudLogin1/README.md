# CampusRide 

Proyecto de administración MVC que alimenta el core de **CampusRide**, una plataforma para coordinar rutas compartidas entre estudiantes universitarios.

Esta aplicación permite gestionar:

- **Usuarios** con roles: `Admin`, `Conductor`, `User`
- **Vehículos** asociados a conductores
- **Sectores** asociados a campus
- Validación en **back-end** de datos sensibles:
  - Cédula ecuatoriana (usuarios)
  - Placa del vehículo

---


- ASP.NET Core MVC (.NET 8 / .NET 9 según proyecto)
- Entity Framework Core
- SQLite
- Bootstrap 5
- Autenticación por Cookies (`Microsoft.AspNetCore.Authentication.Cookies`)

---


Entidades principales:

- `User`
  - `Id`, `Username`, `Cedula`, `PasswordHash`, `Role`
  - Roles:
    - `Admin`: puede gestionar usuarios, vehículos y sectores
    - `Conductor`: puede gestionar **sus vehículos** y sectores
    - `User`: usuario normal/pasajero

- `Campus`
  - `Id`, `Nombre`

- `Sector`
  - `Id`, `Nombre`, `CampusId`
  - Relación con `Campus` (FK)

- `Vehiculo`
  - `Id`, `Placa`, `Marca`, `Modelo`, `NumeroAsientos`, `ConductorId`
  - Relación con `User` (conductor) (FK)

El `AppDbContext` define los `DbSet` y las restricciones (índices únicos, claves foráneas).

---


- Login y registro:
  - `AccountController`
  - Vistas `Login.cshtml` y `Register.cshtml`
- Cookies de autenticación con `Claims`:
  - `ClaimTypes.Name` = `Username`
  - `ClaimTypes.Role` = `Role`
- Roles:
  - `[Authorize(Roles = "Admin")]` → administración de usuarios
  - `[Authorize(Roles = "Admin,Conductor")]` → sectores y vehículos
  - `[Authorize]` en `HomeController.Index` para el panel principal

---

## Validaciones de datos sensibles 

### 1. Cédula ecuatoriana 

- Propiedad en `User`:

  ```csharp
  [Required]
  [StringLength(10, MinimumLength = 10, ErrorMessage = "La cédula debe tener 10 dígitos.")]
  public string Cedula { get; set; } = string.Empty;
