# ADR 0006: Seed salaries, birth dates and stats from files

- **Status:** Accepted
- **Date:** 2026-10-30

## Context

The free tier has no birth dates, stats or contracts. Contracts and stats need the $9.99 or $39.99 tiers. Scraping salary sites would break their terms of use.

## Decision

Keep salaries, contracts, birth dates and a stats snapshot in seed files under `seed/`, imported by the sync job.

## Consequences

- No scraping, no monthly API cost for version 1.
- Manual upkeep; every salary and stats section shows a "data as of" date.
