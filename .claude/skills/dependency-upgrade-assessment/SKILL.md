---
name: dependency-upgrade-assessment
description: Assess (not perform) NuGet and npm dependency upgrades — actual usage in this repo, breaking changes, migration steps, and a validation plan. Read-only unless the user explicitly approves a specific package and target version. Covers the staged Angular 13→17 path.
---

**Read `.claude/skills/_baseline.md` first and comply with it. Priority: P1. READ-ONLY BY DEFAULT.**

## Purpose

Turn "should we upgrade X?" into an evidence-based go/no-go: find where this repo actually uses
the package, map those call sites against the upgrade's breaking changes, and produce a
validation plan. Known debt from the audit: Angular 13 (EOL) + TypeScript 4.4 + peer conflict
masked by `.npmrc` `legacy-peer-deps=true`; `firebase-tools` (a CLI) in runtime `dependencies`;
`@ng-bootstrap/ng-bootstrap@10` peer mismatch.

## Non-goals

- Not an auto-upgrader. No manifest or lock-file edit happens without the user naming the
  package **and** target version in this task.
- Not a CVE feed; if advisories are checked (e.g. `npm audit`), results are reported, never
  auto-fixed.

## Allowed read scope

All `*.csproj`, `package.json`, `package-lock.json`, `.npmrc`, `angular.json`, plus any source
files needed to find usage sites. Web research on changelogs/release notes is allowed and must be
cited.

## Allowed write scope

- Default: **none**.
- Only after explicit approval of `<package> → <version>`: the relevant manifest(s) and the code
  changes strictly required by that upgrade. Lock files change only as a regeneration side effect
  of the standard install/restore command — never edited by hand.

## Forbidden actions

Baseline rules, plus:
- `npm audit fix --force`, `npm update` (bulk), `ncu -u`, or any bulk/forced upgrade command.
- Hand-editing `package-lock.json`.
- Major-version bumps, or bundling multiple packages into one approval.
- Removing `.npmrc`/`legacy-peer-deps` as a side effect (its removal is its own task).

## Required inputs

1. Target: specific package(s), "full report", or "Angular upgrade path".
2. If (and only if) an upgrade should be executed: explicit `<package> <current> → <target>`
   approval in this task.

## Workflow

1. Inventory current versions (manifests + lock file) for the target scope.
2. Find every actual usage site in this repo (imports, APIs called, config touched) — the
   assessment is about *our* exposure, not the package's full changelog.
3. Map usage sites against release notes / migration guides between current and target versions;
   cite sources. Note peer-dependency ripple effects (critical for Angular 13's ecosystem).
4. Classify risk (low/med/high) with the specific call sites that would break, migration steps,
   and estimated blast radius. For Angular: one major version per step, each step independently
   validated.
5. Produce the validation plan using only verified commands (baseline table).
6. If an execution approval exists: state a plan, perform exactly that one bump + required code
   changes, run the full validation, report. Otherwise stop after the assessment.

## Required validation commands

Assessment mode: none required.
Approved-execution mode (all must pass):

| Working dir | Command | Must show |
|---|---|---|
| repo root | `dotnet build src/Server/SuperScrabble.sln` | 0 errors (NuGet changes) |
| `src/Server` | `dotnet test -c Release` | 100% pass (NuGet changes) |
| `src/ClientApp/super-scrabble-app` | `npm install` then `npm run build` | Success (npm changes); report lock-file delta size |
| `src/ClientApp/super-scrabble-app` | `npm test` | **Unverified baseline** — run, report, but do not gate on pre-existing failures |

## Stop and escalation rules

Baseline rules, plus stop when:
- the approved bump forces a cascade (peer deps demanding further majors) — report the cascade,
  do not follow it;
- required code changes touch protected areas (auth, hub, config files);
- `npm install` after the bump changes packages beyond the approved one in a way that alters
  runtime behavior — report and ask.

## Invocation template

```
/dependency-upgrade-assessment
Target: <package(s) | full report | angular upgrade path>
Execution approval (optional, one package only): <package> <current> → <target>
```

## Final report template

```
## dependency-upgrade-assessment report
Target: <…>   Mode: <assessment | approved execution>
| Package | Current | Target | Our usage sites | Breaking changes hitting us (cited) | Risk | Migration steps | Validation plan |
Executed (if approved): <package, files changed, lock-file delta>
Validation: <commands + exact results | none (assessment only)>
Cascades / follow-ups: <…>
Sources cited: <changelog/release-note links>
Status: ASSESSED | UPGRADED | ESCALATED (<reason>)
```
