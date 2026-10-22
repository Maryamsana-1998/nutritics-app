# Café Kitchen App — Requirements (MVP)

Oct 20, 2026 · @Maryam Sana

## Purpose and scope

The MVP is a single source of dish data for one café: what it buys, what it makes, and what it serves, with allergens, nutrition and cost worked out automatically.

It replaces the hand-kept allergen spreadsheet and gives customers an allergy filter on the café's menu. It is built for one business, with no multi-café or multi-tenant support.

## Glossary

These terms are used exactly as defined in the code, UI and docs. The "Avoid" words are never used for these concepts.

| Term | Meaning | Avoid |
| --- | --- | --- |
| Ingredient | Something bought in, with its declared allergens, nutrition and price | Product, item, raw material |
| Discontinued | An ingredient the supplier no longer sells; recipes using it are flagged, not changed |  |
| Recipe | A list of ingredients and other recipes, each with a quantity, plus a yield | Card, formula |
| Prep | A role: a recipe used inside another recipe | Sub-recipe, component, batch |
| Dish | A role: a recipe on a menu (a recipe can be a prep and a dish) | Menu item, product |
| Yield | What one batch makes: finished weight and number of portions | Makes, batch size |
| Allergen | One of the 14 allergens UK/EU law requires to be declared, nothing else | Intolerance, dietary flag |
| Declared | An ingredient whose allergens are known: a list, or explicitly none |  |
| Not declared | An ingredient whose allergens nobody has stated; not the same as none | Blank, none, allergen-free |
| Not confirmed | A recipe containing anything not declared, at any depth | Unknown, safe |
| Held back | An import row whose allergens were kept because the file would remove one |  |
| Roll-up | A recipe's allergens, nutrition and cost, worked out from everything in it | Matrix, summary |
| Menu | The dishes the café serves, in named sections | Matrix |
| Special | A dish marked as short-running on a menu |  |
| Allergy filter | A customer's ticked allergens applied to a menu; never says "safe" | Safe filter, free-from |

## Users and roles

The MVP has two kinds of user and no separate admin role.

| User | Access | Login |
| --- | --- | --- |
| Staff | Manage ingredients, imports, recipes, prices and menus; add other staff accounts | Required |
| Customer | View the public menu and use the allergy filter, read-only | None |

## Functional requirements

Eight feature areas make up the MVP. Each requirement has an ID so features and tests can refer back to it.

### F1 Ingredients

- **F1.1** Staff can create, edit and search ingredients.
- **F1.2** Each ingredient records a name, supplier, pack size, pack price, and unit (g, ml or each). Items sold "each" also record a weight per unit.
- **F1.3** Each ingredient records allergens as not declared, declared none, or a declared list chosen from the 14.
- **F1.4** Each ingredient can record nutrition per 100 g (see business rule B4).
- **F1.5** Staff can mark an ingredient as discontinued.

### F2 Supplier import

- **F2.1** Staff can upload a CSV in a fixed template to create or update ingredients, matched by supplier and name.
- **F2.2** Before saving, the app shows a preview of new, changed and held-back rows.
- **F2.3** A row that would remove an allergen the app holds keeps the app's allergens and goes to a held-back list. The rest of the row is imported.
- **F2.4** Staff can review each held-back row and accept or reject the allergen change.

### F3 Recipes

- **F3.1** Staff can create and edit recipes made of lines, each line being an ingredient or another recipe with a quantity.
- **F3.2** Each recipe records a yield: finished weight (entered by hand) and number of portions.
- **F3.3** The app blocks any change that would make a recipe contain itself, directly or indirectly.
- **F3.4** Each recipe shows where it is used: in which recipes (as a prep) and on which menus (as a dish).

### F4 Roll-up

- **F4.1** Every recipe shows its rolled-up allergens: a list, plus "not confirmed" when anything inside is not declared.
- **F4.2** Every recipe shows nutrition per 100 g and per portion, marked incomplete when data is missing.
- **F4.3** Every recipe shows cost per batch and per portion.
- **F4.4** Roll-ups update automatically whenever an ingredient or recipe changes.

