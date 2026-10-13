# Dockerfile para Render
# Build context: RAÍZ del repo (`Root Directory` en Render = `.`, no `OdontoSoft.API/`).
# Esto es necesario porque la API referencia proyectos hermanos
# (OdontoSoft.Data, OdontoSoft.Entities, OdontoSoft.Web, Odontosoft.Services)
# y DLLs legacy en lib/, y Docker no puede hacer COPY fuera del contexto.
#
# Multi-stage build: SDK para compilar, ASP.NET runtime para ejecutar (imagen final liviana).

# ── Stage 1: build ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiamos primero los .csproj de todos los proyectos para que la restore quede
# en una layer cacheada. Si después cambia solo código (no dependencias), no
# re-restauramos. IMPORTANTE: tienen que estar todos los proyectos referenciados,
# en el mismo orden relativo en que el .sln los declara, sino el restore falla.
COPY ["OdontoSoft.API/OdontoSoft.API.csproj",                       "OdontoSoft.API/"]
COPY ["OdontoSoft.Data/OdontoSoft.Data.csproj",                     "OdontoSoft.Data/"]
COPY ["OdontoSoft.Entities/OdontoSoft.Entities.csproj",             "OdontoSoft.Entities/"]
COPY ["Odontosoft.Services/Odontosoft.Services.csproj",             "Odontosoft.Services/"]
COPY ["OdontoSoft.Web/OdontoSoft.Web.csproj",                       "OdontoSoft.Web/"]
# Las DLLs legacy (Mauro.Entities, MedveCore.Common, Mauro.RepositoryPattern)
# se referencian con HintPath relativo a `lib/`, así que la restore las necesita
# presentes en disco. Sin esto, dotnet restore no las encuentra.
COPY ["lib/",                                                         "lib/"]

# Restore solo del proyecto API (que arrastra transitivamente a los demás).
RUN dotnet restore "OdontoSoft.API/OdontoSoft.API.csproj"

# Ahora el código fuente completo
COPY . .

# Publicamos en Release al directorio /app/publish.
# /p:UseAppHost=false evita generar un ejecutable nativo (no se necesita en Linux).
RUN dotnet publish "OdontoSoft.API/OdontoSoft.API.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# ── Stage 2: runtime ───────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render inyecta $PORT en runtime (default 10000). El shell form permite
# expandir la variable al arranque. Si $PORT no estuviera, cae al default.
ENTRYPOINT ["sh", "-c", "dotnet OdontoSoft.API.dll --urls=http://0.0.0.0:${PORT:-10000}"]
