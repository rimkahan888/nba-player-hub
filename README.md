# NBA Player Hub

[![CI](https://github.com/rimkahan888/nba-player-hub/actions/workflows/ci.yml/badge.svg)](https://github.com/rimkahan888/nba-player-hub/actions/workflows/ci.yml)

A visual NBA player hub built around original player artwork, with each player's bio, stats and contract one tap away. Built with .NET 10, ASP.NET Core and Blazor.

> **Status:** early development. The solution skeleton, domain logic, CI, the Blazor Web App shell and the first database migration (teams and players) are in place; the first end-to-end slice (one player profile, deployed) is next.

<!-- Screenshot or GIF goes here once the gallery exists. -->

## Features (version 1)

- [ ] Player gallery of original artwork, filterable by team and position
- [ ] Player profiles: bio, height, age, draft details
- [ ] Current season stats
- [ ] Contract and salary details, with team payroll totals
- [ ] Admin-only art upload
- [ ] Responsive on phone, tablet and desktop

Planned modules after version 1: a salary cap trade machine and a digital trading card collection. See the [ideas backlog](docs/ideas-backlog.md).

## Tech stack

| Area | Choice |
| --- | --- |
| Runtime | .NET 10 (LTS), C# |
| Backend | ASP.NET Core minimal APIs |
| Frontend | Blazor Web App (auto render mode) |
| Data | PostgreSQL, EF Core |
| Player data | [balldontlie API](https://docs.balldontlie.io/), synced nightly; salaries seeded |
| Testing | xUnit, Shouldly, `WebApplicationFactory`, Testcontainers |
| CI/CD | GitHub Actions |
| Hosting | Fly.io or Railway (container) |

## Architecture

A single deployable app with a layered solution: Domain has no dependencies, Application holds use cases and interfaces, Infrastructure owns EF Core and external APIs, and Web is the host. External data is synced into the app's own database, so pages never wait on a third-party API.

Full details, including a diagram and the data model: [docs/technical-design.md](docs/technical-design.md). Key decisions: [docs/adr](docs/adr/).

```
src/
  NbaHub.Domain/          Entities, value objects and business rules
  NbaHub.Contracts/       DTOs shared by the API and the Blazor client
  NbaHub.Application/     Use cases and interfaces
  NbaHub.Infrastructure/  EF Core, balldontlie client, importers, storage
  NbaHub.Web/             Host: Blazor app, minimal API, background sync
  NbaHub.Web.Client/      Interactive WebAssembly components (auto render mode)
tests/
  NbaHub.Domain.Tests/
  NbaHub.Infrastructure.Tests/
  NbaHub.Web.IntegrationTests/
docs/                     Brief, technical design, ADRs, ideas backlog
```

## Getting started

Prerequisites: [.NET 10 SDK](https://dotnet.microsoft.com/download) and [Docker](https://www.docker.com/) for a local PostgreSQL.

```bash
git clone https://github.com/rimkahan888/nba-player-hub.git
cd nba-player-hub
dotnet build NbaHub.slnx
dotnet test NbaHub.slnx
```

The tests don't need a database. To run the app against one:

```bash
# 1. Start PostgreSQL (choose your own local password)
docker run -d --name nbahub-db -p 5432:5432 \
  -e POSTGRES_DB=nbahub -e POSTGRES_PASSWORD=<local-password> \
  -v nbahub-data:/var/lib/postgresql/data postgres:17-alpine

# 2. Store the connection string in user secrets (never in appsettings)
dotnet user-secrets set ConnectionStrings:NbaHub \
  "Host=localhost;Port=5432;Database=nbahub;Username=postgres;Password=<local-password>" \
  --project src/NbaHub.Web

# 3. Create the tables
dotnet tool restore
dotnet ef database update --project src/NbaHub.Infrastructure --startup-project src/NbaHub.Web

# 4. Run the app
dotnet run --project src/NbaHub.Web
```

The app runs at http://localhost:5080, with a health check at `/health`.

## What I learned

_To be written as the project grows._

## Artwork and trademarks

All player artwork is original and © the repository owner; it is not covered by the code licence. This project is not affiliated with or endorsed by the NBA or any team, and uses no official logos.

## Licence

Code is released under the [MIT licence](LICENSE).
