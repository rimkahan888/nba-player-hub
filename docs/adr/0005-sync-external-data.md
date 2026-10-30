# ADR 0005: Sync external data into our own database

- **Status:** Accepted
- **Date:** 2026-10-30

## Context

The balldontlie free tier allows 5 requests per minute. Calling it during page requests would be slow and would break when the API is down or rate limited.

## Decision

A background job syncs balldontlie data into PostgreSQL nightly. Page requests only read from our database. The client sits behind `IPlayerDataProvider`.

## Consequences

- Fast, predictable pages that survive API outages.
- Data can be up to a day old, which is fine for bio data.
- Changing provider or tier only touches Infrastructure.
