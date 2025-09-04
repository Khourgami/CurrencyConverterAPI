# Currency Converter API

## Overview
A robust, scalable, and maintainable **Currency Converter API** built with **C# and ASP.NET Core**.  
It integrates with the [Frankfurter API](https://www.frankfurter.app/) to provide real-time and historical exchange rates.

---

## Architecture

```
+----------------------------------------------------+
|                    Presentation                    |
|  (CurrencyConverter.Api - ASP.NET Core Controllers)|
+-----------------------------+----------------------+
                              |
                              v
+----------------------------------------------------+
|                  Application Layer                 |
|  - DTOs, Services, Interfaces                      |
|  - Business rules, Validation, Guards              |
+-----------------------------+----------------------+
                              |
                              v
+----------------------------------------------------+
|                  Domain Layer                      |
|  - Entities (DateRange, Conversion, etc.)          |
|  - Domain Exceptions (ValidationException, etc.)   |
|  - Core business logic                             |
+-----------------------------+----------------------+
                              |
                              v
+----------------------------------------------------+
|                Infrastructure Layer                |
|  - EF Core (SQLite)                                |
|  - Currency Providers (FrankfurterProvider)        |
|  - Repositories                                    |
|  - Resilience (Polly: Retry, Circuit Breaker)      |
|  - Caching & Rate Limiting                         |
+----------------------------------------------------+
```

---

## Features

- **Latest Exchange Rates** (per base currency)
- **Currency Conversion**
  - Excludes TRY, PLN, THB, MXN (returns BadRequest)
- **Historical Rates** with Pagination
- **Security**
  - JWT Authentication
  - Role-based Access Control (RBAC)
- **Resilience**
  - Caching (MemoryCache)
  - Retry with exponential backoff (Polly)
  - Circuit breaker for outages
  - Rate limiting per provider
- **Logging & Monitoring**
  - Serilog structured logging
  - OpenTelemetry tracing
- **Testing**
  - Unit tests (xUnit, FluentAssertions)
  - Integration tests (WebApplicationFactory, HttpClient)
  - Code coverage via Coverlet + ReportGenerator
- **Deployment**
  - Environment-based config (Dev/Test/Prod)
  - Dockerfile + docker-compose
  - Ready for horizontal scaling (K8s / containers)
- **API Versioning** (future-proofing)

---

## Setup Instructions

### Prerequisites
- .NET 9 SDK
- Docker & Docker Compose (for containerized deployment)

### Local Run
```bash
# Run API
dotnet run --project src/CurrencyConverter.Api

# Run Unit Tests
dotnet test tests/CurrencyConverter.Tests.Unit

# Run Integration Tests
dotnet test tests/CurrencyConverter.Tests.Integration
```

### Generate Coverage Report
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutput=TestResults/coverage.xml /p:CoverletOutputFormat=cobertura
reportgenerator -reports:TestResults/coverage.xml -targetdir:TestResults/coverage-report -reporttypes:Html
```

### Run with Docker
```bash
docker build -t currency-converter-api .
docker run -p 5000:8080 currency-converter-api
```

Or use docker-compose:
```bash
docker-compose up --build
```

---

## Environments

- `appsettings.Development.json`
- `appsettings.Test.json`
- `appsettings.Production.json`

Configured for SQLite (Dev/Test) and scalable DB in Production.

---

## Assumptions

- SQLite used for demo & testing (lightweight, cross-platform).
- Rate limiting configured for Frankfurter API (5 requests/second).
- JWT authentication with User/Admin roles for demonstration.

---

## Future Enhancements

- Add more exchange rate providers (factory pattern already in place).
- Migrate DB from SQLite → PostgreSQL/SQL Server in production.
- Improve test coverage beyond current baseline (target 90%+).
- CI/CD pipeline with GitHub Actions (build, test, coverage, deploy).
- Kubernetes manifests for autoscaling.

---

## Repository

GitHub: [CurrencyConverterAPI](https://github.com/Khourgami/CurrencyConverterAPI)
