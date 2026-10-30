# CLAUDE.md

Guidance for Claude Code when working in this repository.

## What this project is

NBA Player Hub: a visual NBA app built around original player artwork made by the repo owner. Version 1 shows a player gallery and player profiles (bio, stats, contract and salary). It is the owner's main portfolio piece for a C# / .NET job search, so code quality, tests and clear docs matter as much as features.

Read these before larger changes:

- `docs/project-brief.md`: MVP scope, out of scope, definition of done
- `docs/technical-design.md`: architecture, data model, data sources, API endpoints
- `docs/adr/`: decisions already made and why
- `docs/ideas-backlog.md`: parked ideas

## Scope guard

Version 1 is the player gallery and profile hub only. If a request drifts beyond the MVP (trade machine, trading cards, live scores, fantasy, analytics, accounts for the public), say so, suggest adding it to `docs/ideas-backlog.md` as Parked, and keep going with MVP work unless the owner insists.

## Stack

- .NET 10 (LTS), C# latest, nullable enabled, warnings as errors
- ASP.NET Core minimal APIs and a Blazor Web App (auto render mode)
- EF Core with PostgreSQL (Npgsql)
- xUnit + Shouldly for tests; `WebApplicationFactory` for integration tests; Testcontainers for PostgreSQL once the database exists. Do not add FluentAssertions (v8+ needs a paid commercial licence).
- Package versions are managed centrally in `Directory.Packages.props`. Add `PackageVersion` there and a version-less `PackageReference` in the project.

## Architecture rules

Dependencies point inward. Never add a reference that breaks this:

```
Domain        <- no project references, no NuGet packages unless trivial
Contracts     <- no project references (DTO records shared with the Blazor client)
Application   -> Domain, Contracts
Infrastructure-> Application
Web           -> Application, Infrastructure (composition root)
Web.Client    -> Contracts only
```

- Business rules (age, height, payroll, contract years) live in Domain and are unit tested.
- EF Core, HttpClient, file and storage access live only in Infrastructure, behind interfaces declared in Application.
- Page requests never call balldontlie. Only the sync job does (see ADR 0005). Respect the free tier limit of 5 requests per minute.
- Interactive Blazor components get data through the API or an Application interface, never a `DbContext` directly.
- Secrets (API keys, connection strings, admin password) come from configuration: `dotnet user-secrets` locally, host secrets in production. Never commit them.
- No official NBA or team logos or wordmarks anywhere. Team names and colours are fine.

## C# conventions

- File-scoped namespaces matching the folder path.
- `sealed record` for DTOs and immutable data; `readonly record struct` for small value objects.
- Primary constructors for DI in services and endpoint classes.
- Minimal APIs: group routes with `MapGroup`, return `TypedResults`, keep handlers thin and call Application.
- Every async method takes a `CancellationToken` and passes it on.
- Use `DateOnly` for dates without time, `decimal` for money.
- Prefer small, focused classes over large services. No static mutable state.

## Testing rules

- New domain logic gets unit tests in the same change.
- Test names describe behaviour: `Method_or_subject_does_something_when_condition`.
- External HTTP calls are tested with a fake `HttpMessageHandler` and saved JSON fixtures, never the live API.
- Run `dotnet test` before saying a change is done.

## Commands

```bash
dotnet restore
dotnet build NbaHub.slnx
dotnet test NbaHub.slnx
dotnet run --project src/NbaHub.Web        # http://localhost:5080, health at /health
dotnet format NbaHub.slnx                  # fix formatting
dotnet tool restore                        # installs dotnet-ef from dotnet-tools.json
dotnet ef migrations add <Name> --project src/NbaHub.Infrastructure --startup-project src/NbaHub.Web --output-dir Persistence/Migrations
dotnet ef database update --project src/NbaHub.Infrastructure --startup-project src/NbaHub.Web
dotnet run --project src/NbaHub.Web -- --seed   # load seed/*.json into the database
dotnet run --project src/NbaHub.Web -- --migrate --seed   # what Railway's pre-deploy command runs
```

## Working style

- Work in small, finishable steps. Prefer a thin working slice end to end over a wide unfinished layer.
- Explain trade-offs briefly when making a design choice; record significant ones as a new ADR using `docs/adr/template.md`.
- Use Conventional Commits (`feat:`, `fix:`, `test:`, `docs:`, `chore:`, `ci:`).
- Keep `README.md` and `docs/technical-design.md` in step with what is actually built.

## Current status and next steps

Done:

- Solution skeleton with Domain, Contracts, Application, Infrastructure and Web projects
- `Height` and `PlayerAge` domain logic with unit tests
- `/health` endpoint with an integration test
- GitHub Actions CI building, checking formatting (`dotnet format --verify-no-changes`) and testing on every push
- Blazor Web App (auto render mode) in `NbaHub.Web` with `NbaHub.Web.Client`; template Counter page kept as an interactivity smoke test until real pages exist
- EF Core + Npgsql with `Team` and `Player` entities, `NbaHubDbContext` and the `InitialCreate` migration; a model test fails if a migration is missing
- `seed/teams.json` and `seed/players.json` (Luka Dončić, Lakers) with `ISeedImporter`, run by `--seed`; importer tests use Testcontainers PostgreSQL via `PostgresFixture` in `NbaHub.Infrastructure.Tests`
- `GET /api/players/{slug}` and the server-rendered `/players/{slug}` profile page, both using `PlayerProfileQuery`; endpoint and page tests run the whole app against seeded Testcontainers PostgreSQL (`SeededAppFactory`)

Next (thin slice: one player, end to end, deployed):

1. Deploy to Railway (ADR 0008). The container build (`PublishContainer`), the `--migrate` command and the CI `publish-image` and `deploy` jobs are written. What's left is the one-time Railway setup, the `RAILWAY_TOKEN` secret, making the GHCR package public, and the first live deploy. Then add the live URL to the README.
