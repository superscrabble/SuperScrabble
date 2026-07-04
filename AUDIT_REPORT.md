# SuperScrabble — Technical Audit Report (branch: `new-modes`)

Date: 2026-07-03
Audit machine: Windows 11, .NET SDK 10.0.300 (runtimes 7.0.20 / 8.0.27 / 10.0.8 installed — **no .NET 6 runtime**), Node v24.11.1, npm 11.6.2.

This branch is the feature-rich evolution of the project (294 commits past `main`'s content, last commit 2022-03-24): game modes, parties/teams, invitation codes, and turn timers, restructured under `src/Server/`. A prior audit of `main` exists on branch `fable-work-on-main-branch`; several core defects found there are **still present here** and are marked _(inherited)_.

Claims are **[Confirmed]** (built/ran/read the code) or **[Assumption]**.

---

## 1. Repository map

```
src/Server/SuperScrabble.sln        — 15 projects, net6.0, C# 10 file-scoped namespaces, NRT enabled
├── Common/
│   ├── SuperScrabble.Common                — GlobalConstants (WildcardValue, IsProduction…)
│   ├── SuperScrabble.Common.Attributes / .Exceptions / .Extensions / .Resources
├── Data/
│   ├── SuperScrabble.Data                  — AppDbContext (Identity + EF Core 6), DatabaseConfig,
│   │                                          EFRepository, Migrations, Seeding (Words)
│   ├── SuperScrabble.Data.Common, SuperScrabble.Data.Models (AppUser, AppRole, Word, Game, UserGame)
├── Services/
│   ├── SuperScrabble.Services.Common       — ShuffleService, JsonWebTokenGenerator,
│   │                                          InMemoryEncryptionKeyProvider, InvitationCodeGenerator
│   ├── SuperScrabble.Services.Data         — UsersService, GamesService, WordsService
│   ├── SuperScrabble.Services.Game         — GameService, GameValidator, ScoringService,
│   │                                          MatchmakingService, GameStateFactory
│   ├── SuperScrabble.Services.Game.Common  — Tile, Cell, enums (GameMode, TimerType…),
│   │                                          GameRoomConfiguration, tiles/bonus providers
│   └── SuperScrabble.Services.Game.Models  — GameState, Team, Player, Member, Parties
│                                             (FriendParty/DuoParty), Boards, Bags, WordBuilder
├── Tests/
│   ├── SuperScrabble.Services.Game.Tests   — xUnit: 13 tests (12 GameValidator, 1 Scoring)
│   └── SuperScrabble.Sandbox               — scratch
└── WebApi/
    ├── SuperScrabble.WebApi                — **backend entry point** (minimal-hosting Program.cs),
    │   ├── Hubs/GameHub.cs (585 lines)        Controllers (Games, Users), HubClients (IGameClient),
    │   ├── Timers/ (GameTimer, StandardTimer, ChessTimer, TimerManager)
    │   └── all/  ← word-list .txt files used by the seeder (~866k lines total)
    └── SuperScrabble.WebApi.ViewModels     — input/view models (typed hub contracts)

src/ClientApp/super-scrabble-app/           — Angular 13 SPA (+ @angular/fire + firebase 9!)
tools/WordScrapers/                          — separate scraper solution
(no CI config, no Dockerfile) [Confirmed]
```

- **Entry points:** backend `src/Server/WebApi/SuperScrabble.WebApi/Program.cs`; frontend Angular app under `src/ClientApp`.
- **DB access:** EF Core 6 code-first, migrations in `SuperScrabble.Data/Migrations`, `Database.Migrate()` + word seeding at startup. [Confirmed]
- **Real-time:** SignalR `GameHub` at `/gamehub` with **strongly-typed client interface (`Hub<IGameClient>`)** — a genuine improvement over `main`. JWT via `access_token` query string. [Confirmed]

## 2. Build and runtime status

| Step | Command | Result |
|---|---|---|
| Backend build | `dotnet build src/Server/SuperScrabble.sln` | **SUCCESS** — 0 errors; NETSDK1138 warnings (net6.0 EOL) [Confirmed] |
| Backend tests | `dotnet test --no-build` | **ABORTED** — needs .NET 6 runtime, not installed [Confirmed] |
| Backend run | — | **Blocked by code**: `DatabaseConfig.IsProduction = true` (mutable static, default true) points at placeholder `my-server\SQLEXPRESS`; nothing ever sets it to false, so a local run targets a nonexistent server. Also `Server=.\SQLEXPRESS` hardcoded for the non-production path (the user's machine uses `(localdb)\MSSQLLocalDB`). [Confirmed by code reading] |
| Frontend install | `npm install` | **FAILS** — same `@ng-bootstrap/ng-bootstrap@10` vs Angular 13 peer conflict as `main` _(inherited)_; works with `--legacy-peer-deps` [Confirmed] |
| Frontend build | `npx ng build` | **SUCCESS** on Node 24 (33s) [Confirmed] |

## 3. Architecture assessment

**Improvements over `main`:** typed hub client (`Hub<IGameClient>`), class-level `[Authorize]` on the hub, matchmaking extracted into `MatchmakingService` with `ConcurrentDictionary`, view models unified in one project, environment-based `serverUrl` in the Angular app, multi-mode game engine behind `GameStateFactory`/`GameRoomConfiguration`.

**Critical problems:**

1. **Timer threads mutate game state with no synchronization.** `ChessTimer.OnTimedEvent` fires every second on a threadpool thread and mutates `GameState` (`RemainingSecondsByUserNames`, `NextTeam()`, `EndGame()`, `FillPlayerTiles`) and matchmaking dictionaries **concurrently with hub invocations**. There is no lock anywhere. This is a data-corruption race in the game's core loop. [Confirmed]
2. **Scoped-service capture in timers.** Timers are created via `ActivatorUtilities.CreateInstance(_serviceProvider, …)` from a hub invocation's scoped provider and live for the whole game; the captured `IGamesService`/DbContext is disposed when the invocation ends. The authors hit this: save-on-timeout is **commented out with "UserManager is Disposed"** — so **games that end by timeout are never persisted**. [Confirmed]
3. **Matchmaking is only superficially thread-safe.** The dictionaries are concurrent, but the values are not: `List<WaitingTeam>` inside `waitingTeamsByGameModes` is mutated (`Add`/`Clear`/`Remove`/enumerate) without any lock — two concurrent `JoinRoom` calls can corrupt the queue or start duplicate games; `CreateParty` has a check-then-act race on invitation codes. Multi-step operations (`StartGameFromParty`, `AddToWaitingQueue`) are not atomic. [Confirmed]
4. **Unhandled exceptions instead of error responses.** `MatchmakingService.GetGameState(userName)` **throws `ArgumentException`** when the user is in no game, and `WriteWord`/`ExchangeTiles`/`SkipTurn`/`LoadGame` call it with no guard — any stray command from a client not in a game produces an unhandled hub exception. `SaveGameIfTheGameIsOverAsync` re-resolves state and dereferences `timer!` (NRE if absent). [Confirmed]
5. **Auth (inherited):** hardcoded JWT key `"MY_SUPER_SECRET_KEY_ABC"` in `InMemoryEncryptionKeyProvider`; `OnTokenValidated` never awaits `GetUserAsync` (check is a no-op); `RequireHttpsMetadata=false`; no issuer/audience validation. Plus a stray `AddAuthentication(...).AddCertificate()` registration that serves no visible purpose. [Confirmed]
6. **Config:** `appsettings.json` is empty of anything meaningful; connection string and production/dev switching live in code (`DatabaseConfig.IsProduction`). [Confirmed]
7. **Observability:** no `ILogger` usage in hub/services/timers; `Console.WriteLine` debug leftovers in the hub; timer `Elapsed` handlers are `async` lambdas whose exceptions are silently lost (or crash the process). [Confirmed]
8. **Firebase is present in the frontend** (`@angular/fire`, `firebase`, initialized in `app.module.ts`, config committed in `environment.ts`). Per the engagement constraint this must not grow; whether to remove it is a product decision (flagged, not acted on). [Confirmed]

## 4. Game-engine assessment

| Concern | Where | Verdict |
|---|---|---|
| Board / bonus cells | `Models/Boards`, `Common/BonusCellsProviders`, mode-specific boards | Reusable; modes select board via factory |
| Racks / bag | `Player`, `Models/Bags` (`DrawTile()` returns null when empty; `new Random()` per draw) | Reusable after hardening _(inherited)_ |
| Turn order | `GameState.NextTeam()` + `Team.NextPlayer()` (team-based now) | **Same infinite-loop defect as `main`**: `while (_teams.Count > 1)` never exits if every team has surrendered/run out of time [Confirmed] |
| Move validation | `GameValidator` | Well-decomposed, reusable, **but contains the inherited wildcard exploit**: `playerTilesCopy.Remove(submittedTile)` (GameValidator.cs:82) silently fails for wildcard plays → one blank can be played N times in a move [Confirmed] |
| Word extraction / scoring | `WordBuilder`, `ScoringService` (+ per-mode bonus via providers) | Reusable |
| Dictionary | `WordsService.IsWordValid` → 1 sync DB query per word per move _(inherited)_ | Replace with in-memory set |
| Blank tiles | `Player.RemoveTile` + validator | Exploit above; otherwise OK |
| Pass / swap / resign | `SkipTurn` / `ExchangeTiles` / `LeaveGame` (in hub) | Logic OK; `LeaveGame` logs history with **wrong status (`WriteWord`)** — minor bug [Confirmed] |
| Timers | `StandardTimer` (per-turn), `ChessTimer` (per-player budget) | Feature works in happy path; races + scope capture + unsaved timeout games (§3.1–3.2) |
| Game modes | `GameMode` enum (Duel/Duo/Classic/ChessScrabble/SuperScrabble/MadBoards), `GameStateFactory`, `GameRoomConfiguration` | The right extension seams; reusable |
| Parties | `FriendParty`/`DuoParty`, invitation codes | Reusable; concurrency gaps (§3.3) |
| Game history | `GameHistoryLog` on GameState | New, fine |
| Word seeding | `WordsSeeder`: CWD-relative `"./all"`, ~500k+ **individual `Any()` SQL queries**, single giant SaveChanges | **This is why first-run seeding appears broken/hung** — it either crashes (wrong CWD) or takes tens of minutes silently. Needs the batched rewrite. [Confirmed] |

## 5. Dependency assessment

| Package | Current | Recommendation |
|---|---|---|
| net6.0 (all projects) | EOL Nov 2024; runtime absent here | **Retarget net8.0 now** (runtime installed, LTS, trivial delta from net6). Risk: low |
| EF Core / Identity / JwtBearer 6.0.1 | EOL | 8.0.x with retarget. Risk: low |
| `Microsoft.AspNetCore.Identity` 2.2.0 + `Mvc.Abstractions` 2.2.0 (Services.Data) | Deprecated legacy packages _(inherited pattern)_ | Replace with `FrameworkReference Microsoft.AspNetCore.App`. Risk: low |
| `Microsoft.AspNetCore.Authentication.Certificate` 6.0.1 | Registered but apparently unused | Remove registration + package after confirming. Risk: low |
| Swashbuckle 6.2.3, Test.Sdk 16.11, xunit 2.4.1 | old | Bump with retarget. Risk: low |
| Angular 13 + ng-bootstrap 10 peer conflict | breaks `npm install` _(inherited)_ | `.npmrc` with `legacy-peer-deps=true` now; staged Angular upgrade later |
| `firebase` 9 + `@angular/fire` 7 + `firebase-tools` (a CLI, in dependencies!) | Adds ~heavy deps; constraint says no BaaS growth | Flag: decide whether to remove; at minimum move `firebase-tools` out of runtime deps. Risk of removal: medium (touch UI code) |

## 6. Recommended plan

**A. Critical to build & run (now):** A1 retarget net8 + package bumps; A2 `.npmrc`; A3 config externalization (connection string + JWT key from `appsettings`/env, delete `DatabaseConfig.IsProduction`, key provider reads config, fail-fast); A4 fix `OnTokenValidated` await.

**B. Correctness & security (now, each with tests):** B1 wildcard exploit fix (validator, one line + xunit regression tests); B2 hub guards — catch/convert "not in game" into a client error instead of unhandled exception; B3 `NextTeam()` termination guard; B4 thread safety — per-game locks shared by hub *and* timers, lock around waiting-team lists and party creation, `TimerManager` on `ConcurrentDictionary`; B5 timer scope fix — timers resolve `IGamesService` from a fresh scope (`IServiceScopeFactory`) so timeout games persist; B6 seeder rewrite (batched, content-root-relative path, logging).

**C. Reliability (next):** in-memory word set; `ILogger` everywhere + remove `Console.WriteLine`; global hub exception handling; SignalR `withAutomaticReconnect` on the client; issuer/audience validation; fix `LeaveGame` history-log status; decide Firebase removal.

**D. Tests:** port the validator/state-manager/turn-order regression tests from the `main` engagement to xUnit here; add matchmaking concurrency tests; later `WebApplicationFactory` + SignalR integration test of one full game per mode.

## 7. Recommended target architecture

Unchanged in shape from the `main` audit — ASP.NET Core + SignalR + EF Core/SQL Server + Angular, with the hub as transport only. This branch already has the better bones (typed clients, matchmaking service, factories). The work is: make the concurrency model real (one lock per game honored by hub + timers), give timers proper DI scopes, externalize config, and finish the inherited correctness fixes. In-memory state behind `IMatchmakingService`/`TimerManager` statics remains acceptable for single-instance deployment; both are the seams for a distributed store if ever needed.
