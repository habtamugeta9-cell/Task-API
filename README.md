# Task API — C# ASP.NET Core REST API

A **Task Management REST API** built with **C# 14**, **.NET 10**, **ASP.NET Core 10 Web API**, **Entity Framework Core 10**, and **PostgreSQL**.

This project is a practical **backend development and software engineering project** designed to demonstrate how a modern REST API evolves from a simple CRUD application into a tested, secure, containerized, and deployable production-style backend.

The project is developed incrementally using a **commit-based roadmap**, where each Git commit represents a specific engineering milestone.

---

## Project Overview

The Task API provides a backend service for creating, managing, searching, filtering, and completing tasks.

The project will progressively include:

* RESTful API design
* CRUD operations
* C# backend development
* ASP.NET Core Web API
* Entity Framework Core
* PostgreSQL database
* Dependency Injection
* DTOs and API contracts
* Input validation
* Global error handling
* Structured logging
* Unit testing
* Integration testing
* JWT authentication
* Authorization
* Pagination
* Filtering and sorting
* Rate limiting
* Caching
* Docker
* GitHub Actions CI/CD
* OpenAPI documentation
* Production deployment

---

# Technology Stack

| Technology                   | Purpose                      |
| ---------------------------- | ---------------------------- |
| **C# 14**                    | Programming language         |
| **.NET 10**                  | Runtime and SDK              |
| **ASP.NET Core 10**          | REST API / Web API framework |
| **Entity Framework Core 10** | ORM and database access      |
| **PostgreSQL**               | Relational database          |
| **OpenAPI**                  | API documentation            |
| **xUnit**                    | Automated testing            |
| **Docker**                   | Containerization             |
| **GitHub Actions**           | CI/CD                        |
| **Git / GitHub**             | Version control              |

---

# Core Features

The final Task API is planned to support:

### Task Management

* Create tasks
* Get all tasks
* Get a task by ID
* Update tasks
* Delete tasks
* Complete and uncomplete tasks

### Querying

* Search tasks
* Filter tasks
* Sort tasks
* Paginate results

### Backend Architecture

* Controllers
* Services
* Domain models
* DTOs
* Dependency Injection
* Entity Framework Core
* PostgreSQL

### Reliability

* Validation
* Global exception handling
* ProblemDetails responses
* Structured logging
* Unit tests
* Integration tests

### Security

* User registration
* User login
* Password hashing
* JWT authentication
* Authorization
* Task ownership
* Roles and permissions

### DevOps

* Docker
* Docker Compose
* GitHub Actions
* Automated build and test
* Environment-based configuration
* Health checks
* Production deployment

---

# API Roadmap

The API will progressively evolve toward endpoints such as:

```http
GET    /api/tasks
GET    /api/tasks/{id}
POST   /api/tasks
PUT    /api/tasks/{id}
PATCH  /api/tasks/{id}/complete
DELETE /api/tasks/{id}
```

Authentication will later introduce endpoints such as:

```http
POST /api/auth/register
POST /api/auth/login
POST /api/auth/refresh
```

---

# Development Roadmap

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

### Initial model

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

**Goal:** Introduce the database layer.

### Tasks

* Add Entity Framework Core
* Add PostgreSQL provider
* Create `AppDbContext`
* Configure the database connection
* Register `DbContext`
* Configure entity mapping

### Commit

```bash
git add .
git commit -m "feat: add ef core and postgresql"
```

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
[ ] 02 — Task Domain Model
[ ] 03 — Task Read API
[ ] 04 — Task Write API
[ ] 05 — Application Service Layer
[ ] 06 — Data Transfer Objects
[ ] 07 — Request Validation
[ ] 08 — Entity Framework Core and PostgreSQL
[ ] 09 — Database Persistence
[ ] 10 — Search, Filtering, Sorting, Pagination
[ ] 11 — Global Error Handling
[ ] 12 — Logging
[ ] 13 — Unit Testing
[ ] 14 — Integration Testing
[ ] 15 — JWT Authentication
[ ] 16 — Authorization and Task Ownership
[ ] 17 — Advanced API Features
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

**Current milestone:** COMMIT 01 — Project Foundation

**Next milestone:** COMMIT 02 — Task Domain Model

The next implementation step is to remove the generated `WeatherForecast` example and create the real `Task` domain model.

---

## Keywords

`C#` · `C# 14` · `.NET 10` · `ASP.NET Core 10` · `ASP.NET Core Web API` · `REST API` · `RESTful API` · `Task Management API` · `Task Management System` · `Entity Framework Core` · `EF Core` · `PostgreSQL` · `JWT Authentication` · `REST API Development` · `Backend Development` · `Backend Engineering` · `Docker` · `GitHub Actions` · `CI/CD` · `OpenAPI` · `xUnit` · `C# Backend Project`
