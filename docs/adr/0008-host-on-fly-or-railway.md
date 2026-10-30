# ADR 0008: Host on Fly.io or Railway as a container

- **Status:** Accepted
- **Date:** 2026-10-30

## Context

The app needs a public URL at low cost, deployed from GitHub Actions.

## Decision

Publish a container image with `dotnet publish /t:PublishContainer` and deploy it to Fly.io or Railway. The final choice is made before the first deploy.

## Consequences

- Simple container deploys and a public link.
- Less of an "enterprise" signal than Azure, and a small monthly cost.
