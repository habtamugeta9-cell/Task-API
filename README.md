# Task API — C# ASP.NET Core REST API

A **Task Management REST API** built with **C# 14**, **.NET 10**, **ASP.NET Core 10 Web API**, **Entity Framework Core 10**, and **PostgreSQL**.

This project is a practical **backend development and software engineering project** designed to demonstrate how a modern REST API evolves from a simple CRUD application into a tested, secure, containerized, and deployable production-style backend.

The project is developed incrementally using a **commit-based roadmap**, where each Git commit represents a specific engineering milestone.

---

## Project Overview

Task API is a .NET 10 backend project designed to model a realistic task-management service from the ground up. It follows a structured engineering roadmap where each milestone adds a new layer of backend maturity: domain modeling, persistence, validation, service design, testing, and error handling.

The project is intentionally built as a learning-focused but production-minded API. The current implementation already includes a clean ASP.NET Core controller/service architecture, EF Core persistence, query filtering and pagination, robust validation, and an end-to-end integration test suite.

This repository demonstrates how a small but real API evolves from a simple CRUD application into a professional service that is easier to test, reason about, and extend.

### Architectural blueprint

```text
TaskApi
├── Authorization
│   ├── Policies
│   └── Roles
├── Controllers
│   ├── AuthController
│   └── TasksController
├── Services
│   ├── CurrentUserService
│   ├── ICurrentUserService
│   ├── Auth
│   │   ├── AuthService
│   │   ├── IAuthService
│   │   ├── IJwtTokenService
│   │   ├── JwtOptions
│   │   └── JwtTokenService
│   └── Tasks
│       ├── ITaskQueryBuilder
│       ├── ITaskService
│       ├── TaskQueryBuilder
│       └── TaskService
├── Queries
│   └── QueryService
├── Domain
│   ├── TaskItem
│   └── User
├── DTOs
│   ├── Auth
│   │   ├── AuthResponse
│   │   ├── LoginRequest
│   │   ├── RefreshRequest
│   │   └── RegisterRequest
│   ├── CreateTaskRequest
│   ├── UpdateTaskRequest
│   ├── TaskResponse
│   ├── PagedTaskResponse
│   └── TaskQuery
├── Data
│   └── AppDbContext
├── Errors
│   └── GlobalExceptionHandler
├── Tests
│   ├── TaskServiceTests
│   ├── TaskRequestValidationTests
│   ├── TasksApiIntegrationTests
│   └── GlobalExceptionHandlerTests
├── Program.cs
├── TaskApi.csproj
├── appsettings.json
├── appsettings.Development.json
├── global.json
└── README.md
```

### What the project demonstrates

* RESTful API design
* Dependency injection and service composition
* DTO-based API contracts
* EF Core persistence with PostgreSQL-ready setup
* Query filtering, sorting, and pagination
* Validation and business-rule enforcement
* Global exception handling with ProblemDetails
* Structured logging and diagnostics
* Unit and integration testing with xUnit
* A clear milestone-driven engineering workflow

---

## Technology Stack

| Technology | Purpose |
| --- | --- |
| **C# 14** | Application language |
| **.NET 10** | Runtime and framework |
| **ASP.NET Core 10** | REST API server |
| **Entity Framework Core 10** | ORM and persistence layer |
| **PostgreSQL** | Relational database |
| **xUnit** | Automated testing |
| **OpenAPI** | API metadata and documentation |
| **Git / GitHub** | Version control and collaboration |

---

## Core Features

The project currently covers the following capabilities through COMMIT 17:

### Task Management

* Create tasks
* Read all tasks
* Read a task by ID
* Update tasks
* Delete tasks
* Complete and reopen tasks via the domain model

### Querying and API Behavior

* Search by title or description
* Filter by completion state
* Sort by supported fields
* Paginate results with metadata
* Validate invalid page, page size, and sort options

### Authentication and Authorization

* Register and authenticate users with JWT access tokens
* Rotate refresh tokens
* Assign new accounts the `User` role by default
* Associate tasks with the authenticated user
* Restrict list, read, update, and delete operations to task owners
* Allow administrators to access tasks across owners
* Protect `GET /api/tasks/all` with the admin-only policy

