# Minimal API

A minimal .NET 9 solution providing a small API surface, authentication, persistence and a clear separation of concerns across layers.

Status: Not hosted yet — runs locally.

## Overview / Architecture

This solution follows a lightweight DDD layering:

- `Minimal.API` — HTTP endpoints, authentication, OpenAPI
- `Minimal.Application` MediatR handlers, DTOs, validation, application services
- `Minimal.Domain` — entities, domain rules, Identity related domain models
- `Minimal.Infrastructure` — database provider, JWT/token helpers, external integrations

The goal: keep domain logic in `Minimal.Domain`, orchestrate use-cases in `Minimal.Application`, and keep technical concerns in `Minimal.Infrastructure` / `Minimal.API`.

See the architecture diagram (simple DDD flow) below for a quick depiction.

## Diagram

<img src="https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fwww.hibit.dev%2Fimages%2Fposts%2F2021%2Fddd_layers.png&f=1&nofb=1&ipt=6682157bf2d5be73f17b10cf8255fe2c1366c96f0ea28d8d60e3df29a9f78a16" alt="DDD Architecture" width="500">

## Requirements

- .NET 9 SDK
- PostgreSQL
- Docker: wip

## Installed packages (per project)

- `Minimal.API` (target: `net9.0`)
  - `MediatR` 13.1.0
  - `Microsoft.AspNetCore.Authentication.JwtBearer` 9.0.10
  - `NSwag.AspNetCore` 14.6.1
  - `Serilog.AspNetCore` 9.0.0
  - `Serilog.Sinks.File` 7.0.0
  - `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` 1.22.1

- `Minimal.Application` (target: `net9.0`)
  - `FluentValidation` 12.0.0
  - `FluentValidation.DependencyInjectionExtensions` 12.0.0
  - `MediatR` 13.1.0
  - `Microsoft.AspNetCore.Identity` 2.3.1

- `Minimal.Infrastructure` (target: `net9.0`)
  - `Npgsql.EntityFrameworkCore.PostgreSQL` 9.0.4
  - `System.IdentityModel.Tokens.Jwt` 8.14.0

- `Minimal.Domain` (target: `net9.0`)
  - `Microsoft.AspNetCore.Identity.EntityFrameworkCore` 9.0.4

Notes:
- Versions shown come from project files in the solution.
- `NSwag` provides OpenAPI / Swagger generation.
- `Serilog` is configured for structured logging and file sinks.
- `MediatR` is used for handler-based application flows.
- `FluentValidation` for request validation.

## How to run locally

1. Restore & build

## How to run migrations
- ## dotnet ef
    run all the commands from the solution folder
    ```bash
    dotnet ef Migrations Add InitialMigration -p Minimal.Infrastructure -s Minimal.Infrastructure -c MinimalApiDbContext
    ```
    ```bash
    dotnet ef Migrations Remove -p Minimal.Infrastructure -s Minimal.Infrastructure -c MinimalApiDbContext
    ```

- ## dotnet database
    run all the commands from the solution folder
    ```bash
    dotnet ef database update -p Minimal.Infrastructure -s Minimal.Infrastructure -c MinimalApiDbContext
    ```
    ```bash
    dotnet ef database update 0 -p Minimal.Infrastructure -s Minimal.Infrastructure -c MinimalApiDbContext
    ```