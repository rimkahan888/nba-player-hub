# ADR 0002: One deployable: API hosted in the Blazor server project

- **Status:** Accepted
- **Date:** 2026-10-30

## Context

Version 1 needs a Blazor front end and a JSON API. They could be two separate services or one process.

## Decision

Host the minimal API endpoints inside `NbaHub.Web`, the same process that serves the Blazor app. Keep the code layered (Domain, Application, Infrastructure) so the API can be split out later.

## Consequences

- One container, one URL, one set of secrets, lowest hosting cost.
- The API and UI cannot be scaled separately, which this app does not need yet.
