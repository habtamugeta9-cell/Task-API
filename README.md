# Task API

A modern RESTful Task Management API built with **C# 14**, **ASP.NET Core 10**, **Entity Framework Core 10**, and **PostgreSQL**.

This project is being built progressively as a practical backend engineering project. The goal is not only to create a working API, but to understand how a real backend evolves from a simple application into a tested, secure, containerized, and deployable service.

---

## 🎯 Project Goals

This project is designed to practice and demonstrate:

* Modern C# development
* ASP.NET Core Web API development
* RESTful API design
* Dependency Injection
* Clean application structure
* Entity Framework Core
* PostgreSQL
* Validation
* Error handling
* Logging
* Automated testing
* Authentication and authorization
* Docker
* CI/CD
* API documentation
* Production deployment

The project will be developed incrementally rather than building everything at once.

---

## 🛠️ Technology Stack

| Technology               | Purpose              |
| ------------------------ | -------------------- |
| C# 14                    | Programming language |
| .NET 10                  | Runtime / SDK        |
| ASP.NET Core 10          | Web API framework    |
| Entity Framework Core 10 | Data access / ORM    |
| PostgreSQL               | Relational database  |
| OpenAPI                  | API documentation    |
| xUnit                    | Testing              |
| Docker                   | Containerization     |
| GitHub Actions           | CI/CD                |
| Git / GitHub             | Version control      |

---

# 🧭 Development Roadmap

The project is divided into meaningful Git commits.

Each commit represents **one engineering milestone**.

The rule is:

> One commit = one clear goal + working code + understandable change.

---

## COMMIT 01 — Project Foundation

**Goal:** Create the initial ASP.NET Core project and prepare the repository.

### Tasks

* Create ASP.NET Core 10 Web API
* Configure the project
* Remove unnecessary generated files
* Add `.gitignore`
* Verify the application builds
* Verify the application runs
* Initialize Git
* Connect GitHub repository
* Create initial commit

### Expected result

A clean ASP.NET Core project that builds and runs locally.

### Commit

```bash
git add .
git commit -m "chore: initialize task api project"
```

---

# COMMIT 02 — Task Domain

**Goal:** Define what a Task is in the application.

### Tasks

* Remove `WeatherForecast`
* Create `Task` domain model
* Define task properties
* Define basic task rules
* Add appropriate types
* Build the application

### Initial Task model

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

# COMMIT 03 — Read API

**Goal:** Build the first working API endpoints.

### Endpoints

```http
GET /api/tasks
GET /api/tasks/{id}
```

### Tasks

* Create `TasksController`
* Add in-memory task collection
* Return task collections
* Find task by ID
* Return appropriate HTTP status codes

### Commit

```bash
git add .
git commit -m "feat: add task read endpoints"
```

---

# COMMIT 04 — Write API

**Goal:** Complete basic CRUD functionality.

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

# COMMIT 05 — Application Layer

**Goal:** Separate API concerns from business logic.

### Tasks

* Create `ITaskService`
* Create `TaskService`
* Move business logic out of the controller
* Register the service with Dependency Injection
* Keep controllers thin

### Structure

```text
TaskApi
├── Controllers
│   └── TasksController
│
├── Services
│   ├── ITaskService
│   └── TaskService
│
└── Domain
    └── Task
```

### Commit

```bash
git add .
git commit -m "refactor: introduce task service layer"
```

---

# COMMIT 06 — DTOs

**Goal:** Stop exposing domain models directly through the API.

### Tasks

Create:

```text
CreateTaskRequest
UpdateTaskRequest
TaskResponse
```

### Goals

* Separate API contracts from domain models
* Control incoming data
* Control outgoing responses
* Establish a clean API boundary

### Commit

```bash
git add .
git commit -m "feat: add task request and response dtos"
```

---

# COMMIT 07 — Validation

**Goal:** Prevent invalid task data from entering the application.

### Tasks

* Required title
* Title length limits
* Description limits
* Validate incoming requests
* Return consistent validation errors

### Examples

```text
Empty title        → 400 Bad Request
Title too long     → 400 Bad Request
Invalid request    → 400 Bad Request
```

### Commit

```bash
git add .
git commit -m "feat: add task validation"
```

---

# COMMIT 08 — Database Foundation

**Goal:** Replace temporary in-memory storage with a real database foundation.

### Tasks

* Add Entity Framework Core
* Add PostgreSQL provider
* Create `AppDbContext`
* Configure the database
* Register `DbContext`
* Configure entity mapping

### Commit

```bash
git add .
git commit -m "feat: add ef core and postgresql"
```

---

# COMMIT 09 — Database Persistence

**Goal:** Persist tasks in PostgreSQL.

### Tasks

* Create initial EF migration
* Create database
* Replace in-memory storage
* Implement database CRUD
* Verify persistence after application restart

### Commit

```bash
git add .
git commit -m "feat: persist tasks with postgresql"
```

---

# COMMIT 10 — Query Features

**Goal:** Make the API useful for real collections of tasks.

### Features

* Filtering
* Searching
* Sorting
* Pagination
* Pagination metadata

### Example

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

# COMMIT 11 — Error Handling

**Goal:** Establish consistent API error responses.

### Tasks

