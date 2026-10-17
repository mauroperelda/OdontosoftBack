# OdontoSoft — Backend (.NET)

> Solución backend para la gestión integral de un consultorio odontológico: historia clínica, turnos, presupuestos, pagos, odontograma, periodontograma e imágenes del paciente, con autenticación JWT, autorización por roles/permisos (RBAC) y arquitectura en capas sobre .NET 10.

> **Repos relacionados**
>
> - **Frontend:** [`odontosoft-front`](https://github.com/mauroperelda/odontosoft) — SPA React + Vite que consume la `OdontoSoft.API` vía JWT.
> - **API de identidad (sister-repo):** [`ApiGetToken`](https://github.com/mauroperelda/ApiGetToken) — emite y valida los tokens JWT con BCrypt + RBAC.
>
> Las dos APIs del backend (`OdontoSoft.API` y `ApiGetToken`) comparten la misma `Jwt:Key` / `Jwt:Issuer` / `Jwt:Audience`, por lo que un token emitido por la API de identidad es aceptado por la API de negocio sin re-login.

---

## 1. Resumen

- **Dominio**: software de gestión para clínica odontológica (pacientes, profesionales, turnos, tratamientos, presupuestos, pagos, historia clínica, odontograma y periodontograma).
- **Stack**: **C# / .NET 10**, **ASP.NET Core Web API**, **ASP.NET Core MVC (Razor)**, **Entity Framework Core 10** sobre **PostgreSQL** (Npgsql + snake_case naming).
- **Arquitectura**: monorepo en capas con **Repository + Unit of Work**, **DTOs / ViewModels**, **servicios de dominio** con `ServiceResult` pattern, **API REST desacoplada** consumida por el front React y un **panel admin MVC Razor** (DevExtreme) corriendo standalone como legacy.
- **Seguridad**: hashing de contraseñas con **BCrypt** (workFactor 11) en `ApiGetToken`, **JWT Bearer** con clave HMAC-SHA256, validación de `Issuer`/`Audience`/`Lifetime`, claims de identidad + roles + permisos efectivos firmados dentro del token, y **soft-delete + auditoría** (created/updated, user, fecha) en todas las entidades de negocio.
- **Observabilidad**: logging estructurado con **Serilog** (formato `CompactJsonFormatter` en consola), **health checks** en `/health/live` y `/health/ready` (incluye ping a PostgreSQL), **rate limiting** nativo por IP, **CORS** por lista blanca desde configuración.

---

## 2. Arquitectura

La solución está organizada como **dos repositorios independientes** que comparten la misma autoridad de identidad:

```
OdontosoftBack/                 ← este repositorio (.NET 10)
├── OdontoSoft.API/             ← Web API REST (net10.0) — backend principal
├── OdontoSoft.Web/             ← App MVC Razor (net10.0) — panel admin legacy
├── OdontoSoft.Data/            ← EF Core, DbContext, Repositorios, UoW, Auth helpers
├── OdontoSoft.Entities/        ← Modelos de dominio + ViewModels + BaseEntity
└── OdontoSoft.Services/        ← Servicios de dominio (lógica de negocio)

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
│  Controllers +       │  share │  /api/Auth/Me        │
│  Services + UoW      │        │                      │
└──────────┬───────────┘        └──────────┬───────────┘
           │ EF Core                       │ EF Core
           ▼                               ▼
   ┌───────────────┐                ┌─────────────────┐
   │  PostgreSQL   │                │  PostgreSQL     │
   │   odontosoft  │                │ MANAGER_ODONTOSOFT│
   └───────────────┘                └─────────────────┘
```

> **Nota de Fase 4**: `OdontoSoft.API` ya **no** tiene `ProjectReference` a `OdontoSoft.Web`. La clase `AuthenticatedUser` (que vive en `OdontoSoft.Data/Authentication/`) es la única dependencia compartida. Web corre standalone, ver [`OdontoSoft.Web/README.md`](OdontoSoft.Web/README.md).

### Patrones aplicados

- **Layered architecture**: Controllers → Services → Repositories (UoW) → DbContext → DB.
- **ServiceResult pattern**: los services devuelven `ServiceResult` / `UpsertResult<T>` (con `IsSuccess`, `Error`, `FieldErrors`); los controllers los traducen a `IActionResult` con códigos HTTP apropiados.
- **Repository genérico + Unit of Work** (`IGenericRepository<T>`, `IUnitOfWork`) — las transacciones se confirman con `UnitOfWork.Save()`.
- **DTOs / ViewModels separados de entidades** (`PacienteViewModel`, `LoginUserRequest/Response`) para no exponer el modelo de EF.
- **Soft-delete + auditoría automática** vía `AuditedEntity` y `SaveChanges()`/`SaveChangesAsync()` override que setea `CreatedDate/UpdatedDate/CreatedUserId/UpdatedUserId/Disabled` desde `IAuthenticatedUser` (resuelto por el JWT).
- **Inversión de dependencias** vía `Microsoft.Extensions.DependencyInjection` (`AddScoped`/`AddTransient`).

---

## 3. Stack tecnológico

| Capa           | Tecnología                                                                                                       |
| -------------- | ---------------------------------------------------------------------------------------------------------------- |
| Lenguaje       | **C# 13 / .NET 10** (todos los proyectos alineados)                                                              |
| Backend API    | **ASP.NET Core 10 Web API**, controllers, `[Authorize]`, middleware custom, Rate Limiting nativo                |
| Frontend admin | **ASP.NET Core 10 MVC** (Razor), `AddRazorRuntimeCompilation`, jQuery, **DevExtreme** 24.2                       |
| ORM            | **Entity Framework Core 10** con `Npgsql.EntityFrameworkCore.PostgreSQL 10.x` y `EFCore.NamingConventions 10.x` |
| Migrations     | **EF Core Migrations** (`Add-Migration`, `Update-Database`)                                                      |
| Autenticación  | **JWT Bearer** (`Microsoft.AspNetCore.Authentication.JwtBearer`, `Microsoft.IdentityModel.Tokens`)               |
| Hashing        | **BCrypt.Net-Next** (workFactor 11) — en `ApiGetToken`                                                           |
| Validación     | **FluentValidation 11** + `InvalidModelStateResponseFactory` custom (errores en español)                         |
| Mapping        | **AutoMapper 16.1.1** (en MVC y API)                                                                            |
| Documentación  | **Swashbuckle / Swagger UI** (`Swashbuckle.AspNetCore`)                                                           |
| Logging        | **Serilog.AspNetCore 9** + `Serilog.Formatting.Compact` (JSON estructurado en consola)                           |
| Health checks  | **AspNetCore.HealthChecks.NpgSql** (`/health/live`, `/health/ready`)                                             |
| Base de datos  | **PostgreSQL** (Npgsql provider)                                                                                 |
| Otros          | `FluentValidation.DependencyInjectionExtensions`, `DevExtreme.AspNet.Data 5.1.0`, jQuery Validation              |

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
        ClockSkew = TimeSpan.FromSeconds(30)   // tolerar NTP drift
    };
});
```

Cada controller de la API lleva `[Authorize]` y obtiene la identidad vía `IAuthenticatedUser` (que se construye a partir de los claims del JWT, no de la cookie de sesión).

### 4.4. Endpoints de identidad (viven en `ApiGetToken`)

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

Modelo N:M clásico, **persistente en la base de identidad `MANAGER_ODONTOSOFT`** (PostgreSQL):

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

| DbContext                  | Base de datos             | Provider                                | Propósito                                         |
| -------------------------- | ------------------------- | --------------------------------------- | ------------------------------------------------- |
| `OdontosoftContext`        | `odontosoft`              | `Npgsql.EntityFrameworkCore.PostgreSQL` | Modelo de negocio: pacientes, turnos, pagos, etc. |
| `ManagerOdontosoftContext` | `MANAGER_ODONTOSOFT`      | `Npgsql.EntityFrameworkCore.PostgreSQL` | Identidad + RBAC: User, Rol, Permiso, joins       |

> **Migración reciente (Fase 1.1)**: la base de negocio se cambió de **SQL Server** a **PostgreSQL** con snake_case naming convention (`Paciente` → tabla `paciente`, `CreatedUserId` → columna `created_user_id`).

### 6.2. Unit of Work + Repository genérico

```csharp
public class UnitOfWork : IUnitOfWork, IDisposable {
    private readonly OdontosoftContext _context;
    public IStoredProcedureRepository StoredProcedureRepository { get; }
    public int  Save()                => _context.SaveChanges();
    public Task SaveAsync(CancellationToken ct = default)  => _context.SaveChangesAsync(ct);
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

`StoredProcedureRepository` queda en el código pero la base actual es PostgreSQL — no se usa en los servicios poblados. Será removido en una fase futura.

### 6.3. Auditoría automática

`OdontosoftContext.SaveChanges()` y `SaveChangesAsync()` recorren las entidades `IAuditable` y, según su `ObjectState` (Added/Modified), llaman a `SetInsertAuditData` o `SetUpdateAuditData`, completando:

- `CreatedDate / CreatedUserId / CreatedUserName`
- `UpdatedDate / UpdatedUserId / UpdatedUserName`
- `Disabled` (soft-delete lógico)

Los datos del usuario que audita se obtienen de `IAuthenticatedUser`, que en `OdontoSoft.API` está conectado al **claim `Id` del JWT** y en `OdontoSoft.Web` a la cookie de sesión.

### 6.4. Convenciones de modelo

- Herencia: `Entity` → `AuditedEntity` → entidades concretas (`Paciente`, `Turno`, `Presupuesto`, etc.).
- **Snake_case naming** vía `EFCore.NamingConventions` (PostgreSQL es case-sensitive con quoted identifiers; snake_case evita quoted identifiers en queries raw).
- Todas las FKs usan `DeleteBehavior.Restrict` por defecto (override global en `OnModelCreating`).
- Columnas `datetime` configuradas explícitamente; longitudes máximas en strings.
- Índices únicos: `User.Username` (implícito), `User.Email`, `Rol.Codigo`, `Permiso.Codigo`, `RolPermiso(RolId, PermisoId)`, `UsuarioRol(UsuarioId, RolId)`.
- Migrations versionadas en `ApiGetToken\Migrations\` y `OdontoSoft.Data\Migrations\`.

### 6.5. Modelo de dominio (entidades principales)

`Paciente`, `Profesional`, `Localidad`, `Provincia`, `Tratamiento`, `TratamientoPrecio`, `Ubicacion`, `Turno`, `EstadoTurno`, `FichaTratamiento`, `OdontogramaPieza`, `Periodontograma`, `PeriodontogramaPieza`, `HistoriaClinica`, `Presupuesto`, `PresupuestoDetalle`, `Pago`, `ImagenPaciente`.

---

## 7. Endpoints principales de la API de negocio (`OdontoSoft.API`)

Todos los controllers están bajo `[Authorize]` y exponen operaciones CRUD + endpoints de consulta específicos:

| Controller                                               | Ruta base                         | Endpoints                                                                                                              |
| -------------------------------------------------------- | --------------------------------- | ---------------------------------------------------------------------------------------------------------------------- |
| `PacienteController`                                     | `/api/paciente`                   | `GetAll` (paginado), `GetById/{id}`, `Create`, `Update/{id}`, `Delete/{id}`, `GetAllByNombreDni`, `GetNombreById/{id}` |
| `ProfesionalController`                                  | `/api/profesional`                | CRUD + búsqueda                                                                                                        |
| `TurnoController`                                        | `/api/turno`                      | CRUD + agenda + `GetCalendario`                                                                                        |
| `TratamientoController`                                  | `/api/tratamiento`                | CRUD + `GetAllConPrecios` + upsert/delete de precios por ubicación                                                    |
| `PresupuestoController` / `PresupuestoDetalleController` | `/api/presupuesto[detalle]`       | Confección y cálculo de presupuestos (con auto-flip Pagado/Pendiente)                                                 |
| `PagoController`                                         | `/api/pago`                       | Registro de pagos                                                                                                      |
| `HistoriaClinicaController`                              | `/api/historiaclinica`            | Historia clínica del paciente + piezas del odontograma + fichas de tratamiento                                        |
| `FichaTratamientoController`                             | `/api/fichatratamiento`           | Fichas de tratamiento                                                                                                  |
| `OdontogramaPiezaController`                             | `/api/odontograma`                | Odontograma por pieza dental (upsert por HC + número de pieza)                                                         |
| `PeriodontogramaController`                              | `/api/periodontograma`            | Periodontograma con métricas calculadas (BoP%, placa%, presentes/ausentes)                                             |
| `ImagenPacienteController`                               | `/api/imagenpaciente`             | Upload multipart (hasta 10 MB) de imágenes del paciente                                                                |
| `LocalidadController` / `ProvinciaController`            | `/api/localidad` `/api/provincia` | Geo-data, con importador externo vía `HttpClient`                                                                      |
| `UbicacionController`                                    | `/api/ubicacion`                  | Ubicaciones del consultorio                                                                                            |
| `EstadoTurnoController`                                  | `/api/estadoturno`                | Estados configurables de un turno                                                                                      |

> **Fase 4**: la lógica de negocio se movió de los controllers a la capa `OdontoSoft.Services` (`PresupuestoServices`, `TurnoServices`, `PacienteServices`, etc.). Los controllers son ahora "thin" — reciben el VM, llaman al service, traducen el `ServiceResult` a `IActionResult`.

### Características transversales

- **Errores uniformes** vía `ApiErrorResponse.FromException(ex, "Paciente")` que traduce `DbUpdateException` (unique violation, FK violation, NULL en columna obligatoria) a mensajes en español.
- **Validación de shape** con **FluentValidation** (9 validators viviendo en `OdontoSoft.API/Validators/`).
- **Paginación** con `PaginationParams` + `PagedResult<T>` + `ToPagedResultAsync` (server-side).
- **JSON** camelCase, case-insensitive, sin ciclos, ignorando `null` al escribir, y con `LenientNullableJsonConverter` para aceptar `""` como `null` en `int?`, `decimal?`, `DateTime?`.
- **CORS por lista blanca** desde `Cors:AllowedOrigins` en `appsettings.json` (en Development incluye `http://localhost:5173`, `http://localhost:3000` y `https://odontosoft-front.vercel.app`).
- **Upload de imágenes** con `MultipartBodyLengthLimit = 10 MB`.
- **Documentación** en `/swagger` (UI clásica de Swagger) + spec OpenAPI en `/swagger/v1/swagger.json`.
- **Health checks** en `/health/live` (proceso vivo) y `/health/ready` (incluye ping a PostgreSQL).
- **Rate limiting** nativo: 100 req/min por IP, aplicado con `[EnableRateLimiting("api")]` en endpoints sensibles.

---

## 8. Cómo correrlo localmente

### Requisitos

- .NET 10 SDK.
- PostgreSQL 14+ corriendo local (`localhost:5432` por defecto).
- Acceso al repo hermano `ApiGetToken` para la base de identidad.

### Pasos

```bash
# 1) Base de datos de identidad (en el repo hermano ApiGetToken)
cd ../Apis/ApiGetToken
dotnet ef database update   # crea MANAGER_ODONTOSOFT con tablas User, Rol, Permiso, etc.

# 2) Base de datos de negocio (las migrations se aplican automáticamente
#    al arrancar OdontoSoft.API gracias a db.Database.Migrate() en Program.cs)
#    Sólo necesario si querés crear la DB desde cero sin levantar la API:
cd ../../OdontosoftBack/OdontoSoft.Data
dotnet ef database update   # crea odontosoft con tablas de dominio

# 3) Levantar ApiGetToken (identidad) — puerto 7229
cd ../../Apis/ApiGetToken
dotnet run

# 4) Levantar OdontoSoft.API (negocio) — puerto configurado en launchSettings.json
cd ../../OdontosoftBack/OdontoSoft.API
dotnet run

# 5) Levantar el panel admin MVC (opcional, standalone)
cd ../OdontoSoft.Web
dotnet run
```

Una vez levantada la API:
- Documentación interactiva: `http://localhost:<puerto>/swagger`
- Spec OpenAPI: `http://localhost:<puerto>/swagger/v1/swagger.json`
- Liveness: `GET /health/live`
- Readiness: `GET /health/ready`

### Configuración local

`appsettings.Development.json` ya trae la connection string de PostgreSQL con valores por defecto (`Host=localhost;Port=5432;Database=odontosoft;Username=postgres;Password=…`). Para producción, esos valores se sobrescriben con variables de entorno en Render:

- `ConnectionStrings__DefaultConnection`
- `Jwt__Key`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Cors__AllowedOrigins__0`, `Cors__AllowedOrigins__1`, …

---

## 9. Tests

Hay **120 tests automatizados** distribuidos en dos proyectos:

- **`OdontoSoft.UnitTests`** (xUnit + FluentAssertions + Moq + EF InMemory): cubre helpers, validators, mapping, middleware, repository, y los 8 services de dominio (`PresupuestoServices`, `TurnoServices`, `PacienteServices`, `PresupuestoDetalleServices`, `HistoriaClinicaServices`, `OdontogramaPiezaServices`, `TratamientoServices`, `PeriodontogramaServices`).
- **`OdontoSoft.ArchitectureTests`** (NetArchTest): reglas de capas (Controllers no usan `DbContext` directamente, Services no referencian Controllers, etc.).

Cómo correrlos:

```bash
dotnet test OdontoSoft.sln -c Release
```

Output esperado:
```
Correctas! - Con error:     0, Superado:   115, Omitido:     0, Total:   115
Correctas! - Con error:     0, Superado:     5, Omitido:     0, Total:     5
```

---

## 10. Roadmap

Cosas que aún no se hicieron, anotadas para tracking:

- **Refactor de uploads**: hoy `ImagenPacienteController` escribe a `wwwroot/uploads/pacientes/…` que es **efímero en Render**. Pendiente abstraer `IFileStorage` (S3 / R2 / Supabase Storage) y migrar el código. Marcado con `// TODO` en el controller.
- **Paginación cursor-based**: si las tablas `Turno` o `Pago` superan ~100k filas, migrar `Skip/Take` a keyset (`WHERE id > @lastId LIMIT @pageSize`).
- **API versioning**: agregar `Asp.Versioning.Mvc` y rutas `api/v{version}/…` cuando aparezca un cambio breaking real.
- **Deprecación de `OdontoSoft.Web`**: una vez que el front React cubra todos los flujos del MVC, deprecar el proyecto legacy y eliminar los `MapViewModelToXxx` que sobreviven solo por compatibilidad.

---

## 11. Aprendizajes y logros técnicos destacables

- **Diseño de un sistema de identidad único y desacoplado** consumido por dos frontends distintos (MVC clásico + API REST), compartiendo la misma clave JWT.
- **Patrón Unit of Work + Repositorio genérico** sobre Entity Framework Core, con generación masiva de repositorios específicos por entidad.
- **RBAC completo** modelado en DB (User ↔ Rol ↔ Permiso) y replicado en el JWT como claims, evitando round-trips a la DB en cada request.
- **Auditoría automática** de entidades en `SaveChanges` y `SaveChangesAsync` con datos del usuario autenticado (vía `IAuthenticatedUser`).
- **Migración de contraseñas legadas a BCrypt en el login** (lazy upgrade sin downtime).
- **Manejo de errores uniforme y en español** para toda la API, traduciéndolos desde excepciones crudas (`DbUpdateException`) a respuestas comprensibles.
- **ServiceResult pattern** desacopla la lógica de negocio de HTTP — los services no conocen `IActionResult`, los controllers no contienen reglas de dominio.
- **OpenAPI nativo + Scalar UI** reemplazaron Swashbuckle sin sacrificar la experiencia de docs. *(Revierto a Swashbuckle en fase 6.x — el equipo prefiere la UI clásica de Swagger).*
- **Serilog con `CompactJsonFormatter`** produce logs estructurados listos para ingestión por Datadog/Loki/CloudWatch.
- **Health checks Npgsql** + **rate limiting nativo** de .NET 10 cubren las necesidades de producción sin paquetes externos adicionales.

---

## 12. Licencia

Este proyecto se distribuye bajo los términos del archivo `LICENSE` incluido en el repositorio.

---

**Mauro Perelda** — _Backend .NET Developer_