### F5 Costing

- **F5.1** Staff can set a selling price on each dish.
- **F5.2** The app shows gross profit and margin % per dish.

### F6 Menus

- **F6.1** Staff can create menus (for example, breakfast and lunch) with named sections containing dishes.
- **F6.2** Staff can mark a dish on a menu as a special.
- **F6.3** Staff can switch a menu between active and inactive. Only active menus are public.

### F7 Public menu and allergy filter

- **F7.1** Customers can view active menus without logging in.
- **F7.2** Customers can tick allergens, and each dish then shows "contains", "none declared" or "not confirmed" (see B5).
- **F7.3** Each dish shows its full allergen list and nutrition per portion.

### F8 Attention list

- **F8.1** Staff see one screen listing what needs action: not-confirmed recipes, recipes using discontinued ingredients, and held-back import rows.

## Business rules

These rules define how the roll-up and the allergy filter behave. They are the most heavily tested part of the app.

- **B1 Allergen states.** An ingredient's allergens are not declared, declared none, or a declared list. An empty list never means none.
- **B2 Allergen roll-up.** A recipe's allergens are the union of all allergens in its lines, at any depth. If any line, at any depth, is not declared, the recipe is not confirmed. A recipe can list allergens and be not confirmed at the same time.
- **B3 Units.** Quantities are in g, ml or each. 1 ml counts as 1 g. "Each" converts to grams using the ingredient's weight per unit.
- **B4 Nutrition.** Tracked per 100 g: energy (kJ and kcal), fat, saturates, carbohydrate, sugars, fibre, protein, salt. A recipe's nutrition per 100 g is its total nutrition divided by the yield's finished weight. Missing ingredient data marks the result incomplete and lists the gaps; it does not block anything.
- **B5 Allergy filter.** For each dish: if it contains any ticked allergen, it shows "contains". Otherwise, if it is not confirmed, it shows "not confirmed". Otherwise it shows "none declared". The app never shows "safe" or "free from".
- **B6 Cost.** A line's cost is its quantity as a share of the ingredient's pack size, times the pack price. A prep's cost is its batch cost scaled by the quantity used against its yield weight.
- **B7 Discontinued.** A discontinued ingredient flags every recipe that uses it, at any depth. Recipes are never changed automatically.
- **B8 Import ratchet.** An import may add allergens or change an ingredient from not declared to declared. It may never remove an allergen without a staff member accepting it.

## Non-functional requirements

- **N1 Platform.** Built with .NET (current LTS), with the exact stack decided in the technical design step.
- **N2 Testing.** Every business rule (B1–B8) has automated tests, including nested recipes at least three levels deep.
- **N3 Accessibility.** The public menu works on mobile and meets WCAG 2.2 AA, including screen readers and keyboard use.
- **N4 Security.** Staff pages require login; the public menu exposes only active menus.
- **N5 Performance.** The public menu with its filter loads in under 2 seconds for a menu of 100 dishes.
- **N6 Data.** Staff can export ingredients and recipes as CSV for backup.

## Out of scope

These are deliberately left out of the MVP, in the rough order they could be added later.

| Phase | Feature |
| --- | --- |
| 2 | Printable food labels (UK PPDS / Natasha's Law) |
| 2 | Carbon calculation, reusing the roll-up |
| 3 | Digital publishing: embeddable menus, QR codes, screens |
| 3 | Meal planning |
| Later | Meal ordering |
| Later | Multiple cafés or multi-tenant support |
| Later | "May contain" and trace warnings |
| Later | Admin role, menu date ranges, full change history, liquid densities |

## Approval gate

Once approved, this document is the input to Step 2, the technical design. Changes after approval are added here first, then reflected in the design and backlog.

- [ ] Glossary agreed
- [ ] Functional requirements F1–F8 agreed
- [ ] Business rules B1–B8 agreed
- [ ] Out-of-scope list agreed
- [ ] Approved to start technical design
