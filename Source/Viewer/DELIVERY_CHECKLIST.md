# DMBDocumentationViewer Delivery Checklist

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBDocumentationViewer`
- Main delivery risk: accidental changes to MVC routes, embedded view models, SQLite queries, sidebar output, MCP text output, source-context lookup, or host integration.
- Publication target: `labs_idemobi_com`
- Documentation generation: developer-run; AI must not claim generation unless it was actually run.

## 1. Code scope

- Change is minimal and focused.
- Public route, query, sidebar, MCP, configuration, and view-model behavior is backward compatible, or breakage is explicit.
- Query services remain read-only unless the user explicitly requests otherwise.

## 2. Localization

- Added keys follow `LOCALIZATION_NOMENCLATURE.md`.
- Resource entries are consistent across required locales when resources are involved.

## 3. Documentation quality

- `README.md` and relevant docs are updated for behavior changes.
- Public API XML HeaderDoc exists for new/updated public and protected-contract items.
- XML tags are valid and useful (`summary`, `param`, `returns`, `value`, etc.).
- Documentation is understandable by both developers and AI assistants.

## 4. DocumentationViewer readiness

- Structure is explicit (Context, Explanation, Example, Notes/Constraints).
- Headings are deterministic.
- Examples are self-contained.
- Publication target is `labs_idemobi_com`.
- Developer will run generation or verification commands when needed; AI does not claim execution.

## 5. Final reporting

- Changed files are listed.
- Untested areas are clearly stated.
- Build/test/database generation commands are reported only if actually run.
