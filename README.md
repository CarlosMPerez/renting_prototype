# RentingPrototype - Guía técnica

## 1) ¿Qué hace este proyecto?

`RentingPrototype` es una API REST construida con Minimal API en .NET 9 para un prototipo de renting de vehículos.

Permite:
- Registrar vehículos.
- Consultar vehículos (todos, por id y disponibles).
- Iniciar un alquiler (`rent`).
- Cerrar un alquiler (`return`).
- Consultar historial de alquileres por vehículo y por cliente.

La persistencia usa SQLite con datos semilla, por lo que el proyecto puede ejecutarse sin infraestructura externa.

## 2) Arquitectura y patrones

La solución está separada en capas:

- `src/RentingPrototype.Api`
  - Endpoints HTTP (Minimal API), DI, OpenAPI y Scalar.
- `src/RentingPrototype.Application`
  - Casos de uso (handlers), DTOs y contratos.
- `src/RentingPrototype.Domain`
  - Entidades (`Vehicle`, `Rental`) y reglas de negocio.
- `src/RentingPrototype.Infrastructure`
  - Persistencia SQLite + Dapper, repositorios y Unit of Work.

Patrones aplicados:
- Arquitectura por capas.
- CQRS ligero (lectura/escritura separadas).
- Repository pattern.
- Unit of Work transaccional en comandos.

## 3) Arranque local

### Requisitos
- .NET SDK 9.0

### Ejecutar API
Desde la raíz:

```bash
dotnet restore RentingPrototype.sln
dotnet run --project src/RentingPrototype.Api
```

En modo desarrollo expone por defecto:
- `http://localhost:5062`

Base de datos local:
- Se crea automáticamente en `data/rentingprototype.db` si no existe.
- El esquema y seed se cargan desde `src/RentingPrototype.Infrastructure/Persistence/Schema/rentingprototype-schema.sql`.

### OpenAPI y Scalar
- OpenAPI JSON: `http://localhost:5062/openapi/v1.json`
- Scalar UI: `http://localhost:5062/scalar/v1`
- La ruta raíz `/` redirige a `/scalar`.

## 4) Dockerización

La dockerización está implementada y funcional.

Incluye:
- `Dockerfile` multi-stage con:
  - Build: `mcr.microsoft.com/dotnet/sdk:9.0`
  - Runtime: `mcr.microsoft.com/dotnet/aspnet:9.0`
- Exposición de puerto `8080`.
- Variables de entorno (`ASPNETCORE_URLS`, `ASPNETCORE_ENVIRONMENT`).
- `docker-compose.yml` con servicio `renting-api` (`8080:8080`).
- Perfil de Visual Studio: `Container (Dockerfile)` en `launchSettings.json`.
- `.dockerignore` configurado para excluir artefactos innecesarios de build.

Comandos útiles:

```bash
docker build -t rentingprototype:local .
docker run --rm -p 8080:8080 rentingprototype:local
docker compose up --build
```

## 5) Endpoints principales

- `GET /vehicles/{id}`
- `GET /vehicles/`
- `GET /vehicles/available`
- `POST /vehicles/`
- `POST /rentals/rent-vehicle`
- `POST /rentals/return-vehicle`
- `GET /rentalhistory/vehicles/{id}/rental-history`
- `GET /rentalhistory/customers/{id}/rental-history`

## 6) Ejemplos rápidos (curl)

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

## 7) Tests

Desde raíz:

```bash
dotnet test RentingPrototype.sln
```

Estado verificado el **2026-03-08**:
- Unit tests: **34/34 OK**
- Integration tests: **14/14 OK**
- Host tests: **5/5 OK**
- Total: **53/53 OK**

## 8) Propuestas de mejora (por evaluador - se incluye estado de las mismas)

1. Introducir ValueObjects:
  - Introducidos a modo de ejemplo ValueObjects para las propiedades VehicleId, LicensePlate y ManufactureDate de Vehicle.
  - Refactorizados hyandler, repos y tests.
  - Terminado 2026-03-10
2. Introducir DomainEvents
3. Mejora de arquitectura hexagonal
4. Mejora del manejo de excepciones
5. Añadir observabilidad
6. Añadir logging estructurado
