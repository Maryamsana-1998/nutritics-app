---
name: develop-issue
description: Take a GitHub issue number from this repo, implement it on a branch following the architecture, write and run tests, and open a pull request that closes the issue. Use when the user says "develop issue 5", "work on #12", or invokes /develop-issue <number>.
---

# Develop a GitHub issue end to end

Input: an issue number (argument). If none is given, run `gh issue list --state open` and ask which one.

## 1. Understand the issue

1. `gh issue view <n>` and read the title, body and acceptance criteria.
2. Read `docs/02-architecture.md` and the sections of `docs/01-requirements.md` for the requirement IDs the issue names (F*, B*, N*). Use the glossary terms exactly; never use the words in its "Avoid" column.
3. Check dependencies: if the issue says it depends on another issue, run `gh issue view` on that one and confirm it is merged (`git log origin/main`). If not, stop and tell the user.
4. Explore the existing code the issue builds on before writing anything. Several issues extend code that already exists.
5. If the issue is ambiguous in a way that changes the design, ask the user. Otherwise pick the conventional option and note it in the PR.

## 2. Branch

```
git switch main && git pull --ff-only
git switch -c issue-<n>-<short-kebab-title>
```

Never commit to `main`. Stop if the working tree has unrelated uncommitted changes and ask the user.

## 3. Implement

Follow the dependency rule. It is enforced by project references and must not be bent:

| Layer | Project | May reference |
| --- | --- | --- |
| Domain | `src/CafeKitchen.Domain` | nothing |
| Application | `src/CafeKitchen.Application` | Domain |
| Infrastructure | `src/CafeKitchen.Infrastructure` | Application, Domain |
| Web | `src/CafeKitchen.Web` | Application, Infrastructure (DI only) |

- Business rules go in Domain; use cases and repository interfaces in Application; EF Core, CsvHelper and migrations in Infrastructure; Blazor pages in Web. The Web layer never touches the `DbContext`.
- Staff pages use `@rendermode InteractiveServer`. The public menu is static server rendering (no JavaScript required).
- Schema change: add a migration with
  `dotnet ef migrations add <Name> -p src/CafeKitchen.Infrastructure -o Persistence/Migrations`, and update the schema docs (see section 4).
- Match the surrounding code's style, naming and comment density. Do not refactor unrelated code.
- Allergen wording: never say "safe" or "free from". "Not declared" is not "none".
- Commit in small logical steps. Message style: imperative subject, short body, and end with the attribution line from the session's commit instructions.

## 4. Test

Three tiers of automated tests. All of them run in GitHub Actions on every pull request, so they must pass locally first.

| Tier | Project | What it covers |
| --- | --- | --- |
| Unit | `tests/CafeKitchen.Domain.Tests`, `tests/CafeKitchen.Application.Tests` | Domain rules and use cases, with fake repositories |
| Integration | `tests/CafeKitchen.IntegrationTests` | EF Core mappings, migrations, repositories and imports against real SQL Server (Testcontainers) |
| End to end | `tests/CafeKitchen.E2ETests` | The running web app driven through a browser (or HTTP) against a real database |

**Requirements for every issue**
- **Unit tests are mandatory.** No unit tests, no pull request. Put logic in Domain or Application (not in `.razor` code) so it can be unit tested. Before step 5, confirm `git diff --stat main -- tests` shows new or changed tests exercising the new behavior.
- **Integration tests** are required whenever the issue touches persistence (entities, mappings, migrations, repositories, import).
- **End-to-end tests** are required when the issue adds or changes a user-facing page or flow: add or extend a scenario covering the main path.
- Only infrastructure-only issues (CI, Docker, docs) are exempt, and the PR must say so explicitly.
- If a test project does not exist yet, create it (xUnit) and add it to `CafeKitchen.sln`. If the integration or end-to-end harness is not built yet (see the open issues for them), say so in the PR instead of inventing a throwaway harness.

Steps
1. Write tests for every acceptance criterion that is testable. Name scenarios in business language, e.g. `Recipe_with_undeclared_ingredient_three_levels_deep_is_not_confirmed`. Domain rules B1-B8 need tests, with nested recipes at least three levels deep.
2. Run `dotnet build CafeKitchen.sln` and `dotnet test CafeKitchen.sln`. Fix failures; do not weaken or delete a test to get green.
3. For UI changes, also run the app and look at the affected pages:
   - `dotnet run --project src/CafeKitchen.Web --launch-profile http` (background), then request the pages with `curl` and check status and key content. SQL Server Express must be running (`MSSQL$SQLEXPRESS`).
   - Stop the app afterwards.
4. Report honestly: if something could not be tested, say so in the PR; do not claim it was verified.

**SQL schema stays current**
Every issue that changes the database must update the schema definition in the same pull request, once it exists (see the schema issue):
- add the EF migration, then regenerate `docs/schema.sql` with `dotnet ef migrations script --idempotent -p src/CafeKitchen.Infrastructure -o docs/schema.sql`
- update the tables and ER diagram in `docs/03-database-schema.md`
- CI fails if `docs/schema.sql` does not match the migrations
List the schema changes under **Notes** in the PR, or state "no schema change".

## 5. Pull request

1. Tick the acceptance criteria you completed; list any you did not complete and why.
2. `git push -u origin <branch>`
3. Open the PR:

```
gh pr create --base main --title "<issue title>" --body-file <file>
```

   The body must contain:
   - `Closes #<n>`
   - **Summary**: what changed, by layer
   - **Acceptance criteria**: the checklist with ticked / unticked items
   - **Testing**: unit, integration and end-to-end tests added (file and test names, mapped to the acceptance criteria they cover), commands run and results, plus anything not tested
   - **Schema**: schema changes and the updated `docs/schema.sql`, or "no schema change"
   - **Notes**: decisions taken, migrations added, follow-ups
   - End with the PR attribution line from the session's instructions.
4. Never merge the PR and never push to `main`. Hand the PR URL back to the user for review.
5. Do not edit other issues. Mention follow-up work in the PR and let the user decide whether to open issues.

## Final message to the user

Give: the PR URL, a two-line summary, test results per tier, and anything left undone.
