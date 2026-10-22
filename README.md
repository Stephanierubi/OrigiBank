# OrigiBank

API de banca digital en **ASP.NET Core Web API (.NET 10)** con arquitectura limpia, Entity Framework Core y SQLite.

## Requisitos

- .NET 10 SDK
- Herramienta EF Core: `dotnet tool install --global dotnet-ef`

## Ejecución

```bash
dotnet restore
dotnet ef database update -p src/OrigiBank.Infrastructure -s src/OrigiBank.Api
dotnet run --project src/OrigiBank.Api
```

La API queda en `http://localhost:5160`. En desarrollo, el documento OpenAPI está en `/openapi/v1.json`.

## Pruebas

```bash
dotnet test
```

## Arquitectura

```
Api  ->  Application  ->  Domain
 |            ^
 +-> Infrastructure
```

- **Domain**: entidades (`Customer`, `Account`, `Transaction`), enums y excepciones de negocio. Las reglas viven en las entidades.
- **Application**: interfaces de servicios y repositorios, servicios, DTOs y generador de números de cuenta.
- **Infrastructure**: `DbContext`, configuraciones EF, repositorios y `UnitOfWork` (SQLite).
- **Api**: controladores delgados y `GlobalExceptionHandler` (`IExceptionHandler`).

Los controladores nunca usan el `DbContext`: solo invocan servicios de aplicación y devuelven DTOs.

## Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/customers` | Crear perfil de cliente |
| POST | `/api/accounts` | Crear cuenta (customerId + saldo inicial) |
| GET | `/api/accounts/{accountNumber}/balance` | Consultar saldo |
| POST | `/api/accounts/{accountNumber}/deposits` | Depósito |
| POST | `/api/accounts/{accountNumber}/withdrawals` | Retiro |
| GET | `/api/accounts/{accountNumber}/transactions` | Historial cronológico |

### Ejemplo

```bash
curl -X POST http://localhost:5160/api/customers -H "Content-Type: application/json" \
  -d '{"fullName":"Ana Perez","dateOfBirth":"1990-05-10","gender":"Female","monthlyIncome":1500}'
```

`gender` acepta `Male`, `Female`, `Other`.

## Manejo de errores

Las reglas de negocio lanzan excepciones personalizadas que el manejador global traduce a JSON (`ProblemDetails`):

| Excepción | HTTP |
|---|---|
| `ValidationException`, `InsufficientFundsException` | 400 |
| `NotFoundException` | 404 |
| `ConflictException` | 409 |
| Error inesperado | 500 (mensaje genérico) |

## Decisiones y supuestos

- Número de cuenta: `ACC-YYYYMMDD-XXXX` (sin corchetes), fecha en UTC, sufijo secuencial con inicio aleatorio.
- Solo hay 10 000 combinaciones por día: el servicio verifica y reintenta (hasta 10 veces) y el índice único de `AccountNumber` es la última defensa.
- Se usa `decimal` para dinero y fechas en UTC.
- Concurrencia: token de concurrencia (`Version`) en `Account`.
- Saldo e historial se guardan en una sola operación atómica (`SaveChanges`).
- El historial guarda el saldo resultante de cada movimiento (`balanceAfter`).