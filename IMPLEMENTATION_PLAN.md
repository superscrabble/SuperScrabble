# SuperScrabble — Implementation Plan (branch: `new-modes`)

Derived from `AUDIT_REPORT.md` (2026-07-03, new-modes). Each step is independently buildable and testable. Statuses updated during execution.

## Milestone 1 — Build & run on a supported platform

- **Step 1 — Retarget net6.0 → net8.0 + upgrade EOL packages** _[low-med]_ — STATUS: ✅ done
  All csproj → net8.0; EFCore/Identity/JwtBearer → 8.0.x; Swashbuckle → 6.6.x; Test.Sdk → 17.x; xunit runner current; replace deprecated `Microsoft.AspNetCore.Identity 2.2.0` / `Mvc.Abstractions 2.2.0` with `FrameworkReference`. Verify: build + tests run.
- **Step 2 — Frontend installability** _[low]_ — STATUS: ✅ done
  `.npmrc` with `legacy-peer-deps=true`. Verify: clean `npm install` + `ng build`.
- **Step 3 — Externalize configuration** _[low]_ — STATUS: ✅ done
  `ConnectionStrings:DefaultConnection` (with `TrustServerCertificate=True` — SqlClient in EF 7+ encrypts by default) + `Jwt:SigningKey` in appsettings (dev key in Development file; fail-fast validation ≥32 chars). Delete `DatabaseConfig.IsProduction` switch; `InMemoryEncryptionKeyProvider` → configuration-backed provider. Migration impact: deployments must supply both values.

## Milestone 2 — Correctness & security (with tests)

- **Step 4 — `OnTokenValidated` missing await** _[low]_ — STATUS: ✅ done
- **Step 5 — Wildcard duplicate-consumption exploit** _[low]_ — STATUS: ✅ done
  `GameValidator` removes matched rack tile (`actualTile`), not submitted tile; xunit regression tests (single wildcard OK, double-play throws, duplicate regular tile throws).
- **Step 6 — Hub error handling** _[low]_ — STATUS: ✅ done
  Guard "user not in a game" in `WriteWord`/`ExchangeTiles`/`SkipTurn`/`LeaveGame`/`LoadGame` → typed client error, not unhandled `ArgumentException`; null-guard timer in `SaveGameIfTheGameIsOverAsync`; fix `LeaveGame` history-log status; remove `Console.WriteLine`s.
- **Step 7 — `NextTeam()` termination guard** _[low]_ — STATUS: ✅ done
- **Step 8 — Thread safety: one lock per game, honored by hub AND timers** _[med]_ — STATUS: ✅ done
  Static per-game `SemaphoreSlim` registry (keyed by gameId, removed on game end); hub game commands and `GameTimer.OnTimedEvent` bodies both acquire it. Matchmaking: single matchmaking lock around waiting-queue/party multi-step operations; `TimerManager` on `ConcurrentDictionary`.
- **Step 9 — Timer DI-scope fix** _[med]_ — STATUS: ✅ done
  Timers take `IServiceScopeFactory`, create a scope per tick-side-effect; restore the commented-out save-on-timeout (`SaveGameAsync`) so timeout-ended games persist.
- **Step 10 — Words seeder rewrite** _[low]_ — STATUS: ✅ done
  Batched inserts (10k/AddRange/Clear), case-insensitive dedupe, path = `ContentRootPath/all` with config override `Seeding:WordsDirectory`, ILogger progress, skip when table non-empty (existing guard stays in Program.cs).

## Milestone 3 — Reliability (next)

- Step 11 — In-memory word set behind `IWordsService` (load at startup). _[low]_
- Step 12 — `ILogger` in hub/timers/matchmaking; async-timer exception handling. _[low]_
- Step 13 — Client `withAutomaticReconnect` + rejoin flow. _[med]_
- Step 14 — JWT issuer/audience validation; remove unused certificate-auth registration. _[low]_
- Step 15 — Port `main`-engagement test suite to xUnit here; matchmaking concurrency tests; full-game integration test per mode. _[med]_
- Step 16 — Product decision: remove Firebase from the SPA (or keep deliberately); at minimum move `firebase-tools` to devDependencies. _[med]_

