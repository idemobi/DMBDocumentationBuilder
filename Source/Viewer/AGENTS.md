# AI Rules - DMBDocumentationViewer

## Scope

- Applies to the `DMBDocumentationBuilder/Source/Viewer` folder and descendants.
- This project is autonomous: required rules are defined in local documentation files.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBDocumentationViewer`
- Project folder: `DMBDocumentationBuilder/Source/Viewer`
- Project role: runtime documentation viewer, SQLite query layer, sidebar integration, and MCP documentation tool package.
- Publication host: `labs_idemobi_com`
- Documentation strategy: viewer-first; AI prepares documentation content, the developer runs generation or verification commands when requested.

## Module intent

- Provide the runtime documentation browsing package for generated PageBuilder ecosystem documentation.
- Keep documentation routes, query models, sidebar integration, embedded views, and MCP tool outputs stable for consumers.
- Keep query behavior read-only and deterministic.

## Key constraints

- Keep public APIs backward compatible unless a change request explicitly allows breakage.
- Prefer additive changes over structural rewrites.
- Do not introduce a new database format, frontend framework, or routing model without explicit approval.
- Do not run `dotnet build`, `dotnet test`, `dotnet restore`, or `dotnet format` unless explicitly requested.

## Documentation objective

- Documentation must be authored so it can be extracted and rendered by DocumentationViewer.
- Publication target is `labs_idemobi_com`.
- Documentation output must serve both developers and AI assistants.
- XML documentation comments must be written in English.
- Public classes, public interfaces, public methods, public constructors, public properties, public constants, public enums, public enum values, and protected contract members must have useful XML documentation.

## Local rule sources

- Use [DOCUMENTATION_RULES.md](DOCUMENTATION_RULES.md) for XML HeaderDoc, README/reference documentation, and DocumentationViewer-ready documentation.
- Use [EXAMPLES_AND_TUTORIALS_RULES.md](EXAMPLES_AND_TUTORIALS_RULES.md) only when creating or updating example, demo, information, instruction, concept, or tutorial pages.
- Use [DRAWIO_DIAGRAM_RULES.md](DRAWIO_DIAGRAM_RULES.md) when adding editable Draw.io diagrams to information, instruction, concept, architecture, request-flow, query-flow, example, or tutorial pages.
- Use `CodeBlockBuilder` or the local `Html.CodeBlock(...)` helper for code examples in information, instruction, concept, example, and tutorial pages.
- Use `ActionItem` with `ButtonRender` for page action links when the host project exposes those helpers.
- Store editable Draw.io diagrams as enriched `.drawio.svg` files under `labs_idemobi_com/wwwroot/drawio/{Area}/`.

## Localization

- Follow local [LOCALIZATION_NOMENCLATURE.md](LOCALIZATION_NOMENCLATURE.md).
- Do not assume external localization rules unless duplicated here.

## Before delivery

- Update local docs when behavior changes.
- State untested areas explicitly.
- Do not claim build/test, database generation, or DocumentationBuilder execution when they were not run.
