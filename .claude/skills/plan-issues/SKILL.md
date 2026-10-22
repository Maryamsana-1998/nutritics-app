---
name: plan-issues
description: Management skill. Read the MVP requirements and architecture markdown files and turn them into a complete set of GitHub issues, one per feature, with acceptance criteria, labels and a build order. Use when the user says "plan the issues", "create the backlog from the docs", or invokes /plan-issues [requirements.md] [architecture.md].
---

# Turn the requirements and architecture into GitHub issues

Inputs (arguments, defaults in brackets): the requirements file [`docs/01-requirements.md`] and the architecture file [`docs/02-architecture.md`]. Read both fully before planning. Never invent requirements that are not in them.

## 1. Analyse

1. List every requirement ID (F*, B*, N*) and the out-of-scope table. Out-of-scope items never become issues.
2. From the architecture, note the stack, layers and dependency rule, the domain model, the testing strategy, CI and deployment, and security.
3. Inspect what already exists: `git log`, the `src/` and `tests/` folders, and `gh issue list --state all --limit 200`. Mark requirements that are already partly or fully built, and do not duplicate an existing issue: update it with `gh issue edit` or skip it, and say which in the plan.

## 2. Split into issues

One issue per **feature** a developer can build, test and review in one pull request (roughly a day or two). Split a requirement area that is too big (for example supplier import becomes parse and preview, ratchet and held-back rows, and review). Do not make issues smaller than a user-visible or testable behavior.

Always include these foundation issues (skip one only if it already exists):

1. **Unit tests for the business rules** (every B rule, and N2's "nested recipes at least three levels deep").
2. **SQL schema defined in the repo and kept current**: `docs/schema.sql` generated from the migrations (`dotnet ef migrations script --idempotent`), `docs/03-database-schema.md` with tables and an ER diagram, and a CI check that fails when the script drifts from the migrations.
3. **Integration test harness**: real database through Testcontainers; mappings, migrations, constraints and repositories.
4. **End-to-end smoke test harness**: the most basic scenario against the running app and a real database, reused and extended by later issues.
5. **CI**: GitHub Actions runs build and **all** tests (unit, integration, end to end) and the schema drift check on every pull request, and branch protection on `main` requires it.
6. **Dev setup**: local database (Docker), README, sample data seeding.
7. **Authentication**, if the requirements or architecture call for it.

Then one issue per feature from the requirements, plus issues for non-functional requirements that need their own work (accessibility, performance, export/backup).

## 3. Rules every issue must follow

Put these into each issue's acceptance criteria, not just the foundation ones:

- [ ] **Unit tests** for the feature's logic (mandatory; logic lives in Domain or Application so it can be tested).
- [ ] **Integration tests** if the feature touches persistence.
- [ ] **End-to-end test** scenario if the feature adds or changes a user-facing page or flow.
- [ ] **Schema updated** if the database changes: new migration, regenerated `docs/schema.sql`, updated `docs/03-database-schema.md`.
- [ ] All tests pass in GitHub Actions on the pull request.

Exempt only infrastructure-only issues (CI, Docker, docs), and say so in the issue.

## 4. Issue format

Title: short, starts with the feature area when there is one (`F3: Recipe editor`). Body:

```
<one or two sentences of what and why>

Covers **F3.1, F3.2** (requirement IDs, plus business rules B*, non-functionals N*).
Depends on: #<n> (only if a real dependency exists; omit otherwise)
Already built: <what exists, if the feature extends existing code>

**Acceptance criteria**
- [ ] <behavior, testable>
- [ ] ...
- [ ] Unit tests ... / Integration tests ... / E2E scenario ... (as applicable)
- [ ] Schema updated (or: no schema change)
```

Acceptance criteria are concrete and testable, taken from the requirement wording and business rules. Use the glossary terms exactly and never the words in its "Avoid" column.

Labels: `feature`, `testing`, `infra`, `nfr`, `security`, plus one label per feature area (for example `F1-ingredients`). Create missing labels with `gh label create <name> --force`.

## 5. Confirm, then create

1. Show the user the proposed issue list first: number, title, requirement IDs, dependencies, labels, and the suggested build order (foundation issues first: tests, schema, CI, dev setup, auth; then features in dependency order). Note which existing issues will be updated rather than created.
2. Wait for the user's approval. Creating issues on a public repository is outward-facing, so do not create anything before they confirm.
3. Create issues with `gh issue create --title ... --label ... --body-file <file>`; write bodies to files in the scratchpad directory rather than inline to avoid quoting problems. After creating, fill in real issue numbers in the `Depends on` lines with `gh issue edit`.
4. Never close or delete existing issues. Do not start development; that is the `develop-issue` skill.

## Final message to the user

Give a table of issue number, title and labels, the recommended build order, and what was skipped or updated because it already existed.
