# DMBDocumentationBuilder Project Map

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBDocumentationBuilder`
- Project root folder: `DMBDocumentationBuilder`
- Main role: documentation extraction and generation package.
- Important folders: `General/`, `Shared/`, `ForClass/`, `ForRecord/`, `ForStruct/`, `ForInterface/`, and `ForEnum/`.
- Documentation target: `labs_idemobi_com`

## Folder Responsibilities

- `General/`
  - High-level orchestration and shared project documentation pages.
  - Includes extraction managers, source file snapshot extraction, OpenAPI document extraction, project descriptors, group/namespace models, path helpers, database persistence, sidebar generation, project context extraction, and object page rendering.

- `Shared/`
  - Cross-cutting rendering and extraction helpers.
  - Includes XML comment rendering, code block rendering, keyword extraction, type registry helpers, type link models, XML named/link models, symbol display formats, visual helpers, and attribute helpers.

- `ForClass/`
  - Class-specific extraction models, page managers, renderers, keyword extraction, constructors, fields, properties, and methods.

- `ForRecord/`
  - Record and record struct extraction models, page managers, renderers, keyword extraction, fields, properties, and methods.

- `ForStruct/`
  - Struct-specific extraction models, page managers, renderers, keyword extraction, fields, properties, and methods.

- `ForInterface/`
  - Interface-specific extraction models, page managers, renderers, keyword extraction, properties, methods, and events.

- `ForEnum/`
  - Enum-specific extraction models, page managers, renderers, keyword extraction, and enum value metadata.

- Root files
  - `DocumentationLauncher.cs`: public entry point for cleaning and running documentation generation.
  - `DMBDocumentationBuilder.csproj`: package metadata and Roslyn/SQLite dependencies.
  - `README.md`: package overview and usage context.

- `bin/` and `obj/`
  - Build outputs and intermediate files. Do not use these folders as documentation or source-of-truth inputs.

## Documentation-Related Files

- `README.md`: package overview and usage context.
- `AGENTS.md`: local AI rules and scope for this package.
- `AI_CONTEXT.md`: additional context for AI-assisted maintenance.
- `DOCUMENTATION_RULES.md`: strict documentation policy.
- `DRAWIO_DIAGRAM_RULES.md`: rules for editable Draw.io diagrams used by documentation, concept, instruction, example, and tutorial pages.
- `EXAMPLES_AND_TUTORIALS_RULES.md`: rules for example pages and tutorials only.
- `DELIVERY_CHECKLIST.md`: final quality gate before handoff.
- `ARCHITECTURE_DECISIONS.md`: local architecture decisions and constraints.
- `GLOSSARY.md`: shared vocabulary for this package.
- `LOCAL_DEVELOPMENT_RUNBOOK.md`: local workflow notes and handoff checks.
- `LOCALIZATION_NOMENCLATURE.md`: localization key naming rules for this package.
- `TROUBLESHOOTING.md`: known issues and recovery notes.
