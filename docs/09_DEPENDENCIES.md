# Dependencies
> Status: IMPLEMENTED  
> Source of truth: `.csproj` files  
> Last reviewed: 2026-10-24

## Projects
All projects target `net10.0`. Application references Domain. Infrastructure references Application and Domain. Web references Application and Infrastructure.

## NuGet
| Package | Declared version | Purpose |
|---|---:|---|
| Microsoft.AspNetCore.Identity.UI | `10.*` | ASP.NET Core Identity UI dependency. |
| Microsoft.EntityFrameworkCore | `10.0.12` | EF Core ORM. |
| Microsoft.EntityFrameworkCore.SqlServer | `10.0.12` | SQL Server provider. |
| Microsoft.EntityFrameworkCore.Design/Tools | `10.0.12` | design-time and migration tooling. |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | `10.*` | EF-backed Identity stores. |

## Angular modernization workspace

The in-repository `MiniStore Front-End` workspace pins Node `24.20.0` through `.nvmrc`, declares npm `11.19.0`, and uses Angular/CLI `22.2.x`, TypeScript `6.0.x`, RxJS `7.8.x`, Vitest `5.x` and jsdom `30.x`. It currently adds no component library, grid, state-management framework or runtime SDK. Angular 22 officially supports the installed Node 24 line. `package-lock.json` is the install authority and `npm ci` is the reproducible install command.

`MiniStore.Web` references `Microsoft.AspNetCore.OpenApi` `10.0.12` for the authenticated internal v1 contract. No Swagger UI, JWT library or cross-origin authentication dependency was introduced.

A NuGet/npm vulnerability scan was not executed in MOD-001; vulnerability status remains not verified and must be added to the CI/security gate before production.
