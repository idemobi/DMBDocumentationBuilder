# DMBDocumentationBuilder

`DMBDocumentationBuilder` extracts documentation from source projects and writes viewer-ready documentation data.

The generated SQLite database includes object pages, object source snapshots, captured C# source files, imported OpenAPI documents, REST operation indexes, sidebar entries, project context files, and granular member rows in `DocumentationMembers` for compact runtime rendering by `DMBDocumentationViewer`.

`DMBDocumentationBuilder` generates API documentation pages for PageBuilder ecosystem packages.

It reads C# projects with Roslyn, extracts XML documentation comments, groups public types by namespace and documentation group, renders static documentation pages, writes sidebar/index artifacts, and stores searchable metadata in SQLite.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBDocumentationBuilder`
- Project folder: `DMBDocumentationBuilder`
- Project role: documentation extraction and page generation package.
- Primary consumers: `labs_idemobi_com` and PageBuilder ecosystem packages that publish generated API documentation.
- Main dependencies: Roslyn (`Microsoft.CodeAnalysis.CSharp`) and SQLite (`Microsoft.Data.Sqlite`).
- Documentation generation strategy: DocumentationBuilder-first; AI prepares content and rules, the developer runs generation.

## Main Responsibilities

- Compile project source through Roslyn for documentation extraction.
- Read XML documentation comments and render summaries, remarks, examples, parameters, return values, exceptions, values, type parameters, and see-also links.
- Generate pages for classes, records, structs, interfaces, enums, namespaces, groups, and project context files.
- Generate group pages and group sidebars per package version so runtime navigation does not mix entries from different documentation versions.
- Render two-way object-level dependency graphs from documented source elements, including incoming and outgoing inheritance, interface, and signature-based relations.
- Render generated pages with a semantic heading hierarchy: page title as `h1`, top-level content sections as `h2`, member groups as `h3`, and access-level groups as `h4`.
- Emit display filter metadata on generated member sections and member rows so runtime viewers can personalize API documentation visibility.
- Generate shared sidebar sources and searchable object metadata.
- Capture project C# source files in `DocumentationSourceFiles` so DocumentationViewer MCP tools can support coding, refactoring, and deduplication workflows.
- Import configured OAS3/OpenAPI JSON documents into SQLite and index REST operations for viewer navigation and MCP lookup.
- Preserve deterministic paths, anchors, links, and ordering so generated pages remain stable between runs.
- Store object metadata, object source snippets, full C# source file snapshots, OpenAPI documents, REST operation contracts, and project context in SQLite for downstream search, navigation, and MCP access.

## Non-Goals

- It is not a visual component framework.
- It is not a replacement for XML documentation authoring inside source projects.
- It should not own project-specific tutorial content except through generated documentation output.
- It should not mutate source projects while generating documentation.

## Working Rules

- Keep generation deterministic.
- Treat file paths, project references, XML docs, and generated HTML as public output contracts.
- Keep generated links stable once published.
- Avoid changing `.csproj` files unless the task explicitly requires it.
- Do not run `dotnet build`, `dotnet test`, `dotnet restore`, or `dotnet format` unless explicitly requested.

## Documentation

Local AI and documentation rules live in:

- `AGENTS.md`
- `AI_CONTEXT.md`
- `DOCUMENTATION_RULES.md`
- `PROJECT_MAP.md`
- `DELIVERY_CHECKLIST.md`
- `TROUBLESHOOTING.md`
