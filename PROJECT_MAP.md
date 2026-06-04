# DMBDocumentationBuilder Project Map

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBDocumentationBuilder`
- Project root folder: `DMBDocumentationBuilder`
- Main role: documentation extraction and generation package.
- Builder project folder: `Source/Builder/`
- Viewer project folder: `Source/Viewer/`
- Test project folder: `Source/Test/`
- ImprovementByAI project folder: `Source/ImprovementByAI/`
- ExampleToRaw project folder: `Source/ExampleToRaw/`
- Labs project folder: `Labs/`
- Website project folder: `Website/`
- Local PreBuilding program folder: `Programs/PreBuilding/`
- Unit test project folders: `UnitTests/Builder/`, `UnitTests/Viewer/`, `UnitTests/Test/`, and `UnitTests/ImprovementByAI/`
- Important Builder folders: `Source/Builder/General/`, `Source/Builder/Shared/`, `Source/Builder/ForClass/`, `Source/Builder/ForRecord/`, `Source/Builder/ForStruct/`, `Source/Builder/ForInterface/`, and `Source/Builder/ForEnum/`.
- Documentation target: `labs_idemobi_com`

## Folder Responsibilities

- `Source/Builder/General/`
  - High-level orchestration and shared project documentation pages.
  - Includes extraction managers, source file snapshot extraction, OpenAPI document extraction, project descriptors, group/namespace models, path helpers, database persistence, sidebar generation, project context extraction, and object page rendering.

- `Source/Builder/Shared/`
  - Cross-cutting rendering and extraction helpers.
  - Includes XML comment rendering, code block rendering, keyword extraction, type registry helpers, type link models, XML named/link models, symbol display formats, visual helpers, and attribute helpers.

- `Source/Builder/ForClass/`
  - Class-specific extraction models, page managers, renderers, keyword extraction, constructors, fields, properties, and methods.

- `Source/Builder/ForRecord/`
  - Record and record struct extraction models, page managers, renderers, keyword extraction, fields, properties, and methods.

- `Source/Builder/ForStruct/`
  - Struct-specific extraction models, page managers, renderers, keyword extraction, fields, properties, and methods.

- `Source/Builder/ForInterface/`
  - Interface-specific extraction models, page managers, renderers, keyword extraction, properties, methods, and events.

- `Source/Builder/ForEnum/`
  - Enum-specific extraction models, page managers, renderers, keyword extraction, and enum value metadata.

- `Source/Builder/`
  - `DocumentationLauncher.cs`: public entry point for cleaning and running documentation generation.
  - `DMBDocumentationBuilder.csproj`: package metadata and Roslyn/SQLite dependencies.
  - `README.md`: package overview and usage context.
  - `LICENSE.md`, `DMBDocumentationBuilder.png`, and `DMBDocumentationBuilder.snk`: packaged license, icon, and signing key assets.

- `Source/Viewer/`
  - Runtime documentation viewer package, embedded Razor views, host-facing configuration, query services, sidebar integration, MCP tools, `wwwroot` assets, and package assets.

- `Source/Test/`
  - Documentation coverage/sample project, API fixtures, coverage fixtures, tutorial and release-note examples, and OpenAPI sample input.

- `Source/ImprovementByAI/`
  - Console tool for AI-assisted improvement of generated documentation content and documentation databases.
  - Includes provider-specific runtimes and options for Claude, Groq, LM Studio, Mistral, Ollama, and OpenAI.

- `Source/ExampleToRaw/`
  - Example extraction package that mirrors Razor example partials into raw source files for documentation pages.

- `Labs/`
  - Razor pages, controllers, reusable navigation fragments, and view helpers for documentation-family presentation pages displayed by local and final hosts.

- `Website/`
  - Local ASP.NET Core host website for testing Documentation-family labs pages and generated DocumentationViewer data.
  - `Documentation/` is the local output target for generated API and documentation data.

- `Programs/PreBuilding/`
  - Local console program that generates DocumentationViewer data for the `Website/Documentation/` folder.

- `UnitTests/Builder/`
  - Unit tests for the documentation builder package.

- `UnitTests/Viewer/`
  - Unit tests for the documentation viewer package.

- `UnitTests/Test/`
  - Unit tests for the documentation coverage/sample project.

- `UnitTests/ImprovementByAI/`
  - Unit tests for the AI-assisted documentation improvement console tool.

- `bin/` and `obj/`
  - Build outputs and intermediate files. Do not use these folders as documentation or source-of-truth inputs.

## Documentation-Related Files

- `Source/Builder/README.md`: package overview and usage context.
- `Source/Viewer/README.md`: viewer package overview and usage context.
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
