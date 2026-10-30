# ADR 0008: Host on Railway as a container

- **Status:** Accepted
- **Date:** 2026-10-30 (host chosen 2026-10-31)

## Context

The app needs a public URL at low cost, deployed from GitHub Actions. The candidates were Fly.io and Railway.

## Decision

Deploy to **Railway**, with a Railway PostgreSQL database in the same project.

- CI builds the image with `dotnet publish /t:PublishContainer` (no Dockerfile). On pushes to `main`, after the tests pass, it pushes the image to GitHub Container Registry as `ghcr.io/rimkahan888/nba-player-hub`, tagged `latest` and with the commit SHA.
- The Railway service runs the public `latest` image. CI then runs `railway redeploy` with a project token, so Railway pulls the new image.
- Railway's pre-deploy command runs the same image with `--migrate --seed` before traffic switches over. If it fails, the old version keeps running.
- The connection string is built from Railway's database reference variables. `ASPNETCORE_FORWARDEDHEADERS_ENABLED=true` makes the app trust Railway's HTTPS proxy.

## Consequences

- The app and database are set up in one dashboard, with private networking between them and little configuration.
- Migrations run inside the image through `--migrate`, not as a separate migrations bundle. This means one artifact and the same code path locally and in production. The trade-off is that the runtime image contains the migrations, and the app runs them with the app's database user.
- The image must be public, because pulling from a private registry needs Railway's Pro plan. That is fine: the source is public anyway, and no secrets are baked into the image.
- A small monthly cost, and less of an "enterprise" signal than Azure.
- Rollback: point the service at an older SHA tag in the Railway dashboard.
