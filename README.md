# SalesPlatform

A multi-country sales report upload platform: sales reps upload monthly Excel reports,
managers/admins can see and search across everyone's data.

Backend — .NET 8, Clean Architecture (Domain/Application/Infrastructure/Api) + CQRS
(MediatR). List/read queries go through GraphQL (HotChocolate); commands (upload/update/
delete) stay REST. Frontend — React + TypeScript + Ant Design, built with Vite.

## Prerequisites

- **.NET 8 SDK** — https://dotnet.microsoft.com/download/dotnet/8.0
- **Node.js 20+ (LTS)** — https://nodejs.org (npm ships with it)
- **SQL Server LocalDB** — usually already installed alongside Visual Studio; if not,
  it's a component of [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads)
  ("SQL Server Express LocalDB" in the installer's feature list)

Check everything is installed:

```bash
dotnet --version   # 8.x or newer
node --version     # v20.x or newer
sqllocaldb info    # should list at least one instance, e.g. MSSQLLocalDB
```

## Quick start (Windows / PowerShell)

From the repo root:

```powershell
.\dev.ps1
```

Starts the backend (`http://localhost:5213`) and frontend (`http://localhost:5173`), each in
its own window. If a port is already in use, the script detects it and skips that one instead
of spawning a duplicate process.

## Running manually

### Backend

```bash
cd src/backend/SalesPlatform.Api
dotnet run --urls http://localhost:5213
```

On startup, in Development mode, the backend automatically:
- applies EF Core migrations to LocalDB (the `SalesPlatform` database is created
  automatically — connection string in `src/backend/SalesPlatform.Api/appsettings.json`),
- seeds the test users below, if the `Users` table is empty.

No manual migration/seed step is needed.

Swagger (REST side of the API) — http://localhost:5213/swagger.
GraphQL playground (Reports/Records) — http://localhost:5213/graphql.

### Frontend

```bash
cd src/frontend
npm install
npm run dev
```

Opens at http://localhost:5173. `VITE_API_URL` (the backend's address) is already set in the
committed `src/frontend/.env.development` — nothing extra to configure.

**Important:** the frontend won't work without the backend running — the backend only allows
CORS from `http://localhost:5173` (see `Program.cs`), so requests from any other origin get
blocked by the browser.

## Test users (seeded automatically)

Password for all of them: `p@ssw0rd*-`

| Email | Role | Data visibility |
|---|---|---|
| `anna.kovacs@test.com` | SalesRep | own reports/records only |
| `jan.kowalski@test.com` | SalesRep | own reports/records only |
| `manager@test.com` | Manager | all reports/records, can edit/delete any |
| `admin@test.com` | Admin | all reports/records, can edit/delete any |

## Tests

```bash
cd src/backend
dotnet test
```

