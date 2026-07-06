---
name: security-configuration-review
description: Read-only security audit of authentication, authorization, JWT validation, CORS, endpoint exposure, token storage/transport, and configuration boundaries across the ASP.NET Core backend and Angular frontend. Never prints secret values, never edits files.
---

**Read `.claude/skills/_baseline.md` first and comply with it. Priority: P1. STRICTLY READ-ONLY.**

## Purpose

Recurring audit of the app's security posture. Known baseline from the 2026-07-06 audit (verify
current state each run — these may have been fixed since): issuer/audience validation off and
`RequireHttpsMetadata=false` (`Program.cs`), anonymous user-lookup endpoints in
`UsersController` (`by/email`, `by/username` — account enumeration), JWT in `localStorage` with
no interceptor/guards, JWT via `access_token` query string for `/gamehub`, no rate limiting.

## Non-goals

- Never remediates — every fix is a separate, human-approved task.
- Not a dependency-CVE scan (see `dependency-upgrade-assessment`).
- Not a penetration test; static review of code and configuration only.

## Allowed read scope

- `src/Server/WebApi/SuperScrabble.WebApi/Program.cs`, `Controllers/**`, `Hubs/**`
- `src/Server/Services/SuperScrabble.Services.Common/**` (JWT generator, key provider),
  `Services.Data/Users/**`
- `appsettings*.json` and `src/environments/environment*.ts` — **key names and structure only**
- Frontend auth surface: `src/app/services/web-requests.service.ts`, `signalr.service.ts`,
  login/register components, `common/utilities.ts`, `app-routing.module.ts`

## Allowed write scope

**None.** Report is chat output only.

## Forbidden actions

Baseline rules, plus:
- Any file edit.
- Printing or quoting any secret **value**: JWT signing keys (including the dev key in
  `appsettings.Development.json`), connection strings, Firebase config values. Refer to keys by
  path (e.g. `Jwt:SigningKey`) and describe properties (length, presence) only when relevant.
- Running the app against any non-local environment; issuing requests to external services.

## Required inputs

Optional focus area (e.g. "JWT pipeline", "endpoint authorization coverage", "CORS"). Default:
full sweep of the checklist below.

## Workflow

1. **AuthN pipeline:** read the JWT bearer setup in `Program.cs` — validation parameters
   (issuer/audience/lifetime/signing key), HTTPS metadata, `OnTokenValidated` behavior, token
   sources (header + `access_token` query for `/gamehub`).
2. **AuthZ coverage:** enumerate every controller action and hub method; table of
   route → `[Authorize]`/anonymous → data exposed. Flag enumeration/IDOR shapes (e.g.
   `GET /api/games/summary/{id}` — verify ownership checks).
3. **Token lifecycle on the client:** storage (`localStorage`), attachment, expiry handling,
   logout behavior.
4. **CORS:** policy vs credentialed requests vs deployed origins.
5. **Configuration boundaries:** which secrets exist, where each lives (key paths), what is
   committed vs environment-supplied, fail-fast behavior.
6. **Abuse resistance:** rate limiting, lockout (`Identity` options), password policy, input
   validation on auth endpoints.
7. Separate **Confirmed** findings (file:line evidence) from **Assumptions** (needs runtime
   verification). Rank by severity.

## Required validation commands

None (read-only). Building is permitted to confirm the code compiles as read; report if run.

## Stop and escalation rules

- Every remediation requires explicit human approval and a separate task — this skill never
  proceeds to fixes, even trivial ones.
- If evidence of an actively exploitable auth bypass is found, mark URGENT at the top.
- If a finding depends on deployment topology (unknown — no CI/CD or hosting config in repo),
  state the assumption and ask.

## Invocation template

```
/security-configuration-review
Focus (optional): <area | full sweep>
```

## Final report template

```
## security-configuration-review report (read-only — no files modified, no secret values printed)
Scope: <…>
### Confirmed findings (evidence-based)
| # | Severity | Finding | Evidence (file:line) | Exploit sketch | Remediation direction | Validation for the fix |
### Assumptions / needs runtime verification
| # | Assumption | How to verify |
Endpoint authorization table: <route → auth → data exposed>
Commands run: <none | list + results>
Status: REPORTED — all remediations require human approval
```
