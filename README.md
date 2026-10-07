# Fleet Management API

REST API for fleet and vehicle management built with C# and ASP.NET Core.

The project originally started as a Task Manager API and is now being developed as a transport-focused fleet management application.

The legacy Task Manager version remains deployed on Azure, while the current codebase contains the new fleet management backend.

## Legacy Live Demo

The currently deployed Azure application still represents the original Task Manager version:

https://taskmanager-api.wonderfultree-2214831b.polandcentral.azurecontainerapps.io/scalar/

The current Fleet Management version has not yet replaced the legacy deployment.

## Current Features

- User registration and login
- JWT authentication
- Role-based authorization
- `Admin`, `Employee` and `Viewer` roles
- Administrative user role management
- Vehicle management
- Vehicle service history
- Multiple service items per service record
- Initial role and admin seeding


## Roles

### Admin

- View and manage vehicles
- View and add service records
- View users
- Assign `Viewer` and `Employee` roles

### Employee

- View and manage vehicles
- View and add service records

### Viewer

- View vehicles
- View service history

Newly registered users do not receive fleet access automatically.

An administrator must assign an appropriate role before the user can access fleet resources.

## API Endpoints

### Authentication

```text
POST /api/auth/register
POST /api/auth/login
```

### Users

```text
GET /api/users
PUT /api/users/{id}/role
```

Admin only.

### Vehicles

```text
GET  /api/vehicles
GET  /api/vehicles/{id}
POST /api/vehicles
PUT  /api/vehicles/{id}
DELETE /api/vehicles/{id}
```

### Service History

```text
GET  /api/vehicles/{vehicleId}/service-records
POST /api/vehicles/{vehicleId}/service-records
```

## Current Tech Stack

- C#
- .NET 10
- ASP.NET Core Web API
- ASP.NET Core Identity
- Entity Framework Core
- SQLite
- JWT Bearer Authentication
- Scalar / OpenAPI
- xUnit

## Data Model

The service history model uses the following relationship:

```text
Vehicle
└── ServiceRecord
    └── ServiceRecordItem
```

This allows one service event to contain multiple performed maintenance operations.

## Running Locally

Clone the repository:

```bash
git clone <repository-url>
```

Navigate to the API project:

```bash
cd TaskManagerAPI
```

Restore dependencies:

```bash
dotnet restore
```

Apply migrations:

```bash
dotnet ef database update
```

Configure JWT:

```bash
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "your-long-random-secret-key-min-32-chars"
dotnet user-secrets set "Jwt:Issuer" "TaskManagerAPI"
```

Configure the initial administrator:

```bash
dotnet user-secrets set "Admin:Username" "admin"
dotnet user-secrets set "Admin:Password" "your-strong-admin-password"
```

Run the application:

```bash
dotnet run
```

The application automatically creates the `Admin`, `Employee` and `Viewer` roles.

If administrator credentials are configured, the initial Admin account is also created automatically.



## Legacy Deployment Stack

The original Task Manager version was containerized and deployed using:

- Docker
- Azure Container Registry
- Azure Container Apps
- Azure Files
- Azure environment variables and secrets

The Fleet Management version is planned to use separate environments:

```text
Demo
→ separate application instance
→ separate database
→ predefined users and fleet data

Production
→ separate application instance
→ separate database
→ no demo data
```

## Next Development Stage

A TypeScript frontend is planned next.

Initial frontend scope:

- Login
- Vehicle list
- Vehicle details
- Service history
- Adding service records
- User and role management for Admin

Further backend development may include:

- Maintenance planning
- Insurance and document tracking
- PostgreSQL migration
- CI/CD
- Filtering and pagination

## Known Dependency Warning

The project currently reports a security warning for the transitive dependency:

```text
SQLitePCLRaw.lib.e_sqlite3 2.1.11
GHSA-2m69-gcr7-jv3q / CVE-2025-6965
```

The dependency is part of the EF Core / SQLite stack and should be updated after compatibility with a patched version is verified.