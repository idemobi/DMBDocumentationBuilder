# DMBDocumentationBuilder Architecture Decisions

## Project-specific section

When copying this file to another DocumentationBuilder ecosystem project, update this section first.

- Project name: `DMBDocumentationBuilder`
- Architectural role: documentation extraction and generated API documentation layer.
- Main stability concerns: public launchers, project descriptors, page managers, renderers, XML comment rendering, generated paths, anchors, metadata output, sidebar output, and SQLite persistence.
- Documentation target: DocumentationBuilder output rendered in `labs_idemobi_com`.

## ADR-001: Rendering foundation stability

- Date: 2026-05-14
- Context: The package is consumed by multiple modules; rendering, builder, helper, or metadata regressions propagate quickly.
- Decision: Prefer backward-compatible additive changes for public generation contracts, descriptors, renderers, and output formats.
- Consequences: Refactors may be slower, but consumer safety is improved.
- Status: Accepted

## ADR-002: DocumentationBuilder-first documentation

- Date: 2026-05-14
- Context: Documentation is expected to be extracted/generated into `labs_idemobi_com`.
- Decision: Author docs in extraction-friendly structure and metadata style.
- Consequences: More disciplined writing format, better automation quality.
- Status: Accepted

## ADR-003: Project-autonomous AI rules

- Date: 2026-05-14
- Context: Different projects need explicit and independent AI guidance.
- Decision: Keep local rules complete in module docs, without implicit inheritance.
- Consequences: Some duplication, but clearer execution for AI tools.
- Status: Accepted
