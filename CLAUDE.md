# CLAUDE.md — SuperScrabble

Real-time multiplayer Scrabble (Bulgarian dictionary, ~866k words). Angular 13 SPA + ASP.NET
Core (.NET 8) with SignalR gameplay, EF Core 8 + SQL Server (LocalDB in dev).

**Every agent task in this repo is governed by `.claude/skills/_baseline.md`.** Read it before
making changes — it defines secret handling, protected areas, forbidden commands, and escalation
rules that apply even when no specific skill is invoked.

## Project map

```
src/Server/SuperScrabble.sln        ← the ONLY valid solution (17 projects, net8.0)
├── WebApi/SuperScrabble.WebApi     ← entry point: Program.cs, Controllers/ (Users, Games),
│                                      Hubs/GameHub.cs (SignalR, /gamehub), Timers/, GameLocks.cs,
│                                      all/ (word-list seed data), appsettings*.json
├── WebApi/SuperScrabble.WebApi.ViewModels  ← API + hub contracts (C# side)
├── Services/  Services.Game (engine: validator, scoring, matchmaking, factories),
│              Services.Game.Models (GameState, boards, bags, parties),
│              Services.Data (Users/Games/Words services), Services.Common (JWT, shuffle)
├── Data/      SuperScrabble.Data (AppDbContext, Migrations/ — immutable, Seeding/)
├── Common/    constants, exceptions, extensions, resources
└── Tests/SuperScrabble.Services.Game.Tests  ← xUnit, the only test project (42 passing cases)

src/ClientApp/super-scrabble-app    ← Angular 13 CLI app
└── src/app/  pages/ (home, login, register, game, party, summary), common/ (gameboard, rack…),
              dialogs/, models/ (hand-written TS mirrors of C# view models),
              services/ (signalr, web-requests, matchmaking, game, language)

tools/WordScrapers                  ← separate scraper solution; not part of the app build
resources/, Docs/, Diagrams/        ← data & presentation assets; do not modify
```

## Verified commands (verified 2026-07-06; working directory matters)

| Task | Working directory | Command | Notes |
|---|---|---|---|
| Backend build | repo root | `dotnet build src/Server/SuperScrabble.sln` | 0 errors, 9 known warnings |
| Backend tests | `src/Server` | `dotnet test -c Release` | 42/42 pass. **Release is mandatory** — Smart App Control on this machine blocks Debug DLLs (`0x800711C7`) |
| Frontend build | `src/ClientApp/super-scrabble-app` | `npm run build` | Succeeds with known budget warning (initial bundle 1.10 MB > 500 kB) — flag only growth |
| Frontend tests | `src/ClientApp/super-scrabble-app` | `npm test -- --watch=false --browsers=ChromeHeadless` | Runs (verified 2026-07-06): baseline 7/33 pass, 26 pre-existing scaffold-spec failures. **Not a gate** — flag only new failures beyond this baseline |
| Run backend | `src/Server/WebApi/SuperScrabble.WebApi` | `dotnet run` | Needs SQL LocalDB; first boot seeds ~866k words. See `HOW_TO_RUN.md` |
| Run frontend | `src/ClientApp/super-scrabble-app` | `npm start` | Dev server on :4200 → API on :7168 |

## Protected areas — explicit task-level approval required before editing

Full list and rules in `.claude/skills/_baseline.md` §4. Summary: `Program.cs`, all
authentication/authorization code, `GameHub.cs`, `Timers/` + `GameLocks.cs`,
`MatchmakingService.cs`, `Data/Migrations/` (immutable), package manifests + lock files,
`appsettings*.json` / `environment*.ts` / `.npmrc` / `angular.json` / `.github/workflows/`.
Never run `dotnet ef database update`, destructive git/db commands, or deploys. Never print
secret values. Never weaken tests to pass.

## Skills index

Project skills live in `.claude/skills/<name>/SKILL.md`; all inherit `_baseline.md`.

| Skill | P | One-line purpose | Example invocation |
|---|---|---|---|
| `test-first-backend-fixer` | P0 | Fix one named backend bug: failing xUnit test first, smallest fix, full Release run | `/test-first-backend-fixer Bug: wildcard scores double in Duo mode; Expected: counted once` |
| `api-contract-sync` | P0 | Detect C#↔TypeScript DTO/hub-payload drift; report-only by default, fix one side on request | `/api-contract-sync Scope: WriteWord payloads; Mode: report-only` |
| `hub-concurrency-review` | P0 | Read-only race/lock/lifetime review of GameHub, Timers, GameLocks, MatchmakingService | `/hub-concurrency-review Target: current state of GameHub.cs; Focus: reconnect flow` |
| `angular-feature-work` | P1 | Scoped UI/UX change in the SPA with build (and where possible visual) verification | `/angular-feature-work Change: rack readable at 360px; Acceptance: no horizontal scroll` |
| `security-configuration-review` | P1 | Read-only authN/authZ/JWT/CORS/config audit; never prints secrets, never fixes | `/security-configuration-review Focus: endpoint authorization coverage` |
| `ef-query-migration-review` | P1 | Review EF queries (N+1, sync hot paths, transactions) and migration safety; review-only by default | `/ef-query-migration-review Target: GamesService.SaveGameAsync; Mode: review-only` |
| `dependency-upgrade-assessment` | P1 | Assess upgrades against our actual usage; read-only unless one package+version is approved | `/dependency-upgrade-assessment Target: angular upgrade path` |
| `repo-docs-maintainer` | P2 | Keep root Markdown true: every documented command re-run before being claimed to work | `/repo-docs-maintainer Target: HOW_TO_RUN.md` |

## Agent loops — DISABLED

No autonomous/recurring agent loops are approved in this repository. CI now exists
(`.github/workflows/ci.yml`, backend build+test and frontend build, first run green 2026-07-06),
but there is still no green frontend test gate (`npm test` runs with 26 pre-existing failures —
see table above), so
loop output cannot be fully independently verified. Until green test gates exist on both sides,
all work is single-task, human-reviewed. The audit (`AUDIT_REPORT.md` follow-up, §9) sketches three future
candidates — backend test-green loop, frontend build-and-style loop, documentation-verification
loop — as **proposals only, not approved to run**.
