# ADR-001: Layered Domain/Application/Infrastructure/Web solution
> Status: OBSERVED / ACCEPTED IN CODE  
> Last reviewed: 2026-09-13

## Context
The solution separates Domain, Application, Infrastructure and Web projects.

## Decision observed
Domain defines business entities/contracts; Application owns DTOs/services; Infrastructure implements persistence; Web composes services and presents MVC screens.

## Why / alternatives
The original rationale is Unknown / Not determined from code. Alternatives are not documented.

## Consequences
This supports separation, but direct `AppDbContext` use in `RolesController` and direct repositories in some controllers are deviations to address deliberately rather than silently extending.
