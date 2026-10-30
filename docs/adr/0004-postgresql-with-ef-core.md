# ADR 0004: PostgreSQL with EF Core

- **Status:** Accepted
- **Date:** 2026-10-30

## Context

The app needs a relational database with cheap or free hosting options.

## Decision

Use PostgreSQL with EF Core and the Npgsql provider. Migrations live in `NbaHub.Infrastructure`.

## Consequences

- Free and widely hosted; strong EF Core support.
- Many enterprise .NET teams use SQL Server; keeping database access behind EF Core keeps a switch cheap.
