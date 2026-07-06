---
name: hub-concurrency-review
description: Read-only concurrency review of GameHub, Timers, GameLocks, and MatchmakingService. Use before or after any change touching SignalR gameplay, locking, reconnect/disconnect, or timer code — it finds lock-coverage gaps, races, and disposal hazards but never edits files.
---

**Read `.claude/skills/_baseline.md` first and comply with it. Priority: P0. STRICTLY READ-ONLY.**

## Purpose

The gameplay core is shared mutable state (`GameState`, parties, waiting queues, timers) touched
by hub invocations *and* timer threads. The intended model: per-game `SemaphoreSlim` from
`GameLocks` (shared by hub commands and timer ticks — see `ExecuteGameActionAsync`,
`GameHub.cs`), plus a static `MatchmakingLock` for queue/party multi-step operations. This skill
verifies a diff (or the current code) against that model and reports every violation with a
concrete failing interleaving.

## Non-goals

- Never fixes anything — output is analysis only. Fixes go through a separate approved task.
- Not a general style/code review; only concurrency, lifetime, and async-safety.

## Allowed read scope

Everything under `src/Server/**`, focused on:
- `src/Server/WebApi/SuperScrabble.WebApi/Hubs/GameHub.cs`
- `src/Server/WebApi/SuperScrabble.WebApi/Timers/` (`StandardTimer`, `ChessTimer`, `TimerManager`)
- `src/Server/WebApi/SuperScrabble.WebApi/GameLocks.cs`
- `src/Server/Services/SuperScrabble.Services.Game/Matchmaking/MatchmakingService.cs`
- `src/Server/Services/SuperScrabble.Services.Game.Models/**` (`GameState`, `Player`, parties)

## Allowed write scope

**None.** No file edits, no new files in the repo. The report is chat output only.

## Forbidden actions

Baseline rules, plus: any `Edit`/`Write` to repository files; any command beyond read-only
inspection, `dotnet build`, and `dotnet test -c Release` (allowed only to anchor claims).

## Required inputs

1. Target: a diff, a branch/commit range, or "current state of <file(s)>".
2. Optional focus (e.g. "reconnect flow", "game-over path", "party configuration").

## Workflow

1. Enumerate every *mutation* of shared state in the target: `GameState` fields,
   `Player.ConnectionId`, `Member.ConnectionId`, party fields, `waitingTeamsByGameModes` lists,
   `TimerManager` entries, `GameLocks` registry.
2. For each mutation, determine which synchronization actually covers it at runtime
   (`GameLocks.Get(gameId)` held? `MatchmakingLock` held? none?). Note that `OnConnectedAsync` /
   `OnDisconnectedAsync` / `LoadGame` / `SetFriendPartyConfiguration` historically ran outside
   locks — check whether the target changes that.
3. For every uncovered or wrongly-covered mutation, construct a concrete two-actor interleaving
   (e.g. "timer tick at T1 … hub WriteWord at T2 …") that corrupts state, double-fires, NREs, or
   deadlocks.
4. Check the specific hazard classes: lock ordering (game lock vs matchmaking lock), await inside
   lock and reentrancy, check-then-act on `ConcurrentDictionary` values, `Clients.Client(null)`
   sends after disconnect nulls `ConnectionId`, timer `Dispose` vs in-flight tick, scoped-service
   lifetime captured past the invocation, `async void`/unobserved exceptions in timer handlers,
   and lock removal (`GameLocks.Remove`) racing a waiter.
5. Rank findings by severity and confidence; recommend for each a test shape and a remediation
   *direction* (not an implementation).

## Required validation commands

None required (analysis). If build/tests are run to anchor a claim, report their exact results.

## Stop and escalation rules

- If asked to also apply fixes: refuse within this skill and point to a follow-up task
  (`test-first-backend-fixer` cannot edit these files either — hub fixes need explicit
  task-level approval).
- If the review reveals an actively exploitable game-integrity issue, flag it at the top of the
  report as URGENT.

## Invocation template

```
/hub-concurrency-review
Target: <diff | branch | commit range | file(s)>
Focus (optional): <flow>
```

## Final report template

```
## hub-concurrency-review report (read-only — no files modified)
Target: <…>
Findings (most severe first):
### F1 — <title>
- Location: <file:line>
- Shared state: <what is mutated/read>
- Synchronization present: <lock held / none / wrong lock>
- Failing interleaving: <actor A step → actor B step → observable corruption>
- Risk: <data corruption | NRE | deadlock | lost update | duplicate action> — Severity: <H/M/L>
- Confidence: <confirmed-by-reading | plausible | speculative>
- Recommended test: <shape of the regression/concurrency test>
- Remediation direction: <one sentence — no code>
Commands run: <none | build/test + exact results>
Status: REPORTED
```
