# ADR 0003: Blazor Web App with auto render mode

- **Status:** Accepted
- **Date:** 2026-10-30

## Context

The target role is full-stack .NET. Options were Blazor Server, Blazor WebAssembly, or a Blazor Web App using auto render mode.

## Decision

Use a Blazor Web App with auto render mode: pages are server-rendered first, then interactive components switch to WebAssembly.

## Consequences

- Fast first load and good SEO from server rendering; no constant server connection once WebAssembly has loaded.
- Components can run on the server or in the browser, so data access goes through the API (or an interface with two implementations), never directly to EF Core from a component.