### Advanced API Features

* PATCH complete/uncomplete operations
* Optimistic concurrency with task versions
* HTTP ETag / If-Match protection
* Per-user short-lived task caching
* API rate limiting
* Authentication-specific rate limiting

### Reliability and Quality

* Input validation on DTOs and domain rules
* Consistent HTTP problem responses
* Global exception handling
* Logging for service operations and failures
* Unit tests for business logic
* Integration tests for end-to-end API behavior

### Architecture

* Controllers for HTTP concerns
* Services for application logic
* Domain model for task behavior
* DTOs for request and response contracts
* EF Core `DbContext` for data access
* Dependency injection for decoupled design

---

## API Surface

The API currently supports the following endpoints:

```http
GET    /api/tasks
GET    /api/tasks/all
GET    /api/tasks/{id}
POST   /api/tasks
PUT    /api/tasks/{id}
PATCH  /api/tasks/{id}/complete
PATCH  /api/tasks/{id}/uncomplete
DELETE /api/tasks/{id}
```

All task endpoints require a valid bearer token. `GET /api/tasks` returns only the caller's tasks; `GET /api/tasks/all` is restricted to administrators. Requests for another user's task return `404 Not Found` to avoid revealing whether the task exists. `PUT`, `PATCH`, and `DELETE` task mutations require an `If-Match` header containing the ETag returned by `GET /api/tasks/{id}`.

---

## Development Roadmap

The project is divided into **24 commit-based engineering milestones**.

The development rule is:

> **One commit = one clear engineering goal, working code, and an understandable change.**

---

## COMMIT 01 — Project Foundation

**Goal:** Create and prepare the ASP.NET Core 10 Web API project.

### Tasks

* Create ASP.NET Core 10 Web API
* Configure the project
* Remove unnecessary template files
* Add `.gitignore`
* Verify build
* Verify application startup
* Initialize Git
* Connect GitHub repository
* Create initial commit

### Commit

```bash
git add .
git commit -m "chore: initialize task api project"
```

---

## COMMIT 02 — Task Domain Model

**Goal:** Define the core Task domain.

### Tasks

* Remove `WeatherForecast`
* Create the `Task` domain model
* Define task properties
* Define basic domain rules
* Build the application

### Core domain model

```text
Task
├── Id
├── Title
├── Description
├── IsCompleted
├── CreatedAt
└── UpdatedAt
```

### Commit

```bash
git add .
git commit -m "feat: add task domain model"
```

---

## COMMIT 03 — Task Read API

**Goal:** Build the first REST API endpoints.

### Endpoints

```http
GET /api/tasks
GET /api/tasks/{id}
```

### Tasks

* Create `TasksController`
* Add temporary in-memory storage
* Return task collections
* Find tasks by ID
* Return correct HTTP status codes

### Commit

```bash
git add .
git commit -m "feat: add task read endpoints"
```

---

## COMMIT 04 — Task Write API

**Goal:** Complete basic CRUD operations.

### Endpoints

```http
POST   /api/tasks
PUT    /api/tasks/{id}
DELETE /api/tasks/{id}
```

### Tasks

* Create tasks
* Update tasks
* Delete tasks
* Handle missing resources
* Return appropriate HTTP responses

### Commit

```bash
git add .
git commit -m "feat: add task write endpoints"
```

---

## COMMIT 05 — Application Service Layer

**Goal:** Separate HTTP concerns from business logic.

### Tasks

* Create `ITaskService`
* Create `TaskService`
* Move business logic out of controllers
* Register services with Dependency Injection
* Keep controllers thin

### Structure

```text
TaskApi/
├── Controllers/
│   └── TasksController
│
├── Services/
│   ├── ITaskService
│   └── TaskService
│
└── Domain/
    └── Task
```

### Commit

```bash
git add .
git commit -m "refactor: introduce task service layer"
```

---

## COMMIT 06 — Data Transfer Objects

