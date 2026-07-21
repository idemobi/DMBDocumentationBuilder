# DMBDocumentationViewer Project Map

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBDocumentationViewer`
- Project root folder: `DMBDocumentationBuilder/Source/Viewer`
- Main role: runtime documentation viewer and query package.
- Important folders: `Configuration/`, `Controllers/`, `Managers/`, `Models/`, `Sidebar/`, `Views/Documentation/`, and `wwwroot/`.
- Documentation host: `labs_idemobi_com`

## Folder Responsibilities

- `Configuration/`
  - Package configuration and application registration.
  - Includes the MVC/MCP integration point used by host applications.

- `Controllers/`
  - Runtime entry points.
  - `DocumentationController` renders group, namespace, object, OpenAPI REST API, content, and generic MCP connection pages.
  - `DocumentationContextPackController` renders the latest-version context pack builder form and ZIP export.
  - `DocumentationMcpTools` exposes documentation, REST API, OpenAPI document, source file, source snapshot, coding context, and project-context lookup tools.

- `Managers/`
  - Read-only query and formatting services.
  - Includes documentation search, related object lookup, OpenAPI lookup, source-code lookup, captured source file lookup, MCP help page model construction, project-context lookup, and MCP text formatting.

- `Models/`
  - Query result models and Razor view models.
  - Includes documentation records, search results, OpenAPI query results, source-code results, captured source file results, project-context file results, AI-render source metadata, context option pack models, and page render models.

- `Sidebar/`
  - Sidebar extension points and default sidebar creation.
  - Allows a host project to provide navigation for documentation root, group, and namespace pages.

- `Views/Documentation/`
  - Embedded Razor views used by the documentation viewer.
  - These views should stay aligned with the route and model contracts exposed by `DocumentationController`, including the dedicated MCP connection page and OpenAPI REST API page.

- Root files
  - `DMBDocumentationViewer.csproj`: package metadata, embedded resources, and dependencies.
  - `README.md`: package overview and usage context.
  - `LICENSE.md`, `DMBDocumentationViewer.png`, and `DMBDocumentationViewer.snk`: packaged license, icon, and signing key assets.

- `bin/` and `obj/`
  - Build outputs and intermediate files. Do not use these folders as documentation or source-of-truth inputs.

## Documentation-Related Files

- `README.md`: package overview and usage context.
- `AGENTS.md`: local AI rules and scope for this package.
- `AI_CONTEXT.md`: additional context for AI-assisted maintenance.
- `DOCUMENTATION_RULES.md`: strict XML documentation and reference documentation policy.
- `DRAWIO_DIAGRAM_RULES.md`: rules for editable Draw.io diagrams used by documentation, concept, instruction, example, and tutorial pages.
- `EXAMPLES_AND_TUTORIALS_RULES.md`: rules for example pages and tutorials only.
- `DELIVERY_CHECKLIST.md`: final quality gate before handoff.
- `ARCHITECTURE_DECISIONS.md`: local architecture decisions and constraints.
- `GLOSSARY.md`: shared vocabulary for this package.
- `LOCAL_DEVELOPMENT_RUNBOOK.md`: local workflow notes and handoff checks.
- `LOCALIZATION_NOMENCLATURE.md`: localization key naming rules for host-facing content.
- `TROUBLESHOOTING.md`: known issues and recovery notes.
