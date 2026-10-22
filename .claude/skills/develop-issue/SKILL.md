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
  `dotnet ef migrations add <Name> -p src/CafeKitchen.Infrastructure -o Persistence/Migrations`.
- Match the surrounding code's style, naming and comment density. Do not refactor unrelated code.
- Allergen wording: never say "safe" or "free from". "Not declared" is not "none".
- Commit in small logical steps. Message style: imperative subject, short body, and end with the attribution line from the session's commit instructions.

## 4. Test

1. Test projects live in `tests/` (`CafeKitchen.Domain.Tests`, `CafeKitchen.Application.Tests`, `CafeKitchen.IntegrationTests`). If the one you need does not exist yet, create it (xUnit) and add it to `CafeKitchen.sln`.
2. Write tests for every acceptance criterion that is testable. Name scenarios in business language, e.g. `Recipe_with_undeclared_ingredient_three_levels_deep_is_not_confirmed`. Domain rules B1–B8 need tests, with nested recipes at least three levels deep.
3. Run `dotnet build CafeKitchen.sln` and `dotnet test CafeKitchen.sln`. Fix failures; do not weaken or delete a test to get green.
4. For UI changes, also run the app and exercise the page:
   - `dotnet run --project src/CafeKitchen.Web --launch-profile http` (background), then request the affected pages with `curl` and check status and key content. SQL Server Express must be running (`MSSQL$SQLEXPRESS`).
   - Stop the app afterwards.
5. Report honestly: if something could not be tested, say so in the PR; do not claim it was verified.

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
   - **Testing**: commands run and results, plus anything not tested
   - **Notes**: decisions taken, migrations added, follow-ups
   - End with the PR attribution line from the session's instructions.
4. Never merge the PR and never push to `main`. Hand the PR URL back to the user for review.
5. Do not edit other issues. Mention follow-up work in the PR and let the user decide whether to open issues.

## Final message to the user

Give: the PR URL, a two-line summary, test results, and anything left undone.