**Goal:** Separate API contracts from domain models.

### Create

```text
CreateTaskRequest
UpdateTaskRequest
TaskResponse
```

### Goals

* Control incoming API data
* Control API responses
* Prevent direct domain exposure
* Establish a clear API boundary

### Commit

```bash
git add .
git commit -m "feat: add task request and response dtos"
```

---

## COMMIT 07 — Request Validation

**Goal:** Prevent invalid task data.

### Tasks

* Require task title
* Limit title length
* Limit description length
* Validate requests
* Return consistent validation errors

### Examples

```text
Empty title     → 400 Bad Request
Title too long  → 400 Bad Request
Invalid request → 400 Bad Request
```

### Commit

```bash
git add .
git commit -m "feat: add task validation"
```

---

## COMMIT 08 — Entity Framework Core and PostgreSQL

**Goal:** Introduce the database layer without replacing the in-memory task service. Connecting `TaskService` to PostgreSQL is COMMIT 09.

EF Core 10 is the EF release line for .NET 10 ([what's new in EF Core 10](https://learn.microsoft.com/ef/core/what-is-new/ef-core-10.0/whatsnew)). `DbContext` represents a unit of work for querying and saving data ([EF Core overview](https://learn.microsoft.com/ef/core/)); the Npgsql provider connects EF Core to PostgreSQL.

```text
COMMIT 08
├── Add EF Core and PostgreSQL packages
├── Add EF Core Design tools
├── Create AppDbContext
├── Map TaskItem to the tasks table
├── Configure the database connection
├── Register DbContext with dependency injection
└── Build and verify the existing API
```

### 1. Check PostgreSQL

On Debian or Ubuntu, check whether the PostgreSQL client and service are available:

```bash
psql --version
sudo systemctl status postgresql
```

If PostgreSQL is not installed, install and start it:

```bash
sudo apt update
sudo apt install postgresql postgresql-contrib
sudo systemctl enable --now postgresql
```

These commands prepare a local server for COMMIT 09. The API can build and start without connecting to the database in this milestone.

### 2. Add EF Core packages and tools

Keep the EF Core runtime, provider, design package, and CLI on compatible EF 10 versions. The project currently uses EF Core 10.0.12 and Npgsql 10.0.3:

```bash
dotnet add package Microsoft.EntityFrameworkCore --version 10.0.12
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 10.0.3
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.12
```

The Design package is private to this project and supports EF tooling. Install the matching CLI if needed:

```bash
dotnet tool install --global dotnet-ef --version 10.0.12
dotnet ef --version
```

If `dotnet-ef` is already installed, use `dotnet tool update --global dotnet-ef --version 10.0.12` instead.

### 3. Create `AppDbContext`

Add `Data/AppDbContext.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using TaskApi.Domain;

namespace TaskApi.Data;

/// <summary>
/// Represents the Entity Framework Core database session for the Task API.
/// </summary>
public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("tasks");

            entity.HasKey(task => task.Id);

            entity.Property(task => task.Id)
                .ValueGeneratedNever();

            entity.Property(task => task.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(task => task.Description)
                .HasMaxLength(2000);

            entity.Property(task => task.IsCompleted)
                .IsRequired();

            entity.Property(task => task.CreatedAt)
                .IsRequired();

            entity.Property(task => task.UpdatedAt);
        });
    }
}
```

`DbSet<TaskItem>` exposes the task entities to EF Core. The explicit mapping sets the table name, key behavior, and column constraints. EF Core supports the domain model's private property setters, so `TaskItem` does not need to be renamed or made mutable for this milestone.

### 4. Configure the connection string

Add a local connection string to `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "TaskApiDatabase": "Host=localhost;Port=5432;Database=task_api;Username=postgres;Password=postgres"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

The username and password above are local placeholders; replace them with credentials for your machine. Never put a real or production password in a tracked settings file. Use development secrets or environment configuration for real credentials.

### 5. Register the context

In `Program.cs`, register the PostgreSQL provider and context:

```csharp
using Microsoft.EntityFrameworkCore;
using TaskApi.Data;

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("TaskApiDatabase"));
});
```

`AddDbContext` registers the context as a scoped service. `TaskService` remains backed by its in-memory list for now; COMMIT 09 will inject `AppDbContext` into the service and move persistence to PostgreSQL.

### 6. Build and verify

Build the project:

```bash
dotnet build
```

Verify the EF CLI and context configuration:

```bash
dotnet ef --help
dotnet ef dbcontext info
```

The context information command should report `TaskApi.Data.AppDbContext` and the Npgsql provider. It does not require creating a migration or changing the service's storage.

Start the API and confirm the existing endpoint still works:

```bash
dotnet run
curl -i http://localhost:5058/api/tasks
```

The request should return `HTTP/1.1 200 OK` with the in-memory tasks. The database schema and migration are COMMIT 09.

### Commit

Before committing, inspect `git status` and `git diff`, then stage only the intended milestone files:

```bash
git add TaskApi.csproj Program.cs appsettings.json Data/AppDbContext.cs README.md
git commit -m "feat: add ef core and postgresql"
```

The README progress tracker below already marks COMMIT 08 complete and COMMIT 09 pending.

---

## COMMIT 09 — Database Persistence

**Goal:** Store tasks permanently in PostgreSQL.

### Tasks

* Create initial migration
* Create database schema
* Replace in-memory storage
* Implement database CRUD
* Verify persistence

### Commit

```bash
git add .
git commit -m "feat: persist tasks with postgresql"
```

---

## COMMIT 10 — Search, Filtering, Sorting, and Pagination

**Goal:** Support realistic task queries.

### Features

* Search
* Filtering
* Sorting
* Pagination
* Pagination metadata

### Examples

```http
GET /api/tasks?completed=false
GET /api/tasks?search=backend
GET /api/tasks?page=1&pageSize=20
GET /api/tasks?sort=createdAt
```

### Commit

```bash
git add .
git commit -m "feat: add task filtering sorting and pagination"
```

---

## COMMIT 11 — Global Error Handling

**Goal:** Provide consistent API error responses.

### Tasks

* Global exception handling
* ProblemDetails
* 400 Bad Request
* 404 Not Found
* 500 Internal Server Error
* Prevent internal implementation details from leaking

### Commit

```bash
git add .
git commit -m "feat: add global error handling"
```

---

## COMMIT 12 — Logging

**Goal:** Improve observability and debugging.

### Tasks

* Structured logging
* Service-level logging
* Error logging
* Diagnostic information
* Avoid sensitive information in logs

### Commit

```bash
git add .
git commit -m "feat: add application logging"
```

---

## COMMIT 13 — Unit Testing

**Goal:** Test application logic independently.

### Tests

* Task creation
* Task updates
* Task deletion
* Validation
* Business rules
* Edge cases

### Commit

```bash
git add .
git commit -m "test: add task service unit tests"
```

---

## COMMIT 14 — Integration Testing

**Goal:** Test the REST API as a complete application.

### Tests

* HTTP endpoints
* Request validation
* HTTP status codes
* Response bodies
* Database interactions
* CRUD workflows

### Commit

```bash
git add .
git commit -m "test: add api integration tests"
```

---

## COMMIT 15 — JWT Authentication

**Goal:** Add user authentication.

### Features

```http
POST /api/auth/register
POST /api/auth/login
POST /api/auth/refresh
```

### Tasks

* User model
* Registration
* Password hashing
* Login
* JWT authentication
* Token validation
* Protected endpoints

### Commit

```bash
git add .
git commit -m "feat: add jwt authentication"
```

---

## COMMIT 16 — Authorization and Task Ownership

**Goal:** Control access to resources.

### Tasks

* Associate tasks with users
* Task ownership
* Roles
* Permissions
* Authorization policies
* Prevent unauthorized task access

### Migration note

The ownership migration requires an empty `tasks` table because existing tasks do not contain enough information to infer their owners. It stops with an explicit error when task rows exist; back up the database and assign those tasks to users before retrying. Existing users are assigned the `User` role during migration.

### Commit

```bash
git add .
git commit -m "feat: add task authorization"
```

---

## COMMIT 17 — Advanced API Features

**Goal:** Add production-oriented API capabilities.

### Features

* PATCH operations
* Complete / uncomplete task
* Optimistic concurrency
* Rate limiting
* Caching where appropriate

### Example

```http
PATCH /api/tasks/{id}/complete
```

### Commit

```bash
git add .
git commit -m "feat: add advanced task api features"
```

---

## COMMIT 18 — Production Configuration

**Goal:** Prepare the application for different environments.

### Tasks

* Development configuration
* Production configuration
* Environment variables
* Secret management strategy
* Health checks
* Production-safe configuration

### Commit

```bash
git add .
git commit -m "chore: add production configuration"
```

---

## COMMIT 19 — Docker and Docker Compose

**Goal:** Containerize the application and database.

### Tasks

* Create `Dockerfile`
* Create `compose.yaml`
* Containerize ASP.NET Core API
* Containerize PostgreSQL
* Configure networking
* Verify the complete local environment

### Architecture

```text
┌───────────────┐
│    Client     │
└───────┬───────┘
        │ HTTP
        ▼
