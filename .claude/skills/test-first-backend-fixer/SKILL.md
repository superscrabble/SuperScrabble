---
name: test-first-backend-fixer
description: Fix one named backend/gameplay bug in the .NET server only after a failing xUnit test reproduces it. Use for bugs in game logic, validation, scoring, turn order, services, or data services. Red test first, smallest fix second, full Release test run third.
---

**Read `.claude/skills/_baseline.md` first and comply with it. Priority: P0.**

## Purpose

Fix exactly one named backend or gameplay bug using a strict red → green workflow: reproduce the
bug as a failing xUnit test, then apply the smallest fix that makes the whole suite pass.

## Non-goals

- Not for features, refactors, performance work, or "fix everything you find".
- Not for bugs whose root cause lives in the SignalR hub, timers, or matchmaking — those are
  diagnosed here but fixed only with explicit approval (see Stop rules).
- Not for frontend bugs (use `angular-feature-work`) or data-access review (use
  `ef-query-migration-review`).

## Allowed read scope

All of `src/Server/**` plus root docs. Frontend may be read only to understand the symptom.

## Allowed write scope

- `src/Server/Services/**`
- `src/Server/Common/**`
- `src/Server/Tests/**` (new tests and test helpers)

## Forbidden actions

Baseline rules, plus:
- No edits to `GameHub.cs`, `Timers/`, `GameLocks.cs`, or `MatchmakingService.cs` — stop and
  report the diagnosis instead.
- No test deletion, skipping, or assertion weakening — ever.
- No public API signature changes on services unless the task explicitly allows it.
- No fix without a failing test that reproduces the bug first.

## Required inputs

1. One-sentence bug statement (what is wrong).
2. Where it was seen (hub method, service, or user-visible symptom).
3. Reproduction steps or a concrete failing input, if known.
4. Expected correct behavior.

If the reporter cannot supply 3, deriving a reproduction is part of the job — but if no failing
test can be written, stop (see below).

## Workflow

1. Read the implicated code and its callers; state a short plan (suspected root cause, test to
   write, files to touch, validation).
2. Write a failing xUnit test in `src/Server/Tests/SuperScrabble.Services.Game.Tests/` that
   encodes the *expected* behavior. Follow the style of `GameValidatorTests.cs` /
   `GameStateTurnTests.cs` (use `[Fact(Timeout = 5000)]` for anything that could loop).
3. Run the suite; confirm the new test fails for the expected reason and record the failure text.
4. Implement the smallest fix inside the write scope.
5. Run the full suite; all tests (old + new) must pass.
6. Produce the final report. Do not commit.

## Required validation commands

| Step | Working dir | Command | Must show |
|---|---|---|---|
| Red | `src/Server` | `dotnet test -c Release` | New test fails, everything else passes |
| Build | repo root | `dotnet build src/Server/SuperScrabble.sln` | 0 errors |
| Green | `src/Server` | `dotnet test -c Release` | 100% pass, count increased |

## Stop and escalation rules

Baseline rules, plus stop and report (no edits beyond the test) when:
- the root cause is in `GameHub.cs`, `Timers/`, `GameLocks.cs`, or `MatchmakingService.cs`;
- the fix would touch more than 3 non-test files;
- you cannot write a deterministic failing test (e.g. the bug is a concurrency race — hand off to
  `hub-concurrency-review`);
- fixing it changes behavior other callers rely on (product decision).

## Invocation template

```
/test-first-backend-fixer
Bug: <one sentence>
Seen in: <hub method / service / symptom>
Repro: <steps or failing input, or "unknown">
Expected: <correct behavior>
Protected-file approval granted this task: <none | list>
```

## Final report template

```
## test-first-backend-fixer report
Bug: <restated>
Root cause: <file:line — explanation>
Test added: <test file :: test name> (failed before fix with: <error excerpt>)
Fix: <files changed, diff summary>
Validation:
- dotnet build src/Server/SuperScrabble.sln → <result>
- dotnet test -c Release (before) → <N failed / M passed>
- dotnet test -c Release (after)  → <0 failed / M+k passed>
Uncertainty / follow-ups: <anything unverified or out of scope>
Status: DONE | ESCALATED (<reason>)
```
