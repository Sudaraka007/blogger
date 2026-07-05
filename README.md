# Blogger API

ASP.NET Core Web API for authors and posts. Clean Architecture with MediatR CQRS and MySQL.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (optional, for MySQL)

## Projects

| Project | Role |
|---------|------|
| `Blogger.Api` | HTTP API, validation, exception handling |
| `Blogger.Domain` | Domain model, use cases, MediatR pipeline |
| `Blogger.Persistence` | EF Core, repositories, migrations |

## Run

```bash
dotnet restore
dotnet build
docker compose up -d db          # start MySQL
dotnet ef database update --project src/Blogger.Persistence   # apply migrations
dotnet run --project src/Blogger.Api
```

Default URLs: `http://localhost:5122` (HTTP), `https://localhost:7184` (HTTPS).

Docker (API + DB): `docker compose up --build` applies migrations on startup and serves the API at `http://localhost:8080`.

## Tests

```bash
dotnet test
```

| Project | Scope |
|---------|-------|
| `Blogger.Domain.Tests` | Handlers, validators, pipeline behaviors, logging |
| `Blogger.Persistence.Tests` | Repositories, DbContext, UnitOfWork |
| `Blogger.Api.Tests.Unit` | API validators, DI, exception handlers |
| `Blogger.Api.Tests.Integration` | Controllers with mocked `IMediator` |
| `Blogger.Api.Tests.E2E` | Full stack with in-memory DB |

## API

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/health` | Health check |
| `POST` | `/api/authors` | Create author |
| `GET` | `/api/authors/{id}` | Get author |
| `POST` | `/api/posts` | Create post |
| `GET` | `/api/posts/{id}` | Get post |

Use `?includeAuthor=true` on get-post endpoints to include author details.

Errors return RFC 7807 problem details (JSON).

## Configuration

Connection string in `appsettings.json` (`ConnectionStrings:DefaultConnection`). Use `Server=db` inside Docker Compose.