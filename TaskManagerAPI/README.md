# Task Manager API

A REST API for managing users, projects and tasks, built with C# and ASP.NET Core.

The application allows users to register and log in using JWT authentication. Each authenticated user can create and manage their own projects and tasks.

## Features

* User registration and login
* JWT authentication
* Password hashing
* User-specific project ownership
* User-specific task access
* Create and retrieve projects
* Create, retrieve, update and delete tasks
* Update task completion status
* Entity Framework Core
* SQLite database
* DTO-based request and response models
* Input validation
* Unit tests
* Integration tests
* OpenAPI documentation with Scalar

## Tech Stack

* C#
* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* SQLite
* JWT Bearer Authentication
* BCrypt
* xUnit
* EF Core InMemory provider for tests
* Scalar / OpenAPI

## Project Structure

```text
TaskManagerAPI/
├── Controllers/
├── Data/
├── DTOs/
├── Models/
├── Services/
├── Migrations/
└── Program.cs

TaskManagerAPI.Tests/
├── AuthServiceTests.cs
├── ProjectServiceTests.cs
├── TaskServiceTests.cs
└── IntegrationTests.cs
```

# Main API Endpoints

The API provides endpoints for creating, retrieving, updating and deleting tasks, as well as updating task status and retrieving tasks belonging to a project.

Protected endpoints require a valid JWT bearer token.

### Authentication

POST /api/auth/register
POST /api/auth/login

### Projects

GET    /api/projects
GET    /api/projects/{id}
POST   /api/projects

### Tasks

GET    /api/tasks
GET    /api/tasks/{id}
GET    /api/tasks/project/{projectId}
POST   /api/tasks
PUT    /api/tasks/{id}
PATCH  /api/tasks/{id}/status
DELETE /api/tasks/{id}}

## Authentication

After a successful login, the API returns a JWT token.

The token should be included in protected requests using the Authorization header:

```text
Authorization: Bearer <token>
```

The authenticated user's ID is obtained from the token and is used to restrict access to resources owned by that user.

## Data Model

The main relationship is:

```text
User
 └── Projects
      └── Tasks
```

A project belongs to a user, and a task belongs to a project.

The service layer verifies ownership so that authenticated users can access only their own resources.

## Tests

The project contains both unit and integration tests.

Unit tests verify the business logic implemented in the service layer.

Integration tests verify complete HTTP request flows including authentication, authorization, controllers, services and database access.

Run the tests with:

```bash
dotnet test
```

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

## Configuration

Before running the application, set the JWT signing key using .NET User Secrets:

```bash
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "your-long-random-secret-key-min-32-chars"
dotnet user-secrets set "Jwt:Issuer" "TaskManagerAPI"
```

The key must be at least 32 characters long (256 bits) to satisfy the HMAC SHA-256 algorithm requirements.

Run the application:

```bash
dotnet run
```

The OpenAPI/Scalar interface can then be used to explore and test the API.

## Known Dependency Warning

The project currently reports a NuGet security warning for the transitive dependency:

```text
SQLitePCLRaw.lib.e_sqlite3 2.1.11
GHSA-2m69-gcr7-jv3q / CVE-2025-6965
```

The dependency is part of the EF Core / SQLite stack.

A manual major-version override is intentionally avoided until compatibility with the current EF Core stack is verified.

The dependency should be updated when a compatible patched version becomes available.

## Status

The core API functionality, authentication, authorization and automated tests are implemented.

Deployment and final repository cleanup are the remaining steps.