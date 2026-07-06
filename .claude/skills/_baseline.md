# Shared baseline — applies to EVERY skill in this repository

Every skill in `.claude/skills/` inherits these rules. A skill may be *stricter* than this
baseline, never looser. If a skill instruction and this baseline conflict, the stricter rule wins.

## Hard rules

1. **Never expose secrets.** Do not print, quote, or copy secrets, tokens, JWT signing keys,
   connection strings, certificates, or private configuration *values* — refer to them by key
   path only (e.g. `Jwt:SigningKey` in `appsettings.Development.json`). This includes values in
   `appsettings*.json`, `src/ClientApp/super-scrabble-app/src/environments/environment*.ts`,
   user secrets, and `.claude/settings.local.json`.
2. **Never run database or deployment commands.** Forbidden regardless of task:
   `dotnet ef database update`, `dotnet ef migrations remove`, any `DROP`/`DELETE`/`TRUNCATE`
   against a database, `sqllocaldb delete`, `git push`, publish/deploy commands, and any
   destructive shell command (`rm -rf` outside scratchpad, `git reset --hard`, `git clean`).
3. **Never delete, skip, weaken, or rewrite tests to make a change pass.** No `[Fact(Skip=…)]`,
   no loosened assertions, no removed test files. If a test blocks you, stop and report why.
4. **Protected areas — do not modify without explicit task-level approval** (the user must name
   the file in *this* task; approval in a past task does not carry over):
   - `src/Server/WebApi/SuperScrabble.WebApi/Program.cs`
   - Authentication/authorization code anywhere (JWT setup, `[Authorize]` attributes,
     `UsersService` auth paths, `JsonWebTokenGenerator`, `ConfigurationEncryptionKeyProvider`)
   - `src/Server/WebApi/SuperScrabble.WebApi/Hubs/GameHub.cs`
   - `src/Server/WebApi/SuperScrabble.WebApi/Timers/` and `GameLocks.cs`
   - `src/Server/Services/SuperScrabble.Services.Game/Matchmaking/MatchmakingService.cs`
   - `src/Server/Data/SuperScrabble.Data/Migrations/` (existing migrations are immutable)
   - Package manifests (`*.csproj` package references, `package.json`) and lock files
     (`package-lock.json`)
   - Environment/configuration files: `appsettings*.json`, `environment*.ts`, `.npmrc`,
     `angular.json`, `.github/workflows/`
5. **Prefer the smallest reversible diff.** No drive-by refactors, no formatting sweeps, no
   renames beyond what the task requires.
6. **Inspect before editing.** Read the relevant code (and its callers) before changing it.
7. **Plan before code changes.** For any code change, state a short plan (files, approach,
   validation) before the first edit.
8. **Use only verified commands.** Commands must come from `CLAUDE.md`, `HOW_TO_RUN.md`, or
   repository configuration (`package.json` scripts, csproj). If a command cannot be verified
   from those sources or by running it, label it **Unverified** — never invent one.
9. **Run the skill's required validation and report exact results** (pass/fail counts, error
   text). Never claim "tests pass" without having run them in this session.
10. **Stop and escalate** — end the turn with a report instead of proceeding — when:
    - the scope grows beyond what the task named;
    - a protected area turns out to be involved;
    - a required reproduction (failing test) cannot be achieved;
    - validation fails and the fix isn't obviously in scope;
    - the change requires a product, security, or architecture decision.

## Environment facts (verified 2026-07-06)

- Active solution: `src/Server/SuperScrabble.sln`. **Ignore** the stale untracked
  `src/SuperScrabble.sln` — it references projects that do not exist on this branch.
- Backend tests **must run with `-c Release`** on this machine: Windows Smart App Control
  intermittently blocks freshly built Debug DLLs (error `0x800711C7`). This is machine policy,
  not a code problem.
- The folders `src/Common`, `src/Data`, `src/Services`, `src/Tests`, `src/WebApi` contain only
  orphaned `bin`/`obj` from an old branch layout — never read or edit anything under them.

## Verified commands (working directory matters)

| Purpose | Working directory | Command | Status |
|---|---|---|---|
| Backend build | repo root | `dotnet build src/Server/SuperScrabble.sln` | Verified: 0 errors, 9 warnings |
| Backend tests (full) | `src/Server` | `dotnet test -c Release` | Verified: 42/42 pass |
| Frontend build | `src/ClientApp/super-scrabble-app` | `npm run build` | Verified: succeeds; known non-fatal budget warning (initial bundle 1.09 MB > 500 kB budget) |
| Frontend unit tests | `src/ClientApp/super-scrabble-app` | `npm test` | **Unverified** — pass status unknown; do not gate on it, report if you run it |
| Backend run | `src/Server/WebApi/SuperScrabble.WebApi` | `dotnet run` | Documented in `HOW_TO_RUN.md`; needs SQL LocalDB; not re-verified this session |
| Frontend dev server | `src/ClientApp/super-scrabble-app` | `npm start` | Documented in `HOW_TO_RUN.md`; not re-verified this session |

## Reporting

Every skill ends with its own final-report template. Always include: what changed (or "nothing —
read-only"), every command run with its exact result, and anything left unverified or uncertain.
