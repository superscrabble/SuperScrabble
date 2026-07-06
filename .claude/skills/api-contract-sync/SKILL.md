---
name: api-contract-sync
description: Detect and (only on request) repair drift between the C# API/SignalR contracts and the hand-written Angular TypeScript models. Use when a DTO, view model, hub event payload, enum, or field seems out of sync between server and client. Report-only by default.
---

**Read `.claude/skills/_baseline.md` first and comply with it. Priority: P0.**

## Purpose

The API/SPA contract is entirely untyped (Angular calls use `responseType: 'text'` and
hand-maintained models; SignalR events are matched by string name). This skill audits one
endpoint, hub event, or model family — or does a full sweep — and reports every mismatch. It
fixes drift only when explicitly asked, and then only on **one side per task**.

## Non-goals

- Not for designing new endpoints or changing API semantics.
- Not for introducing an OpenAPI client generator (separate, human-led task).
- Never a "fix both sides at once" tool — one side per task, chosen by the user.

## Allowed read scope

- C# contracts: `src/Server/WebApi/SuperScrabble.WebApi.ViewModels/**`,
  `src/Server/WebApi/SuperScrabble.WebApi/Controllers/**`,
  `src/Server/WebApi/SuperScrabble.WebApi/HubClients/**` (`IGameClient`),
  `src/Server/WebApi/SuperScrabble.WebApi/Hubs/GameHub.cs` (read-only, method names/params).
- TS side: `src/ClientApp/super-scrabble-app/src/app/models/**`,
  `src/ClientApp/super-scrabble-app/src/app/services/**` (usage sites).

## Allowed write scope

- Default: **none** (report-only).
- If the task says "fix TypeScript side": `src/app/models/**` and the touched call sites in
  `src/app/services/**` / components — TS only.
- If the task says "fix C# side": `SuperScrabble.WebApi.ViewModels/**` only. Note this is a
  server contract change and per-file approval is required in the task.

## Forbidden actions

Baseline rules, plus:
- Never modify both C# and TypeScript in the same task.
- Never rename SignalR hub methods or client event names (`IGameClient` members) — string-matched
  on the client; renames break gameplay silently.
- Never change serialization settings, JSON options, or `Program.cs`.
- Never "fix" drift by loosening TS types to `any`.

## Required inputs

1. Scope: endpoint(s), hub event(s), or "full sweep".
2. Mode: `report-only` (default) or `fix` + which side (`csharp` | `typescript`).

## Workflow

1. Enumerate the requested contract surface: controller action signatures + view/input models on
   the C# side; `IGameClient` members and hub method parameters for SignalR.
2. Locate the TS counterpart models and every usage site (services, components parsing
   responses).
3. Compare field by field. Check: property name, casing (C# PascalCase vs JSON camelCase vs TS),
   type, nullability (`?`/NRT vs TS optional), enum members *and numeric values* (client receives
   ints, e.g. `GameMode`), collection shapes, date/time representation, and any field present on
   one side only.
4. Produce the drift table. In report-only mode, stop here.
5. In fix mode: state a plan, edit only the approved side, keeping runtime wire format identical
   unless the task says the wire format itself is wrong.
6. Validate and report. Do not commit.

## Required validation commands

Report-only mode: none required (state that no commands were run).
Fix mode — both builds must pass regardless of which side changed:

| Working dir | Command | Must show |
|---|---|---|
| repo root | `dotnet build src/Server/SuperScrabble.sln` | 0 errors |
| `src/ClientApp/super-scrabble-app` | `npm run build` | Success (budget warning is known/acceptable) |
| `src/Server` | `dotnet test -c Release` | 42+/42+ pass (only when C# changed) |

## Stop and escalation rules

Baseline rules, plus stop when:
- fixing drift requires changing the wire format (both sides by definition) — report the two
  options and let the user pick the side and sequence;
- a mismatch involves auth payloads (login/register token responses) — security review needed;
- an enum mismatch implies stored data would be reinterpreted (e.g. persisted `GameMode` ints).

## Invocation template

```
/api-contract-sync
Scope: <endpoint | hub event | model name | full sweep>
Mode: report-only | fix (side: csharp | typescript)
Protected-file approval granted this task: <none | list>
```

## Final report template

```
## api-contract-sync report
Scope audited: <…>   Mode: <report-only | fix:side>
Drift table:
| Contract | Field | C# (type/null) | TS (type/null) | Wire (actual JSON) | Verdict |
|---|---|---|---|---|---|
Fixes applied (if any): <files + summary, side: …>
Validation:
- dotnet build → <result / not run (report-only)>
- npm run build → <result / not run>
- dotnet test -c Release → <result / not run>
Unverified: <e.g. runtime wire shape not observed live; inferred from serializer defaults>
Status: REPORTED | FIXED | ESCALATED (<reason>)
```