## Change log (updated during execution)

2026-07-04 — Milestones 1 & 2 (Steps 1–10) completed:

1. **Step 1** — All 15 projects retargeted net6.0 → net8.0; EFCore/Identity/JwtBearer → 8.0.11, Swashbuckle → 6.6.2, Test.Sdk → 17.11.1, xunit → 2.9.2, SpaServices → 8.0.11. Deprecated `Microsoft.AspNetCore.Identity 2.2.0` and `Mvc.Abstractions 2.2.0` replaced with `FrameworkReference Microsoft.AspNetCore.App`. Build: 0 errors (pre-existing NRT warnings remain, listed for later cleanup).
2. **Step 2** — `.npmrc` (`legacy-peer-deps=true`); `npm install` + `ng build` verified on Node 24.
3. **Step 3** — `ConnectionStrings:DefaultConnection` + `Jwt:SigningKey` (+ `Seeding:WordsDirectory`) in appsettings; `DatabaseConfig.IsProduction` switch (defaulting to a placeholder production server!) deleted; `InMemoryEncryptionKeyProvider` ("MY_SUPER_SECRET_KEY_ABC") deleted, replaced by `ConfigurationEncryptionKeyProvider` with fail-fast ≥32-char validation; stray certificate-auth registration removed.
4. **Step 4** — `OnTokenValidated` awaits `GetUserAsync`; deleted-user check now real.
5. **Step 5** — Wildcard duplicate-consumption exploit fixed in `GameValidator` (remove `actualTile`); 3 xunit regression tests in `SubmittedTilesOwnershipTests.cs`.
6. **Step 6** — Hub: game commands guarded via `ExecuteGameActionAsync` (client gets `Error("UserNotInsideAGame")` instead of an unhandled `ArgumentException`); `LoadGame` missing `return` after `NoSuchGame` fixed; `LeaveGame` history log status corrected to `Leave`; timer null-guard in save path; `Console.WriteLine`s removed.
7. **Step 7** — `NextTeam()` returns when no team can play; 3 tests in `GameStateTurnTests.cs`.
8. **Step 8** — New `GameLocks` (per-game `SemaphoreSlim`, removed on game end) acquired by hub game commands **and** both timers' tick handlers; static `MatchmakingLock` serializes JoinRoom/JoinRandomDuo/CreateParty/JoinParty/LeaveParty/StartGameFromParty/StopSearching; `TimerManager` moved to `ConcurrentDictionary` + `RemoveTimer` disposal; registered as singleton.
9. **Step 9** — Timers no longer capture scoped services: `IServiceScopeFactory` per tick; **save-on-timeout restored** (the commented-out `SaveGameAsync` in `ChessTimer` now runs in a fresh scope; same for `StandardTimer`); timer tick exceptions caught and logged instead of vanishing.
10. **Step 10** — `WordsSeeder` rewritten: batched 10k inserts with change-tracker clearing, case-insensitive dedupe, ILogger progress, path from `ContentRootPath/all` (config-overridable) instead of CWD-relative `./all`.

**Verified:** backend build clean; 42/42 tests pass (3 consecutive runs); `ng build` succeeds; **end-to-end boot on LocalDB**: database created, migrations applied, **865,809 words seeded** by the new seeder, second boot correctly skips seeding; smoke-tested live API — swagger 200, register 200 (JWT issued with config key), login 200, missing-user lookup 404.

**Note:** backend tests on this machine must run with `-c Release` — Windows Smart App Control intermittently blocks freshly built DLLs in the Debug output folder (error 0x800711C7). This is machine policy, not a code issue.
