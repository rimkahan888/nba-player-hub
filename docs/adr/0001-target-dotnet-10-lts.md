# ADR 0001: Target .NET 10 LTS

- **Status:** Accepted
- **Date:** 2026-10-30

## Context

The project starts in late October 2026. .NET 9 (STS) reaches end of support on 10 November 2026. .NET 10 is the current LTS release, supported until 14 November 2028. .NET 11 (STS) is due in November 2026.

## Decision

Target `net10.0` for every project, pinned through `global.json` with `rollForward: latestFeature`.

## Consequences

- Three years of security patches with no forced upgrade during the job search.
- New .NET 11 features are not available at first; upgrading later is a small, contained change.
