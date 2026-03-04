# RentingPrototype - Guía técnica (src)

## 1) ¿Qué hace este proyecto?

`RentingPrototype` es una API REST (Minimal API en .NET 9) para un prototipo de renting de vehículos.

Permite:
- Registrar vehículos.
- Consultar vehículos (todos, por id y disponibles).
- Iniciar un alquiler de vehículo (rent).
- Cerrar un alquiler (return).

El sistema usa SQLite con datos semilla (vehículos, clientes e histórico de alquileres), lo que facilita probar el flujo sin configuración adicional de base de datos externa.

## 2) ¿Cómo lo hace? (arquitectura y patrones)

El proyecto está organizado en capas:

- `RentingPrototype.Api`
  - Exposición HTTP con Minimal APIs.
  - Endpoints en `Endpoints/Vehicle` y `Endpoints/Rental`.
  - Configuración de DI, OpenAPI y Scalar en `Program.cs`.

- `RentingPrototype.Application`
  - Casos de uso con handlers (`CreateVehicleHandler`, `CreateRentalHandler`, `UpdateRentalHandler`, etc.).
  - Contratos/abstracciones (`IUnitOfWork`, repositorios command/query).
  - DTOs de entrada y salida.

- `RentingPrototype.Domain`
  - Entidades de dominio (`Vehicle`, `Rental`) y reglas de negocio.
  - Ejemplo: un vehículo no puede registrarse si tiene más de 5 años.

- `RentingPrototype.Infrastructure`
  - Implementaciones de persistencia con SQLite + Dapper.
  - Repositorios SQL para comandos y consultas.
  - `SqliteUnitOfWork` para transacciones por caso de uso.
  - Script de esquema y seed: `rentingprototype-schema.sql`.

Patrones aplicados actualmente:
- Arquitectura por capas (separación de responsabilidades).
- CQRS ligero (handlers y repositorios separados para lectura/escritura).
- Repository pattern (acceso a datos desacoplado de aplicación).
- Unit of Work (begin/commit/rollback en casos de escritura).

## 3) Puesta en marcha y pruebas (incluyendo Scalar)

### Requisitos
- .NET SDK 9.0

### Arranque local
Desde la raíz del repo:

```bash
dotnet restore
dotnet run --project src/RentingPrototype.Api
```

La API arranca, crea automáticamente la base SQLite si no existe y carga schema + seed.

Con `launchSettings.json`, en desarrollo expone por defecto:
- `http://localhost:5062`

### Documentación OpenAPI y Scalar
Con la API arrancada:
- OpenAPI JSON: `http://localhost:5062/openapi/v1.json`
- Scalar UI: `http://localhost:5062/scalar/v1`
  - También responde `http://localhost:5062/scalar/`.

### Endpoints principales
- `GET /vehicles/{id}`
- `GET /vehicles`
- `GET /vehicles/available`
- `POST /vehicles`
- `POST /rentals/rent-vehicle`
- `POST /rentals/return-vehicle`

### Ejemplos rápidos (curl)

Crear vehículo:

```bash
curl -X POST http://localhost:5062/vehicles \
  -H "Content-Type: application/json" \
  -d '{
    "licensePlate": "9999-ZZZ",
    "brand": "Honda",
    "model": "Civic",
    "manufactureDateUtc": "2024-01-01T00:00:00Z"
  }'
```

Consultar disponibles:

```bash
curl http://localhost:5062/vehicles/available
```

Crear alquiler:

```bash
curl -X POST http://localhost:5062/rentals/rent-vehicle \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "a1111111-0000-0000-0000-000000000001",
    "vehicleId": "b2222222-0000-0000-0000-000000000001",
    "startDate": "2026-03-01T10:00:00Z"
  }'
```

Cerrar alquiler:

```bash
curl -X POST http://localhost:5062/rentals/return-vehicle \
  -H "Content-Type: application/json" \
  -d '{
    "id": "<rental-id>",
    "endDate": "2026-03-04T18:00:00Z"
  }'
```

### Tests
Desde raíz:

```bash
dotnet test RentingPrototype.sln
```

Estado actual verificado:
- Unit tests: 16 OK
- Integration tests: 1 OK

## 4) TO-DO / futuros pasos

1. Endurecer reglas de dominio de `Rental`:
- Validar coherencia de fechas (`startDate <= endDate`).
- Validar existencia de `customerId` y `vehicleId` antes de crear alquiler.

2. Corregir y robustecer `UpdateRentalHandler`:
- Manejar `rental == null` (actualmente puede provocar null reference).
- Evitar actualizar alquileres ya cerrados sin regla explícita.

3. Mejorar consistencia de contratos CQRS:
- Usar DTO específico para `GetAvailableVehicles` (hoy reutiliza `ListVehiclesQueryDto`).

4. Mejorar API y observabilidad:
- Añadir validación de requests (FluentValidation o filtro equivalente).
- Añadir middleware de manejo global de errores y problem details.
- Añadir logging estructurado.

5. Evolución de persistencia:
- Añadir migraciones/versionado de esquema (actualmente script SQL plano).
- Revisar estrategia de inicialización de base para producción.

6. Completar cobertura de pruebas:
- Casos de alquiler/retorno y errores de negocio.
- Pruebas de concurrencia (doble alquiler del mismo vehículo/cliente).
- Tests de integración para rutas de lectura y errores HTTP.

7. Dockerización del proyecto (próximos pasos):
- Crear un `Dockerfile` multi-stage para .NET 9:
- Etapa build con `mcr.microsoft.com/dotnet/sdk:9.0` (restore/build/publish).
- Etapa runtime con `mcr.microsoft.com/dotnet/aspnet:9.0` (imagen final ligera).
- Incluir únicamente los artefactos publicados y las dependencias necesarias para ejecutar la API.
- Añadir soporte de Visual Studio para ejecutar en contenedor:
- Incluir perfil Docker en `launchSettings.json` (o añadir soporte con los artefactos estándar de contenedor de VS).
- Añadir/ajustar `.dockerignore` para excluir `bin/`, `obj/`, `.git/`, `.vs/`, `.data/` y otros ficheros no necesarios.
- Configuración adicional de contenedor:
- Exponer el puerto de la API (por ejemplo `8080` en contenedor y mapeo a `5062` en host).
- Definir variables de entorno requeridas (por ejemplo `ASPNETCORE_URLS` y `ASPNETCORE_ENVIRONMENT`).
- Valorar volumen para persistir la base SQLite fuera del contenedor (`/app/.data`).
- Verificar funcionamiento en ambos escenarios:
- Ejecución local sin contenedor (`dotnet run`).
- Ejecución en contenedor (`docker build` + `docker run`) validando endpoints y Scalar.
