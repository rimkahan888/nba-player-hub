# ADR 0007: Store artwork in object storage

- **Status:** Accepted
- **Date:** 2026-10-30

## Context

Original player art is the app's visual identity. Images are large and should be served efficiently.

## Decision

Store image files in object storage and keep only URLs and metadata in the `Artwork` table. Storage sits behind `IArtStorage`.

## Consequences

- Small database, images can be served through a CDN.
- One more service to configure; the provider is still to be chosen.
