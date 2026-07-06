---
name: repo-docs-maintainer
description: Keep root Markdown docs (CLAUDE.md, HOW_TO_RUN.md, README.md, plans/reports) accurate after code changes. Verifies every documented command by running it before claiming it works; labels anything unrunnable as Unverified. Edits Markdown only.
---

**Read `.claude/skills/_baseline.md` first and comply with it. Priority: P2.**

## Purpose

Documentation drifts after every merged change. This skill re-aligns the repo's Markdown docs
with reality: commands, paths, ports, configuration keys, and described behavior — with the rule
that **nothing is documented as working unless it was executed successfully in this session**.

## Non-goals

- Never changes code, configuration, or anything non-Markdown to "make the docs true".
- Does not author marketing/presentation material (`Docs/` is out of scope — those are
  presentation binaries, not engineering docs).
- Does not invent architecture intent; undocumentable-from-code claims are attributed or dropped.

## Allowed read scope

Entire repository (read-only outside the write scope), plus running the documented commands.

## Allowed write scope

- Root `*.md` files only (`CLAUDE.md`, `HOW_TO_RUN.md`, `README.md`, `AUDIT_REPORT.md`,
  `IMPLEMENTATION_PLAN.md`, and new root-level engineering docs when asked).
- `docs/**` **if that directory exists** (it does not exist today — do not create it unless the
  task asks).
- `.claude/skills/*.md` and `.claude/skills/*/SKILL.md` only when the task is explicitly about
  the skill system itself.

## Forbidden actions

Baseline rules, plus:
- Any edit outside the write scope, including "just fixing" a code comment or config value the
  docs contradict — report the contradiction instead.
- Deleting a documentation section without prior approval (see Stop rules).
- Documenting a command as working that was not run successfully in this session — label it
  `Unverified` instead.
- Touching `Docs/` (presentation assets) or `resources/`.

## Required inputs

1. Trigger: a merged diff / commit range, a specific doc, or "full refresh of <file>".
2. Optional: known behavior changes the docs must reflect.

## Workflow

1. Read the target doc(s) fully and the code/config areas they describe.
2. Build a claim inventory: every command, path, port, config key, and behavioral statement.
3. Verify each claim:
   - **Commands:** run them (respecting baseline safety — never DB/deploy commands; those are
     verified only as far as safe, then labeled). Record exact output.
   - **Paths/keys:** check existence with file tools.
   - **Behavior:** confirm in code, cite file:line in the report.
4. Edit the doc: correct what's wrong, label what couldn't be verified as `Unverified`, keep the
   author's structure and tone; propose (don't perform) deletions of obsolete sections.
5. Report with the verification matrix. Do not commit.

## Required validation commands

Exactly the commands the target doc claims (each run before being documented as working), e.g.
from `HOW_TO_RUN.md`: backend build/test, `npm install`, `npm run build`, `npm start`. Commands
that are unsafe to run under the baseline (DB resets, deploys) are labeled `Unverified — unsafe
to auto-run` with the reason.

## Stop and escalation rules

Baseline rules, plus stop when:
- a documented command **fails**: report the failure verbatim — the fix belongs to a code-side
  skill, not to "adjusting the docs to hide it" (unless the doc itself is wrong about usage);
- a section appears obsolete: propose deletion with justification and wait for approval;
- two docs contradict each other on intent (e.g. plan vs how-to): surface both, ask which wins.

## Invocation template

```
/repo-docs-maintainer
Target: <doc file(s) | commit range | "full refresh of X">
Known changes to reflect (optional): <…>
Deletion approval granted (optional): <section names | none>
```

## Final report template

```
## repo-docs-maintainer report
Target: <…>
Verification matrix:
| Claim (command/path/behavior) | Method | Result | Doc action taken |
|---|---|---|---|
| … | ran it / file check / code read (file:line) | pass/fail/Unverified | corrected / kept / labeled Unverified |
Sections proposed for deletion (NOT deleted): <list + reason | none>
Contradictions found: <…>
Files changed: <list>
Status: DONE | ESCALATED (<reason>)
```
