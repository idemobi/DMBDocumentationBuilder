# DMBDocumentationViewer Architecture Decisions

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBDocumentationViewer`
- Architectural role: runtime documentation viewing, generated SQLite querying, sidebar integration, and MCP documentation access.
- Main stability concerns: public routes, view models, query result models, SQLite lookup behavior, MCP output format, sidebar provider contract, embedded Razor views, and configuration defaults.
- Documentation target: DocumentationViewer output rendered in `labs_idemobi_com`.

## ADR-001: Runtime viewer stability

- Date: 2026-05-14
- Context: The package is consumed by host websites and AI tooling; route, query, model, or MCP regressions can break documentation browsing and assistant workflows.
- Decision: Prefer backward-compatible additive changes for public routes, query models, sidebar contracts, MCP tools, and view model shapes.
- Consequences: Refactors may be slower, but consumer safety is improved.
- Status: Accepted

## ADR-002: Read-only query layer

- Date: 2026-05-14
- Context: Generated documentation databases are produced by the builder pipeline and consumed by the viewer.
- Decision: Keep viewer query services read-only and deterministic.
- Consequences: Database creation and migration logic stays outside this package; viewer behavior is easier to reason about.
- Status: Accepted

## ADR-003: DocumentationViewer-first documentation

- Date: 2026-05-14
- Context: Documentation is expected to be extracted/generated into `labs_idemobi_com`.
- Decision: Author docs in extraction-friendly structure and metadata style.
- Consequences: More disciplined writing format, better automation quality.
- Status: Accepted

## ADR-004: Project-autonomous AI rules

- Date: 2026-05-14
- Context: Different projects need explicit and independent AI guidance.
- Decision: Keep local rules complete in module docs, without implicit inheritance.
- Consequences: Some duplication, but clearer execution for AI tools.
- Status: Accepted
