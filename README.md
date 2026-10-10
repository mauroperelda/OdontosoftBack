# OdontoSoft — Backend (.NET)

> Solución backend para la gestión integral de un consultorio odontológico: historia clínica, turnos, presupuestos, pagos, odontograma, periodontograma e imágenes del paciente, con autenticación JWT, autorización por roles/permisos (RBAC) y arquitectura en capas sobre .NET.

> **Repos relacionados**
>
> - **Frontend (este sistema):** [`odontosoft-front`](https://github.com/mauroperelda/odontosoft-front) — SPA React + Vite que consume la `OdontoSoft.API` vía JWT.
> - **API de identidad (sister-repo):** [`ApiGetToken`](https://github.com/mauroperelda/ApiGetToken) — emite y valida los tokens JWT con BCrypt + RBAC.
>
> Las dos APIs del backend (`OdontoSoft.API` y `ApiGetToken`) comparten la misma `Jwt:Key` / `Jwt:Issuer` / `Jwt:Audience`, por lo que un token emitido por la API de identidad es aceptado por la API de negocio sin re-login.

---

## 1. Resumen

- **Dominio**: software de gestión para clínica odontológica (pacientes, profesionales, turnos, tratamientos, presupuestos, pagos, historia clínica, odontograma y periodontograma).
- **Stack**: **C# / .NET 9–10**, **ASP.NET Core Web API**, **ASP.NET Core MVC (Razor)**, **Entity Framework Core 9** sobre **SQL Server**.
- **Arquitectura**: monorepo en capas con **Repository + Unit of Work**, **DTOs / ViewModels**, **servicios de dominio**, **API REST** desacoplada y **frontend MVC Razor** con **DevExtreme** (jQuery + Bootstrap) — todo detrás de un único sistema de autenticación **JWT con BCrypt** y **RBAC** (Roles + Permisos) consumido por los dos frontends.
- **Seguridad**: hashing de contraseñas con **BCrypt** (workFactor 11), **JWT Bearer** con clave HMAC-SHA256, validación de `Issuer`/`Audience`/`Lifetime`, claims de identidad + roles + permisos efectivos firmados dentro del token, y **soft-delete + auditoría** (created/updated, user, fecha) en todas las entidades de negocio.
- **Escalabilidad**: paginación y filtros server-side (`PaginationParams` + `PagedResult<T>`), stored procedures para búsquedas específicas (ej. `GetPacienteForTurno`), CORS abierto para frontends externos (React/Vite) y tunneled (trycloudflare), Swagger/OpenAPI documentado.

---

## 2. Arquitectura

La solución está organizada como **dos repositorios independientes** que comparten la misma autoridad de identidad:

```
OdontosoftBack/                 ← este repositorio (.NET)
├── OdontoSoft.API/             ← Web API REST (net10.0) — backend principal
├── OdontoSoft.Web/             ← App MVC Razor (net9.0) — panel admin (legacy, coexistente)
├── OdontoSoft.Data/            ← EF Core, DbContext, Repositorios, UoW
├── OdontoSoft.Entities/        ← Modelos de dominio + ViewModels + BaseEntity
└── Odontosoft.Services/        ← Servicios de dominio (lógica de negocio)

ApiGetToken/                    ← repositorio hermano (.NET 10) — identidad + RBAC
├── Controllers/                ← AuthController, AccountController, Usuarios/Roles/Permisos
├── DATA/                       ← ManagerOdontosoftContext (EF Core)
├── Models/                     ← User, Rol, Permiso, UsuarioRol, RolPermiso
├── Helpers/Authentication/     ← AuthenticateUser, UserCreator (BCrypt)
├── Helpers/GenerateToken/      ← GenerateToken (JWT con claims + RBAC)
└── Migrations/                 ← EF Core migrations (incluye AddRBAC)
```

### Diagrama lógico

```
┌──────────────────────┐        ┌──────────────────────┐
│  OdontoSoft.Web      │        │  odontosoft-front    │
│  (MVC Razor,         │        │  (React + Vite, SPA) │
│   DevExtreme UI)     │        │                      │
└──────────┬───────────┘        └──────────┬───────────┘
           │ cookie auth                    │ JWT Bearer
           ▼                                ▼
┌──────────────────────┐        ┌──────────────────────┐
│  OdontoSoft.API      │ ◄────  │  ApiGetToken         │
│  (REST, [Authorize]) │  JWT   │  /api/Account/Login  │
│  Controllers + UoW   │  share │  /api/Auth/Me        │
└──────────┬───────────┘        └──────────┬───────────┘
           │ EF Core                       │ EF Core
           ▼                               ▼
   ┌───────────────┐                ┌─────────────────┐
   │  SQL Server   │                │  SQL Server     │
   │  Odontosoft   │                │ MANAGER_ODONTOSOFT│
   └───────────────┘                └─────────────────┘
```

### Patrones aplicados

- **Layered architecture**: Controllers → Services → Repositories (UoW) → DbContext → DB.
- **Repository genérico + Unit of Work** (`IGenericRepository<T>`, `IUnitOfWork`) — las transacciones se confirman con `UnitOfWork.Save()`.
- **DTOs / ViewModels separados de entidades** (`PacienteViewModel`, `LoginUserRequest/Response`) para no exponer el modelo de EF.
- **Soft-delete + auditoría automática** vía `AuditedEntity` y `SaveChanges()` override que setea `CreatedDate/UpdatedDate/CreatedUserId/UpdatedUserId/Disabled` desde `IAuthenticatedUser` (resuelto por el JWT).
- **Inversión de dependencias** vía `Microsoft.Extensions.DependencyInjection` (`AddScoped`/`AddTransient`).

---

## 3. Stack tecnológico

| Capa           | Tecnología                                                                                                                                              |
| -------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Lenguaje       | **C# 12 / .NET 9 y .NET 10**                                                                                                                            |
| Backend API    | **ASP.NET Core 10 Web API**, controllers, `[Authorize]`, middleware custom                                                                              |
| Frontend admin | **ASP.NET Core 9 MVC** (Razor), `AddRazorRuntimeCompilation`, jQuery, **DevExtreme** 24.2                                                               |
| ORM            | **Entity Framework Core 9 / 10** con `Microsoft.EntityFrameworkCore.SqlServer`                                                                          |
| Migrations     | **EF Core Migrations** (`Add-Migration`, `Update-Database`)                                                                                             |
| Autenticación  | **JWT Bearer** (`Microsoft.AspNetCore.Authentication.JwtBearer`, `Microsoft.IdentityModel.Tokens`)                                                      |
| Hashing        | **BCrypt.Net-Next** (workFactor 11)                                                                                                                     |
| Validación     | `DataAnnotations` + `InvalidModelStateResponseFactory` custom (errores en español)                                                                      |
| Mapping        | **AutoMapper 16.1.1** (en MVC), mapeos manuales en Web API                                                                                              |
| Documentación  | **Swashbuckle / Swagger UI**                                                                                                                            |
| Base de datos  | **Microsoft SQL Server** (Express / LocalDB)                                                                                                            |
| Otros          | `Newtonsoft.Json`, `Identity.EntityFrameworkCore` (Web), `DevExtreme.AspNet.Data 5.1.0`, `Microsoft.AspNetCore.Mvc.NewtonsoftJson`, `jQuery Validation` |

---

## 4. Sistema de login / autenticación

La autenticación está centralizada en la API hermana **`ApiGetToken`** y se consume desde el panel MVC (vía cookie re-emitido) y desde la **`OdontoSoft.API`** (vía JWT Bearer). **Ambas APIs comparten exactamente la misma `Jwt:Key` / `Jwt:Issuer` / `Jwt:Audience`**, por lo que un token emitido por `ApiGetToken` es aceptado por `OdontoSoft.API` sin re-login.

### 4.1. Hashing de contraseñas — BCrypt

- Algoritmo: **BCrypt** con `workFactor = 11` (cada hash es único por su salt aleatorio).
- Implementación: `BCrypt.Net.BCrypt.HashPassword(plain, workFactor: 11)` y `BCrypt.Net.BCrypt.Verify(plain, hash)`.
- **Migración lazy de contraseñas legadas**: si el campo `Password` no comienza con `$2…$` (formato BCrypt), se compara en plano una sola vez; si coincide, se re-hashea con BCrypt y se persiste, de modo que el próximo login ya entra por la ruta BCrypt.

### 4.2. Flujo de login

```
[POST /api/Account/Login] { username, password }
   │
   ▼
AuthenticateUser.Authenticate()
   • Normaliza username/email a minúsculas (case-insensitive)
   • Busca User donde (Username == id || Email == id) AND !Disabled
   • VerifyAndMigratePassword()  →  BCrypt.Verify (o upgrade + persist)
   • Chequea Activated = true
   │
   ▼
GenerateToken.Generate(user)
   • Claims: Name, Nombre, Apellido, Email, Id
   • Claims legacy: administrator, superuser
   • Claims RBAC:  Rol = <código>, Permiso = <código>   (desde DB, 1 query)
   • Firma:  HS256, Issuer, Audience, expira en 8 h
   │
   ▼
200 OK { result: true, message: "OK", token, userData }
```

### 4.3. Validación de tokens en `OdontoSoft.API`

```csharp
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true, ValidateAudience = true,
        ValidateLifetime = true, ValidateIssuerSigningKey = true,
        ValidIssuer   = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});
```

Cada controller de la API lleva `[Authorize]` y obtiene la identidad vía `IAuthenticatedUser` (que se construye a partir de los claims del JWT, no de la cookie de sesión).

### 4.4. Endpoints de identidad

| Método              | Ruta                    | Auth | Descripción                                                                  |
| ------------------- | ----------------------- | ---- | ---------------------------------------------------------------------------- |
| POST                | `/api/Account/Login`    | —    | Autentica con `username + password`, devuelve JWT + datos del usuario.       |
| POST                | `/api/Account/Register` | —    | Registro self-service (ignora `RolIds` por seguridad).                       |
| GET                 | `/api/Auth/Me`          | JWT  | Devuelve el usuario actual con roles y permisos efectivos (re-leídos de DB). |
| GET/POST/PUT/DELETE | `/api/Usuarios`         | JWT  | CRUD administrativo de usuarios + asignación de roles.                       |
| GET/POST/PUT/DELETE | `/api/Roles`            | JWT  | CRUD de roles + asignación de permisos.                                      |
| GET/POST/PUT/DELETE | `/api/Permisos`         | JWT  | CRUD de permisos.                                                            |

### 4.5. Panel MVC (login por cookie)

`OdontoSoft.Web` (proyecto Razor) consume `ApiGetToken` por HTTP desde `AccountService`, valida el JWT recibido y emite una **cookie de autenticación propia** (`OdontoSoftCookie`, `CookieAuthentication`) con `ExpireTimeSpan = 8 h` y `SlidingExpiration = true`. Esto permite que el MVC siga funcionando como una app clásica de servidor con `Authorize` y `LoginPath = /Account/Login`.

---

## 5. Sistema RBAC (Roles y Permisos)

Modelo N:M clásico, **persistente en la base de identidad `MANAGER_ODONTOSOFT`**:

```
┌────────────┐   ┌──────────────┐   ┌────────────┐
│   User     │──►│  UsuarioRol  │◄──│   Rol      │
└────────────┘   └──────┬───────┘   └─────┬──────┘
                        │                 │
                        │   ┌─────────────┴──────┐
                        └──►│   RolPermiso        │◄──┐
                            └─────────────┬──────┘   │
                                          ▼          │
                                      ┌──────────┐   │
                                      │ Permiso  │───┘
                                      └──────────┘
```

- **`Permiso`** = unidad mínima de autorización. Tiene `Codigo` único, `Modulo`, `Nombre`, `Descripcion`, `Disabled` (soft-delete).
- **`Rol`** = agrupación de permisos. Tiene `Codigo` único, `Nombre`, `Descripcion`, `Disabled`.
- **`RolPermiso`** = join N:M con PK compuesta `(RolId, PermisoId)`, `OnDelete(Cascade)`.
- **`UsuarioRol`** = join N:M con PK compuesta `(UsuarioId, RolId)`, `OnDelete(Cascade)`.

### Cómo se resuelven los permisos

En `GenerateToken.Generate()` se hace **una sola query** que cruza `UsuarioRol → Rol → RolPermiso → Permiso` filtrando `Disabled` en ambos lados, y se inyectan como **claims**:

```csharp
claims.Add(new Claim("Rol",     rol.Codigo));     // 1 claim por rol
claims.Add(new Claim("Permiso", permiso.Codigo)); // 1 claim por permiso
```

Así, el backend principal (`OdontoSoft.API`) puede autorizar sin volver a la DB:

```csharp
User.HasClaim("Permiso", "Paciente.Crear")
User.HasClaim("Rol",     "admin")
```

> **Trade-off documentado**: si cambian los permisos/roles del usuario, debe volver a loguearse para que el JWT refleje los nuevos claims (los tokens ya emitidos no se invalidan).

---

## 6. Capa de datos

### 6.1. DbContexts

| DbContext                  | Base de datos             | Paquete EF                                     | Propósito                                         |
| -------------------------- | ------------------------- | ---------------------------------------------- | ------------------------------------------------- |
| `OdontosoftContext`        | `Odontosoft` (SQL Server) | `Microsoft.EntityFrameworkCore.SqlServer` 9.x  | Modelo de negocio: pacientes, turnos, pagos, etc. |
| `ManagerOdontosoftContext` | `MANAGER_ODONTOSOFT`      | `Microsoft.EntityFrameworkCore.SqlServer` 10.x | Identidad + RBAC: User, Rol, Permiso, joins       |

### 6.2. Unit of Work + Repository genérico

```csharp
public class UnitOfWork : IUnitOfWork, IDisposable {
    private readonly OdontosoftContext _context;
    public IStoredProcedureRepository StoredProcedureRepository { get; }
    public int  Save()       => _context.SaveChanges();
    public Task SaveAsync()  => _context.SaveChanges();
    public IQueryable<BaseEntity> GetAll(Type type) { /* reflexión */ }
}
```

Cada entidad de negocio tiene su `I…Repository : IGenericRepository<T>` (ej. `IPacienteRepository`, `ITurnoRepository`, `IPresupuestoRepository`). `IGenericRepository<T>` expone:

```csharp
IQueryable<T> All();
IQueryable<T> GetAllEnabled();        // excluye Disabled
T GetById(int id);
T GetByIdAsNoTracking(int id);
bool Add(T entity);
Task<bool> Delete(int id);
bool Upsert(T entity);
bool ValidateBeforeSave(T entity);
```

Además, **`StoredProcedureRepository`** encapsula la ejecución de stored procedures de SQL Server (ej. `GetPacienteForTurno(@searchText)`) para búsquedas optimizadas que no se resuelven bien con LINQ.

### 6.3. Auditoría automática

`OdontosoftContext.SaveChanges()` recorre las entidades `IAuditable` y, según su `ObjectState` (Added/Modified), llama a `SetInsertAuditData` o `SetUpdateAuditData`, completando:

- `CreatedDate / CreatedUserId / CreatedUserName`
- `UpdatedDate / UpdatedUserId / UpdatedUserName`
- `Disabled` (soft-delete lógico)

Los datos del usuario que audita se obtienen de `IAuthenticatedUser`, que en `OdontoSoft.API` está conectado al **claim `Id` del JWT** y en `OdontoSoft.Web` a la cookie de sesión.

### 6.4. Convenciones de modelo

- Herencia: `Entity` → `AuditedEntity` → entidades concretas (`Paciente`, `Turno`, `Presupuesto`, etc.).
- Convención: se remueve el pluralizado de nombres de tabla (`RemovePluralizingTableNameConvention`).
- Todas las FKs usan `DeleteBehavior.Restrict` por defecto (override global en `OnModelCreating`).
- Columnas `datetime` configuradas explícitamente; longitudes máximas en strings.
- Índices únicos: `User.Username` (implícito), `User.Email`, `Rol.Codigo`, `Permiso.Codigo`, `RolPermiso(RolId, PermisoId)`, `UsuarioRol(UsuarioId, RolId)`.
- Migrations versionadas en `ApiGetToken\Migrations\` (incluye `20250106…` inicial y `20260620…_AddRBAC` para el módulo de roles/permisos).

### 6.5. Modelo de dominio (entidades principales)

`Paciente`, `Profesional`, `Localidad`, `Provincia`, `Tratamiento`, `TratamientoPrecio`, `Ubicacion`, `Turno`, `EstadoTurno`, `FichaTratamiento`, `OdontogramaPieza`, `Periodontograma`, `PeriodontogramaPieza`, `HistoriaClinica`, `Presupuesto`, `PresupuestoDetalle`, `Pago`, `ImagenPaciente`.

---

## 7. Endpoints principales de la API de negocio (`OdontoSoft.API`)

Todos los controllers están bajo `[Authorize]` y exponen operaciones CRUD + endpoints de consulta específicos:

| Controller                                               | Ruta base                         | Endpoints                                                                                                              |
| -------------------------------------------------------- | --------------------------------- | ---------------------------------------------------------------------------------------------------------------------- |
| `PacienteController`                                     | `/api/paciente`                   | `GetAll` (paginado), `GetById/{id}`, `Create`, `Update/{id}`, `Delete/{id}`, `GetAllByNombreDni`, `GetNombreById/{id}` |
| `ProfesionalController`                                  | `/api/profesional`                | CRUD + búsqueda                                                                                                        |
| `TurnoController`                                        | `/api/turno`                      | CRUD + agenda                                                                                                          |
| `TratamientoController`                                  | `/api/tratamiento`                | CRUD + precios                                                                                                         |
| `PresupuestoController` / `PresupuestoDetalleController` | `/api/presupuesto[detalle]`       | Confección y cálculo de presupuestos                                                                                   |
| `PagoController`                                         | `/api/pago`                       | Registro de pagos                                                                                                      |
| `HistoriaClinicaController`                              | `/api/historiaclinica`            | Historia clínica del paciente                                                                                          |
| `FichaTratamientoController`                             | `/api/fichatratamiento`           | Fichas de tratamiento                                                                                                  |
| `OdontogramaPiezaController`                             | `/api/odontograma`                | Odontograma por pieza dental                                                                                           |
| `PeriodontogramaController`                              | `/api/periodontograma`            | Periodontograma                                                                                                        |
| `ImagenPacienteController`                               | `/api/imagenpaciente`             | Upload multipart (hasta 10 MB) de imágenes del paciente                                                                |
| `LocalidadController` / `ProvinciaController`            | `/api/localidad` `/api/provincia` | Geo-data, con importador externo vía `HttpClient`                                                                      |
| `UbicacionController`                                    | `/api/ubicacion`                  | Ubicaciones del consultorio                                                                                            |
| `EstadoTurnoController`                                  | `/api/estadoturno`                | Estados configurables de un turno                                                                                      |

### Características transversales

- **Errores uniformes** vía `ApiErrorResponse.FromException(ex, "Paciente")` que traduce `DbUpdateException` (DNI duplicado, FK violation, NULL en columna obligatoria) a mensajes en español.
- **Paginación** con `PaginationParams` + `PagedResult<T>`.
- **JSON** camelCase, case-insensitive, sin ciclos, ignorando `null` al escribir, y con `LenientNullableJsonConverter` para aceptar `""` como `null` en `int?`, `decimal?`, `DateTime?`.
- **CORS** abierto (`AllowAnyOrigin/Header/Method`) para frontends React/Vite y tunnels tipo `trycloudflare.com`.
- **Upload de imágenes** con `MultipartBodyLengthLimit = 10 MB`.

---

## 8. Cómo correrlo localmente

### Requisitos

- .NET 9 SDK + .NET 10 SDK instalados.
- SQL Server (Express o LocalDB). Connection strings en `appsettings.json`.

### Pasos

```bash
# 1) Base de datos de identidad (en el repo hermano ApiGetToken)
cd ../Apis/ApiGetToken
dotnet ef database update   # crea MANAGER_ODONTOSOFT con tablas User, Rol, Permiso, etc.

# 2) Base de datos de negocio
cd ../../OdontosoftBack/OdontoSoft.Web
dotnet ef database update   # crea Odontosoft con tablas de dominio

# 3) Levantar ApiGetToken (identidad)
cd ../../Apis/ApiGetToken
dotnet run                  # https://localhost:7229

# 4) Levantar OdontoSoft.API (negocio)
cd ../../OdontosoftBack/OdontoSoft.API
dotnet run                  # https://localhost:7xxx

# 5) Levantar el panel admin MVC (opcional)
cd ../OdontoSoft.Web
dotnet run                  # https://localhost:5xxx
```

Swagger UI queda disponible en cada API (`/swagger`).

---

## 9. Aprendizajes y logros técnicos destacables

- **Diseño de un sistema de identidad único y desacoplado** consumido por dos frontends distintos (MVC clásico + API REST), compartiendo la misma clave JWT.
- **Patrón Unit of Work + Repositorio genérico** sobre Entity Framework Core, con generación masiva de repositorios específicos por entidad.
- **RBAC completo** modelado en DB (User ↔ Rol ↔ Permiso) y replicado en el JWT como claims, evitando round-trips a la DB en cada request.
- **Auditoría automática** de entidades en `SaveChanges` con datos del usuario autenticado (vía `IAuthenticatedUser`).
- **Migración de contraseñas legadas a BCrypt en el login** (lazy upgrade sin downtime).
- **Manejo de errores uniforme y en español** para toda la API, traduciéndolos desde excepciones crudas de SQL Server (`DbUpdateException`) a respuestas comprensibles.
- **Stored procedures + EF Core LINQ** coexistiendo en la misma solución para casos de búsqueda optimizados.
- **CORS, multipart upload, paginación, validación custom, Swagger** configurados de forma production-ready.

---

## 10. Licencia

Este proyecto se distribuye bajo los términos del archivo `LICENSE` incluido en el repositorio.

---

**Mauro Perelda** — _Backend .NET Developer_
