---
name: angular-feature-work
description: Implement UI/UX and presentational changes in the Angular SPA (pages, components, dialogs, styles, assets) with build verification. Use for styling, layout, responsiveness/mobile, component behavior, and user-facing text. Escalates before touching dependencies, config, API contracts, auth, or SignalR plumbing.
---

**Read `.claude/skills/_baseline.md` first and comply with it. Priority: P1.**

## Purpose

Deliver one scoped UI/presentation change in the Angular 13 app — visual design, layout,
responsiveness, component interaction, dialogs, or user-facing text — verified by a production
build and, where possible, visual inspection.

## Non-goals

- Not for changing what the app *sends or receives* (HTTP/SignalR payloads) — that is
  `api-contract-sync` territory.
- Not for dependency, framework, or build-config changes (see `dependency-upgrade-assessment`).
- Not for auth flows, token handling, guards, or interceptors.

## Allowed read scope

All of `src/ClientApp/super-scrabble-app/**`; server code read-only when needed to understand a
payload.

## Allowed write scope

- `src/ClientApp/super-scrabble-app/src/app/**` — **excluding** `services/signalr.service.ts`,
  `services/web-requests.service.ts`, and anything that changes an API/hub call shape
- `src/ClientApp/super-scrabble-app/src/styles.scss`
- `src/ClientApp/super-scrabble-app/src/assets/**`

## Forbidden actions

Baseline rules, plus:
- No edits to `package.json`, `package-lock.json`, `angular.json`, `.npmrc`,
  `src/environments/**`, `tsconfig*.json`, `karma.conf.js`.
- No new npm dependencies.
- No changes to `signalr.service.ts`, `web-requests.service.ts`, or auth/token code
  (login/register token storage) without explicit task approval.
- No hardcoded user-facing strings: all user-visible text goes through `LanguageService`
  (`src/app/services/language.service.ts`) with **both** EN and BG entries.
- Respect the existing dark-theme variables in `styles.scss`; don't fork a parallel style system.

## Required inputs

1. The change, described from the user's perspective.
2. Affected pages/components (or "find them").
3. **Visual acceptance criteria** — what must be true on screen (required; ask if missing).
4. Target viewports (e.g. 360px mobile, desktop) when layout is involved.

## Workflow

1. Locate the affected components/styles; read them and neighboring patterns first.
2. State a short plan (files, approach, how it will be verified visually).
3. Implement within the write scope, matching existing component and SCSS conventions.
4. Run the production build.
5. If a dev server is available this session (`npm start`, documented in `HOW_TO_RUN.md`),
   verify visually at the required viewports and capture screenshots. If not available,
   **state explicitly that visual verification was not performed** and list exactly what the
   user must check manually against the acceptance criteria.
6. Report bundle-size delta from the build output. Do not commit.

## Required validation commands

| Working dir | Command | Must show |
|---|---|---|
| `src/ClientApp/super-scrabble-app` | `npm run build` | Success. The pre-existing budget warning (initial 1.09 MB > 500 kB) is known; flag if the bundle *grows* |
| `src/ClientApp/super-scrabble-app` | `npm test` | **Unverified baseline** — not a required gate yet; if run, report results |

## Stop and escalation rules

Baseline rules, plus stop before:
- adding/upgrading any dependency, or touching `angular.json`/environments;
- any change to API-call or hub-event shapes, auth, or `signalr.service.ts`;
- the change demands a design decision with no acceptance criteria to resolve it;
- bundle size grows noticeably (> ~10 kB raw) — report and ask.

## Invocation template

```
/angular-feature-work
Change: <user-visible description>
Pages/components: <list or "locate">
Acceptance criteria: <what must be visible/behave, per viewport>
Viewports: <e.g. 360px, 768px, desktop>
Protected-file approval granted this task: <none | list>
```

## Final report template

```
## angular-feature-work report
Change: <restated>
Files changed: <list>
Validation:
- npm run build → <result, bundle delta vs 1.09 MB baseline>
- Visual verification: <performed (screenshots attached, viewports …) | NOT performed — manual checklist below>
Manual checks for the user (if any): <bullet list mapped to acceptance criteria>
i18n: <EN/BG strings added via LanguageService | not applicable>
Uncertainty: <…>
Status: DONE | ESCALATED (<reason>)
```
