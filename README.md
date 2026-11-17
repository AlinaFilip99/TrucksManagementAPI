# TrucksManagementAPI

A .NET Web API for managing truck records and trip assignments. The project uses ASP.NET Core Identity for authentication and authorization, Entity Framework Core for persistence, and Swagger for local API exploration.

## Overview

This API exposes endpoints for:

- registering and managing user accounts
- assigning trucks to admins
- creating, updating, deleting, and querying trips
- checking which trucks are available within a date range

## Tech stack

- ASP.NET Core Web API
- .NET 10
- ASP.NET Core Identity
- Entity Framework Core
- SQL Server or SQLite
- Swagger / OpenAPI

## Requirements

- .NET 10 SDK
- A local database option:
  - SQLite (default for local Development config)
  - SQL Server / LocalDB (supported through the appsettings configuration)

## Run locally

From the repository root:

```bash
dotnet restore TrucksManagement.sln
dotnet run --project TrucksManagement/TrucksManagement.csproj
```

When the app runs in Development mode, Swagger UI is served at the root URL, often:

- `http://localhost:5000`
- `https://localhost:5001`

The app is configured to enable Swagger only in Development.

## Configuration

The database configuration is controlled by `TrucksManagement/appsettings.json` and `TrucksManagement/appsettings.Development.json`.

The app reads:

- `DatabaseProvider` (`SqlServer` or `Sqlite`)
- `ConnectionStrings:DefaultConnection`

Example configuration:

```json
{
  "DatabaseProvider": "Sqlite",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=TruckManagement.db"
  }
}
```

For SQL Server, a typical configuration is:

```json
{
  "DatabaseProvider": "SqlServer",
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TruckManagementDb;Trusted_Connection=True;ConnectRetryCount=0"
  }
}
```

The current default local development settings use SQLite:

```json
{
  "DatabaseProvider": "Sqlite",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=TruckManagement.db"
  }
}
```

## Database setup

The project includes EF Core migrations. To create or update the database, run:

```bash
cd TrucksManagement
dotnet ef database update
```

If the EF Core CLI is not installed, install it with:

```bash
dotnet tool install --global dotnet-ef
```

## API endpoints

All routes are defined without an `/api` prefix. Request and response payloads use JSON.

### Trips

| Method | Route | Description |
|---|---|---|
| `GET` | `/trip` | List all trips. |
| `GET` | `/trip/{tripId}` | Get a trip by ID. |
| `GET` | `/trip/truck/{truckId}` | List trips for a truck/user ID. |
| `GET` | `/trip/user/{adminId}` | List trips for trucks linked to an admin ID. |
| `POST` | `/trip` | Create a trip. |
| `PUT` | `/trip` | Update a trip. |
| `DELETE` | `/trip?id={tripId}` | Delete a trip by ID. |

Example trip payload:

```json
{
  "userId": "truck-user-id",
  "country": "United States",
  "region": "California",
  "city": "Los Angeles",
  "street": "Main Street",
  "moreAddress": "",
  "duration": "02:30",
  "startDateTime": "2026-11-18T08:00:00",
  "endDateTime": "2026-11-18T10:30:00",
  "isFinished": false,
  "isDoneInTime": false,
  "isActive": true
}
```

### Users and trucks

| Method | Route | Description |
|---|---|---|
| `POST` | `/user/signup` | Register a user. |
| `POST` | `/user/login` | Sign in using email and password. |
| `POST` | `/user/roles` | Get roles for an account. |
| `POST` | `/user/logout` | Sign out the current session. |
| `GET` | `/user/{email}` | Get an account by email. |
| `GET` | `/user/id/{userId}` | Get an account by ID. |
| `GET` | `/user/trucks` | List users in the `User` role. |
| `GET` | `/user/trucks/{adminId}` | List trucks assigned to an admin. |
| `GET` | `/user/available?adminId={id}&startDate={date}&endDate={date}` | Find trucks available during a date range. |
| `PUT` | `/user/edit` | Update account fields. |
| `DELETE` | `/user/delete?id={userId}` | Delete an account by ID. |

User signup accepts role names such as `Admin` and `User`. Identity enforces unique emails and password rules of at least eight characters, including uppercase, lowercase, and a number.

The request field `passwordHash` is the API's current name for the password value supplied to Identity; it should contain the raw password, not a precomputed hash.

Example availability request:

```text
GET /user/available?adminId=admin-id&startDate=2026-11-18T08:00:00&endDate=2026-11-18T10:30:00
```

## Project structure

- `TrucksManagement/Controllers` — HTTP endpoints for users and trips
- `TrucksManagement/Models` — application models, representations, and EF Core context
- `TrucksManagement/Services` — business logic
- `TrucksManagement/Repositories` — data access layer
- `TrucksManagement/Migrations` — EF Core schema migrations

## Notes

- Swagger is enabled for Development only.
- HTTPS redirection is enabled outside Development.
- CORS is currently configured to allow any origin, method, and header.
