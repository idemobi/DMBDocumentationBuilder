# DMBDocumentationBuilder Local Development Runbook

## Purpose

Guide local development for `DMBDocumentationBuilder` changes.

## Project-specific section

When copying this file to another DocumentationBuilder ecosystem project, update this section first.

- Project name: `DMBDocumentationBuilder`
- Main code areas: `Source/Builder/General/`, `Source/Builder/Shared/`, `Source/Builder/ForClass/`, `Source/Builder/ForRecord/`, `Source/Builder/ForStruct/`, `Source/Builder/ForInterface/`, `Source/Builder/ForEnum/`, and `Source/Builder/DocumentationLauncher.cs`.
- Main risk areas: generated HTML output, metadata ordering, path stability, XML comment rendering, project reference resolution, sidebar generation, and SQLite persistence.
- Documentation target: `labs_idemobi_com`

## Typical workflow

1. Update extraction, renderer, page manager, model, path, database, or launcher code.
2. Update XML HeaderDoc for changed public API surface.
3. Update local markdown docs (`README`, rules, checklists) if behavior changed.
4. Validate downstream usage in nearest consumers.
5. Hand off for developer-run DocumentationBuilder generation.

## Common checks

- Resource key consistency in `Resources/*.resx`.
- Namespace and public contract consistency.
- Null/argument validation behavior.
- Generated HTML, metadata, path, sidebar, and persistence expectations.
- documentation helper discoverability and usage consistency.

## Documentation handoff checks

- Documentation structure is extraction-ready.
- Examples are self-contained.
- Audience (developers + AI) is respected.
