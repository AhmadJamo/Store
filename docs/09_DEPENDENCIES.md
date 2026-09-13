# Dependencies
> Status: IMPLEMENTED  
> Source of truth: `.csproj` files  
> Last reviewed: 2026-09-13

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

No third-party JavaScript package manager files, external service SDKs, API clients, or test packages were found. A vulnerability package scan was not completed because the local NuGet configuration was inaccessible; vulnerability status is Not verified.
