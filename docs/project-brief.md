# NBA Player Hub – Project Brief

_Last updated: 30 Oct 2026_

Version 1 is a deployed web app showing original player art with each player's bio, stats and salary, built in modern C# as a portfolio piece.

## Why

- **The problem:** most NBA stats sites are dense tables with stock photos. None put original artwork at the centre.
- **What's different:** every player is shown through my own art, with bio, stats and salary one tap away.
- **The portfolio angle:** a polished, deployed app that shows modern .NET skills and gives a memorable story to tell in interviews.

## Audience

1. **Hiring managers and interviewers:** they spend a few minutes on the live link, the README and the code structure.
2. **NBA fans:** they browse the gallery and look players up.
3. **Me:** a project I enjoy enough to keep improving after the job search.

## MVP scope (version 1)

Eight features, built in this order. Each one should be deployable on its own.

| # | Feature | What it includes |
| --- | --- | --- |
| 1 | Player gallery | Grid of player art cards with name, team and position |
| 2 | Filters and search | Filter by team and position; search by name |
| 3 | Player profile page | Large art, bio (age, height, weight, draft, college or country) |
| 4 | Stats section | Current season averages |
| 5 | Contract and salary | Current salary, contract length and total value |
| 6 | Team pages | Team roster with art and total team payroll |
| 7 | Art management | Admin-only upload and linking of art to players |
| 8 | Responsive design | Works well on phone, tablet and desktop |

Start with a thin slice: one player, one profile page, real data, deployed.

## Out of scope (not in version 1)

These are parked in the [ideas backlog](ideas-backlog.md). If a new idea comes up mid-build, it goes there too.

- Trade machine and salary cap rules checking (planned module)
- Trading cards, packs and collections (planned module)
- Public user accounts and logins (admin only for now)
- Live scores and play-by-play
- Fantasy rosters, predictions and analytics boards
- Native mobile app (the web app is responsive instead)
- Historical and retired players

## Definition of done

- [ ] All 8 MVP features work on the live site
- [ ] Deployed with a public link
- [ ] Art shown for at least 30 players
- [ ] Unit tests cover the core logic; CI runs them on every push
- [ ] README has a screenshot or GIF, a live link, the tech stack and a "what I learned" section
- [ ] Pages load in under 2 seconds on a normal connection
- [ ] No official NBA or team logos anywhere in the app
- [ ] Short ADRs in `/docs/adr` for the main decisions

## Tech direction

| Area | Choice | Why |
| --- | --- | --- |
| Runtime | .NET 10 (LTS) | Supported until Nov 2028 |
| Backend | ASP.NET Core minimal APIs | Modern, lightweight, common in job ads |
| Frontend | Blazor Web App (auto render mode) | Full-stack C#, one language end to end |
| Database | PostgreSQL with EF Core | Free hosting options, widely used |
| Player data | balldontlie API, synced into my own database | Free tier, avoids rate limits |
| Salary data | Seeded dataset I maintain | Avoids scraping sites against their terms |
| Art storage | Object storage | Images don't belong in the database |
| Hosting | Fly.io or Railway (container) | Simple deploys, low cost, public link |
| CI/CD | GitHub Actions | Build and test on every push |

**Constraints:**

- Original art only. No official NBA or team logos or wordmarks; team names and colours are fine.
- Built solo in evenings and weekends, so scope stays small.
- Hosting cost should stay close to zero.

## Decisions so far (30 Oct 2026)

- **Target role:** full-stack .NET, with balanced effort on the Blazor UI and the API.
- **Frontend:** Blazor Web App with auto render mode, calling the app's own API.
- **Player scope:** current players only.
- **Hosting:** Fly.io or Railway, deployed as a container.

## Still open

- [ ] Which players are in the first 30, and how fast can I produce art?
- [ ] Where does the salary data come from, and how often will I update it?
- [ ] Final app name.
