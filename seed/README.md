# Seed data

Data the free balldontlie tier doesn't provide, maintained by hand (see [ADR 0006](../docs/adr/0006-seed-salaries-and-missing-data.md)).

| File | Contents |
| --- | --- |
| `teams.json` | Teams that seeded players belong to. Matched on `abbreviation`. |
| `players.json` | Player bios, including birth dates. Matched on the slug built from the name, so editing a name creates a new player. |

- `asOf` is the date the file was last checked.
- `team` in `players.json` must be an abbreviation listed in `teams.json`.
- `height` uses the source's feet-inches format, for example `"6-6"`.
- `externalId` is the balldontlie ID, used later by the nightly sync. Leave it `null` if unknown.

Load the files into the database with:

```bash
dotnet run --project src/NbaHub.Web -- --seed
```

Running it again only updates what changed.
