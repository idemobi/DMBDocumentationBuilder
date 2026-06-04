# DMBDocumentationViewer Local Development Runbook

## Purpose

Guide local development for `DMBDocumentationViewer` changes.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBDocumentationViewer`
- Main code areas: `Configuration/`, `Controllers/`, `Managers/`, `Models/`, `Sidebar/`, `Views/Documentation/`, and `wwwroot/`.
- Main risk areas: route compatibility, view-model shape, SQLite query filters, MCP output format, source-context lookup, sidebar provider behavior, embedded views, and host application registration.
- Documentation target: `labs_idemobi_com`

## Typical workflow

1. Update controller, query service, model, sidebar, configuration, view, or MCP formatter code.
2. Update XML HeaderDoc for changed public API surface.
3. Update local markdown docs (`README`, rules, checklists) if behavior changed.
4. Validate downstream usage in nearest consumers.
5. Hand off any build, test, or generation commands for the developer unless explicitly requested.

## Common checks

- Route values and optional parameters remain compatible.
- Query services handle missing databases and empty results predictably.
- MCP tool text remains concise and stable.
- Sidebar provider fallback behavior is documented.
- Embedded views consume the expected model shape.
- Namespace and public contract consistency.
- Null/argument validation behavior.

## Documentation handoff checks

- Documentation structure is extraction-ready.
- Examples are self-contained.
- Audience (developers + AI) is respected.
- Commands that were not run are not claimed.
