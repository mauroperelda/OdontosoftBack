# OdontoSoft.Web

Proyecto legacy ASP.NET Core MVC (.NET 9) que expone la misma funcionalidad
que [OdontoSoft.API](../OdontoSoft.API/README.md) pero con un frontend Razor
(DevExtreme/jQuery/Bootstrap) en lugar de la API REST consumida por el
front React.

## Estado actual

**Legacy.** A partir de la Fase 4 del plan de modernización
(ver [PLAN_MODERNIZACION.md](../PLAN_MODERNIZACION.md) si existe, o el
[commit `20660db`](https://github.com/)), `OdontoSoft.API` ya no depende
de este proyecto. La clase `AuthenticatedUser` y las constantes
`PermissionCodes` se movieron a `OdontoSoft.Data/Authentication/`, así que
Web puede seguir compilando standalone sin necesidad de un
`ProjectReference` desde la API.

**Roadmap:** deprecar este proyecto una vez que la API cubra todos los
flujos que hoy expone el MVC. Mientras tanto, ambos frontends conviven:
- API en producción, consumida por el front React.
- Web como herramienta de administración interna (DevExtreme grilla +
  formularios) que aún no fue migrada al front nuevo.

## Cómo correrlo standalone

### Requisitos
- .NET 9 SDK.
- PostgreSQL accesible (misma connection string que la API).
- Variables de entorno o `appsettings.json` con `ConnectionStrings:DefaultConnection`.

### Pasos

```bash
# 1. Restaurar la solución completa (necesita los proyectos hermanos
#    porque Web referencia OdontoSoft.Data + OdontoSoft.Entities + Services)
dotnet restore OdontoSoft.sln

# 2. Compilar y correr Web en particular
dotnet run --project OdontoSoft.Web

# 3. Por defecto escucha en https://localhost:5001 / http://localhost:5000
#    Abrir en el browser y loguearse con las credenciales del seed.
```

### Rutas principales

| Ruta | Qué hace |
|---|---|
| `/Account/Login` | Login (cookie auth) |
| `/Browser?code=Paciente` | Grilla DevExtreme de Pacientes |
| `/Browser?code=Turno` | Grilla + calendario de Turnos |
| `/Browser?code=Presupuesto` | Presupuestos con sus detalles y pagos |
| `/ApiGetToken` | Emite JWT para la API (usado por scripts) |

Las acciones POST/PUT/DELETE en `OdontoSoft.Web/Controllers/Custom/*` son
la fuente original de la lógica de negocio. Esa lógica se está migrando
progresivamente a `OdontoSoft.Services/Custom/*` (cada service tiene su
contraparte en un controller API). **No agregar lógica nueva en los
controllers de Web** — agregar el método en el Service correspondiente y
exponerlo en el controller de Web también.

## Docker

**Este proyecto no se incluye en el Docker image de producción.** El
`Dockerfile` solo compila y publica `OdontoSoft.API`. Si necesitás
correr Web en un container propio, podés armar uno aparte con
`dotnet publish -c Release -o out` y un `Dockerfile` mínimo basado en
`aspnet:9.0`.

## Estructura

```
OdontoSoft.Web/
├── Controllers/
│   ├── Base/                    ← AppBaseController + CustomConstructors/*
│   ├── Custom/                  ← Implementación de los controllers (legacy)
│   └── *.cs                     ← Controllers "thin" que delegan a Custom/
├── Helpers/
│   ├── Application/             ← ApplicationMenu, módulos
│   ├── Constants/               ← Modulos (NO mover — solo Web lo usa)
│   ├── Menu/                    ← Sidebar / menú
│   └── ...
├── Views/                       ← Razor (.cshtml) — DevExtreme
├── Models/                      ← ErrorViewModel + otros
├── Program.cs                   ← Bootstrap del host MVC
└── appsettings.json
```

## Dependencias clave

- `OdontoSoft.Data` (compartido) — para los repos y `IAuthenticatedUser`.
- `OdontoSoft.Entities` (compartido) — para los modelos y VMs.
- `OdontoSoft.Services` (compartido) — la lógica de negocio migrada vive acá.
- `DevExtreme.AspNet.Core` + `DevExtreme.AspNet.Mvc` — los componentes de grilla.
- `MedveCore.Common.dll`, `Mauro.Entities.dll`, `Mauro.RepositoryPattern.dll`
  (en `lib/`) — dependencias legacy que aún no se pueden mover a NuGet.