┌───────────────┐
│   Task API    │
│ ASP.NET Core  │
└───────┬───────┘
        │
        ▼
┌───────────────┐
│  PostgreSQL   │
└───────────────┘
```

### Commit

```bash
git add .
git commit -m "feat: add docker support"
```

---

## COMMIT 20 — GitHub Actions CI/CD

**Goal:** Automate build and test verification.

### Pipeline

```text
Push
  ↓
Restore
  ↓
Build
  ↓
Test
  ↓
Pass / Fail
```

### Tasks

* Create GitHub Actions workflow
* Restore dependencies
* Build application
* Run tests
* Fail the workflow when tests fail

### Commit

```bash
git add .
git commit -m "ci: add github actions pipeline"
```

---

## COMMIT 21 — OpenAPI Documentation

**Goal:** Make the REST API understandable to other developers.

### Tasks

* Configure OpenAPI
* Document endpoints
* Document request bodies
* Document responses
* Add examples
* Document authentication

### Commit

```bash
git add .
git commit -m "docs: improve api documentation"
```

---

## COMMIT 22 — Production Deployment

**Goal:** Deploy the Task API for remote access.

### Tasks

* Choose a hosting provider
* Deploy ASP.NET Core API
* Deploy PostgreSQL
* Configure environment variables
* Configure HTTPS
* Run database migrations
* Verify production endpoints

### Commit

```bash
git add .
git commit -m "deploy: release task api"
```

---

## COMMIT 23 — Professional Project Documentation

**Goal:** Make the repository easy for developers and employers to understand.

### Documentation

* Project overview
* Features
* Architecture
* Technology stack
* API endpoints
* Local setup
* Database setup
* Docker setup
* Testing
* CI/CD
* Deployment
* API examples
* Future improvements

### Commit

```bash
git add .
git commit -m "docs: complete project documentation"
```

---

## COMMIT 24 — Version 1.0 Portfolio Release

**Goal:** Prepare the Task API as a finished portfolio project.

### Final checks

* Clean source structure
* All tests passing
* CI pipeline passing
* Production API working
* README complete
* Architecture diagram included
* API examples included
* GitHub repository polished
* Production deployment verified

### Release

```bash
git tag -a v1.0.0 -m "Task API v1.0.0"
git push origin v1.0.0
```

---

# Progress Tracker

Update this checklist as development progresses.

```text
[✅] 01 — Project Foundation
[✅] 02 — Task Domain Model
[✅] 03 — Task Read API
[✅] 04 — Task Write API
[✅] 05 — Application Service Layer
[✅] 06 — Data Transfer Objects
[✅] 07 — Request Validation
[✅] 08 — Entity Framework Core and PostgreSQL
[✅] 09 — Database Persistence
[✅] 10 — Search, Filtering, Sorting, Pagination
[✅] 11 — Global Error Handling
[✅] 12 — Logging
[✅] 13 — Unit Testing
[✅] 14 — Integration Testing
[✅] 15 — JWT Authentication
[✅] 16 — Authorization and Task Ownership
[✅] 17 — Advanced API Features
[ ] 18 — Production Configuration
[ ] 19 — Docker and Docker Compose
[ ] 20 — GitHub Actions CI/CD
[ ] 21 — OpenAPI Documentation
[ ] 22 — Production Deployment
[ ] 23 — Professional Project Documentation
[ ] 24 — Version 1.0 Portfolio Release
```

---

# C# and Backend Learning Map

This project is also a structured **C# backend development learning path**.

## C# 14

```text
C#
├── Classes and Objects
├── Records
├── Interfaces
├── Generics
├── Collections
├── LINQ
├── Nullable Reference Types
├── Exceptions
├── async / await
├── Dependency Injection
└── Modern C# Language Features
```

## ASP.NET Core 10

```text
ASP.NET Core
├── Controllers
├── Routing
├── Model Binding
├── Validation
├── Middleware
├── Configuration
├── Dependency Injection
├── Logging
├── Authentication
└── Authorization
```

## Entity Framework Core 10

```text
Entity Framework Core
├── DbContext
├── Entities
├── Entity Configuration
├── Relationships
├── LINQ Queries
├── Migrations
└── Transactions
```

## Backend Engineering

```text
Backend Development
├── HTTP
├── REST
├── JSON
├── Databases
├── API Design
├── Error Handling
├── Testing
├── Security
├── Docker
├── CI/CD
└── Deployment
```

---

# Development Workflow

Every milestone follows the same engineering cycle:

```text
Understand
    ↓
