---
name: ef-query-migration-review
description: Review EF Core data access for lazy-loading N+1s, synchronous DB calls on hot paths, transaction boundaries, and migration safety/reversibility. Review-only by default; edits query code only when explicitly authorized and behavior-preserving tests exist. Never applies or edits migrations.
---

**Read `.claude/skills/_baseline.md` first and comply with it. Priority: P1.**

## Purpose

Review data-access code and proposed migrations in a codebase where **lazy-loading proxies are
enabled globally** (`UseLazyLoadingProxies()`, `Program.cs`) — so any navigation-property access
in a loop is a probable N+1 — and where at least one hot path issues synchronous per-item SQL
(`WordsService.IsWordValid`, one query per word per move). Optionally, with explicit
authorization, rewrite a named query.

## Non-goals

- Never applies migrations to any database.
- Never edits existing migrations (immutable history in
  `src/Server/Data/SuperScrabble.Data/Migrations/`).
- No schema/model changes without an explicitly approved task naming them.
- Not a general refactoring tool.

## Allowed read scope

- `src/Server/Data/**` (`AppDbContext`, `EFRepository`, models, migrations, seeding)
- `src/Server/Services/SuperScrabble.Services.Data/**`
- Callers of data services (hub, controllers, timers) — read-only, to establish hot paths.

## Allowed write scope

- Default: **none** (review-only).
- With explicit authorization naming the query/service: query code in
  `src/Server/Services/SuperScrabble.Services.Data/**` and new tests in `src/Server/Tests/**`.
- Migration *generation* only when the task explicitly approves it (`dotnet ef migrations add`),
  and the generated migration is presented for review — never applied.

## Forbidden actions

Baseline rules, plus:
- `dotnet ef database update`, `dotnet ef migrations remove`, any command executing SQL against a
  database.
- Editing files under `Migrations/` (including the model snapshot) by hand.
- Changing entity classes, `AppDbContext` model configuration, or relationships without an
  approved schema task.
- Disabling lazy-loading proxies globally (that is an architecture decision).

## Required inputs

1. Target: a diff, a service/method name, or "sweep of data-access hot paths".
2. Mode: `review-only` (default) | `fix-query` (named query, explicit) | `generate-migration`
   (explicit, with the approved model change already specified by the user).

## Workflow

1. Map the target's query call sites and classify each: hub-invoked hot path (per-move), startup
   (seeding/migration), or occasional (login, game save).
2. For each query, assess: lazy-load triggers inside loops (N+1), sync execution
   (`.ToList()`/`FirstOrDefault` without async) on request/hub paths, missing `AsNoTracking` on
   read-only reads, client-side evaluation, unbounded result sets, and transaction boundaries
   (multi-`SaveChanges` operations like game save — is partial failure consistent?).
3. For migrations under review: forward correctness, reversibility (`Down()`), data preservation
   (renames vs drop+create), lock/duration risk on large tables (`Words` has ~866k rows), and
   whether startup auto-migrate (`Database.Migrate()` in `Program.cs`) makes deployment ordering
   risky.
4. Review-only: deliver the report and stop.
5. `fix-query` mode: state a plan; write a behavior-parity test **first** (same results as the
   old query on representative data), then rewrite; keep the public service signature unless the
   task approves changing it.
6. Report. Do not commit.

## Required validation commands

Review-only: none required.
`fix-query` mode:

| Working dir | Command | Must show |
|---|---|---|
| repo root | `dotnet build src/Server/SuperScrabble.sln` | 0 errors |
| `src/Server` | `dotnet test -c Release` | 100% pass incl. new parity tests |

## Stop and escalation rules

Baseline rules, plus stop when:
- any fix would change the database schema or an entity relationship;
- correctness of a rewrite cannot be pinned by a test (no representative fixture available);
- a transaction-boundary fix would change user-visible failure behavior (product decision);
- the task drifts from the named query into surrounding services.

## Invocation template

```
/ef-query-migration-review
Target: <diff | service.method | sweep>
Mode: review-only | fix-query (<named query>) | generate-migration (<approved model change>)
Protected-file approval granted this task: <none | list>
```

## Final report template

```
## ef-query-migration-review report
Target: <…>   Mode: <…>
Query findings:
| Call site (file:line) | Path class (hot/startup/occasional) | Concern | Generated-SQL risk | Fix direction |
Migration findings (if applicable): <forward/rollback/data-preservation/lock-risk notes>
Changes made (fix-query only): <files, parity test name>
Validation: <commands + exact results | none (review-only)>
Explicitly NOT done: <no migrations applied; schema untouched>
Status: REPORTED | FIXED | ESCALATED (<reason>)
```
