# AI Rules - DMBDocumentationBuilder

## Scope

- Applies to `DMBDocumentationBuilder` folder and descendants.
- This project is autonomous: required rules are defined in local documentation files.

## Project-specific section

When copying this file to another DocumentationBuilder ecosystem project, update this section first.

- Project name: `DMBDocumentationBuilder`
- Project folder: `DMBDocumentationBuilder`
- Project role: foundational documentation generation package.
- Publication host: `labs_idemobi_com`
- Documentation generation strategy: DocumentationBuilder-first; AI prepares content, the developer executes generation.

## Module intent

- Provide the foundational documentation generation package for the DocumentationBuilder ecosystem.
- Keep documentation metadata, documentation builders, documentation helpers, asset registration, and rendering behavior stable for consumers across the ecosystem.

## Key constraints

- Keep public APIs backward compatible unless a change request explicitly allows breakage.
- Prefer additive changes over structural rewrites.
- Keep builder and rendering behavior deterministic and predictable.
- Do not run `dotnet build`, `dotnet test`, `dotnet restore`, or `dotnet format` unless explicitly requested.

## Documentation objective

- Documentation must be authored so it can be extracted and rendered by DocumentationBuilder.
- Publication target is `labs_idemobi_com`.
- Documentation output must serve both developers and AI assistants.
- AI prepares documentation content and structure; the developer runs DocumentationBuilder.
- XML documentation comments must be written in English.
- Public classes, public methods, public properties, public constants, public enums, public enum values, and other public members must have useful XML documentation.

## Local rule sources

- Use [DOCUMENTATION_RULES.md](DOCUMENTATION_RULES.md) for XML HeaderDoc, README/reference documentation, and DocumentationBuilder-ready documentation.
- Use [EXAMPLES_AND_TUTORIALS_RULES.md](EXAMPLES_AND_TUTORIALS_RULES.md) only when creating or updating example, demo, information, instruction, concept, or tutorial pages.
- Use [DRAWIO_DIAGRAM_RULES.md](DRAWIO_DIAGRAM_RULES.md) when adding editable Draw.io diagrams to information, instruction, concept, architecture, rendering pipeline, example, or tutorial pages.
- Use `CodeBlockBuilder` or the local `Html.CodeBlock(...)` helper for code examples in information, instruction, concept, example, and tutorial pages.
- Use `ActionItem` with `ButtonRender` for page action links when the project exposes those helpers.
- Store editable Draw.io diagrams as enriched `.drawio.svg` files under `labs_idemobi_com/wwwroot/drawio/{Area}/`.

## Localization

- Follow local [LOCALIZATION_NOMENCLATURE.md](LOCALIZATION_NOMENCLATURE.md).
- Do not assume external localization rules unless duplicated here.

## Before delivery

- Update local docs when behavior changes.
- State untested areas explicitly.
- Do not claim build/test or DocumentationBuilder execution when they were not run.