Design
    ↓
Implement
    ↓
Run
    ↓
Test
    ↓
Debug
    ↓
Refactor
    ↓
Document
    ↓
Commit
    ↓
Push
```

The goal is not simply to finish a **C# Task API**.

The goal is to learn how to independently design, build, test, debug, document, deploy, and maintain a modern backend application.

---

# Repository Structure

The final project is expected to evolve toward a structure similar to:

```text
TaskApi/
├── Controllers/
├── Domain/
├── DTOs/
├── Services/
│   ├── Auth/
│   └── Tasks/
├── Queries/
├── Data/
├── Middleware/
├── Tests/
├── Properties/
├── Program.cs
├── appsettings.json
├── Dockerfile
├── compose.yaml
├── TaskApi.csproj
├── README.md
└── .github/
    └── workflows/
```

The structure will evolve during development rather than being created all at once.

---

# Current Status

**Current milestone:** COMMIT 16 — Authorization and Task Ownership

**Next milestone:** COMMIT 17 — Advanced API Features

COMMIT 16 is complete. The API now includes user roles, task ownership, current-user resolution from JWT claims, owner-scoped task queries, and an admin-only authorization policy that prevents unauthorized access to other users' tasks.

### Verified project quality at this stage

* Task CRUD flow is covered end-to-end
* Validation rules are enforced consistently
* Search, filtering, and pagination behavior is tested
* User roles are part of the identity model
* Task ownership is enforced in the service layer
* Owner-based authorization prevents unauthorized reads, updates, and deletes
* Admin authorization policy allows cross-user task visibility when explicitly allowed
* Global exception handling returns problem details without leaking internal details

### Security features included

* User roles
* Task ownership
* Owner-based authorization
* Admin authorization policy
* User-isolated task queries
* New registrations cannot choose an administrative role
* The project stays within the planned milestone boundary without adding Commit 17 features

---

## Keywords

`C#` · `C# 14` · `.NET 10` · `ASP.NET Core 10` · `ASP.NET Core Web API` · `REST API` · `RESTful API` · `Task Management API` · `Task Management System` · `Entity Framework Core` · `EF Core` · `PostgreSQL` · `JWT Authentication` · `REST API Development` · `Backend Development` · `Backend Engineering` · `Docker` · `GitHub Actions` · `CI/CD` · `OpenAPI` · `xUnit` · `C# Backend Project`