* Global exception handling
* ProblemDetails
* 400 responses
* 404 responses
* 500 responses
* Avoid leaking internal implementation details

### Commit

```bash
git add .
git commit -m "feat: add global error handling"
```

---

# COMMIT 12 — Logging

**Goal:** Make the application observable and easier to debug.

### Tasks

* Structured logging
* Service-level logging
* Error logging
* Useful diagnostic information
* Avoid logging sensitive data

### Commit

```bash
git add .
git commit -m "feat: add application logging"
```

---

# COMMIT 13 — Unit Testing

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

# COMMIT 14 — Integration / API Testing

**Goal:** Test the API as an actual application.

### Tests

* HTTP requests
* HTTP status codes
* Request validation
* Response bodies
* Database interactions
* CRUD behavior

### Commit

```bash
git add .
git commit -m "test: add api integration tests"
```

---

# COMMIT 15 — Authentication

**Goal:** Introduce user identity and authentication.

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

# COMMIT 16 — Authorization

**Goal:** Ensure users can access only what they are allowed to access.

### Tasks

* Associate tasks with users
* Task ownership
* Roles
* Permissions
* Authorization policies
* Prevent cross-user access

### Commit

```bash
git add .
git commit -m "feat: add task authorization"
```

---

# COMMIT 17 — Advanced API Features

**Goal:** Add more realistic backend behavior.

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

# COMMIT 18 — Production Configuration

**Goal:** Prepare the application for real environments.

### Tasks

* Environment-based configuration
* Development configuration
* Production configuration
* Environment variables
* Secret management strategy
* Health checks

### Commit

```bash
git add .
git commit -m "chore: add production configuration"
```

---

# COMMIT 19 — Docker

**Goal:** Make the application reproducible and portable.

### Tasks

* Create `Dockerfile`
* Create `docker-compose.yml`
* Containerize API
* Containerize PostgreSQL
* Configure networking
* Verify complete local environment

### Expected architecture

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

# COMMIT 20 — CI/CD

**Goal:** Automatically verify every change.

### GitHub Actions pipeline

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
* Fail pipeline when tests fail

### Commit

```bash
git add .
git commit -m "ci: add github actions pipeline"
```

---

# COMMIT 21 — API Documentation

**Goal:** Make the API easy for other developers to understand.

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

# COMMIT 22 — Production Deployment

**Goal:** Deploy the API so it can be accessed remotely.

### Tasks

* Choose hosting provider
* Deploy API
* Deploy PostgreSQL
* Configure environment variables
* Configure HTTPS
* Run database migrations
* Verify production API
* Test production endpoints

### Commit

```bash
git add .
git commit -m "deploy: release task api"
```

---

# COMMIT 23 — Professional README

**Goal:** Turn the repository into a professional portfolio project.

### README sections

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

# COMMIT 24 — Portfolio Release

**Goal:** Prepare the project for employers and public review.

### Final checks

* Clean repository
* Clean source structure
* All tests passing
* CI passing
* Production API working
* README complete
* Architecture diagram included
* Example requests included
* GitHub repository polished
* Add release tag

### Release

```bash
git tag -a v1.0.0 -m "Task API v1.0.0"
git push origin v1.0.0
```

---

# 📊 Progress Tracker

Update this section as the project progresses.

```text
[✅] 01 — Project Foundation
[ ] 02 — Task Domain
[ ] 03 — Read API
[ ] 04 — Write API
[ ] 05 — Application Layer
[ ] 06 — DTOs
[ ] 07 — Validation
[ ] 08 — Database Foundation
[ ] 09 — Database Persistence
[ ] 10 — Query Features
[ ] 11 — Error Handling
[ ] 12 — Logging
[ ] 13 — Unit Testing
[ ] 14 — Integration Testing
[ ] 15 — Authentication
[ ] 16 — Authorization
[ ] 17 — Advanced API Features
[ ] 18 — Production Configuration
[ ] 19 — Docker
[ ] 20 — CI/CD
[ ] 21 — API Documentation
[ ] 22 — Production Deployment
[ ] 23 — Professional README
[ ] 24 — Portfolio Release
```

---

# 🧠 Learning Map

The project is also a C# and backend learning path.

```text
C#
├── Classes
├── Records
├── Interfaces
├── Generics
├── Collections
├── LINQ
├── Nullable Reference Types
├── Exceptions
├── async / await
└── Dependency Injection

ASP.NET Core
├── Controllers
├── Routing
├── Model Binding
├── Validation
├── Middleware
├── Configuration
├── Logging
├── Authentication
└── Authorization

Entity Framework Core
├── DbContext
├── Entities
├── Relationships
├── LINQ queries
├── Migrations
└── Transactions

Backend Engineering
├── REST
├── HTTP
├── Databases
├── Error handling
├── Testing
├── Security
├── Docker
├── CI/CD
└── Deployment
```

---

# 🔄 Development Cycle

Every milestone follows the same process:

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

Do not commit code that you do not understand.

The objective is not simply to finish the API.

The objective is to become capable of designing, building, testing, debugging, and explaining a backend system independently.

---

# 🚀 Current Status

**Current milestone:** COMMIT 01 — Project Foundation

**Next milestone:** COMMIT 02 — Task Domain

The next implementation step is to remove the generated `WeatherForecast` example and introduce the real `Task` domain.
