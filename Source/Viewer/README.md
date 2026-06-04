# DMBDocumentationViewer

`DMBDocumentationViewer` is the runtime documentation viewer for the PageBuilder ecosystem. It reads generated documentation metadata from SQLite, renders documentation pages through MVC views, exposes sidebar navigation, and provides MCP tools so AI assistants can query documentation, imported OpenAPI REST contracts, captured C# source files, source snippets, and project context.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBDocumentationViewer`
- Project role: runtime viewer for generated documentation.
- Primary host: `labs_idemobi_com`
- Main consumers: MVC documentation pages, sidebar providers, documentation search, and MCP clients.
- Main data source: generated SQLite documentation databases and embedded Razor views.

## Responsibilities

- Render generated documentation pages through `DocumentationController`.
- Render compact generated member lists through `DocumentationObjectMembersViewComponent` and the `Html.DocumentationObjectMembers(...)` fluent API.
- Query generated documentation, source code snippets, and project context files from SQLite databases.
- Query imported OAS3/OpenAPI documents and REST operation contracts from SQLite databases.
- Query captured C# source files from SQLite for AI coding, refactoring, and deduplication workflows.
- Build documentation sidebars through `IDocumentationSidebarProvider` and `DocumentationSidebarFactory`.
- Resolve group sidebars from the requested package version, falling back to the latest generated version when no version route value is provided.
- Preserve the selected documentation version on group return links, breadcrumbs, and root documentation navigation.
- Provide reader-side display controls for generated member sections when pages expose DocumentationBuilder filter metadata, while showing the menu disabled on documentation pages that do not expose filterable members.
- Expose embedded DocumentationViewer static assets through the host application's static file provider.
- Expose MCP tools through `DocumentationMcpTools` for documentation lookup, REST API operation lookup, OpenAPI document retrieval, search, source file lookup, source snapshot lookup, coding context bundles, and project context retrieval.
- Render dedicated Razor REST API pages for imported OpenAPI documents and operation contracts.
- Render a dedicated Razor MCP connection page from API sidebars so humans can connect AI assistants and inspect the available tool contracts.
- Keep documentation routes stable for generated object pages, namespace pages, and group pages.

## Fluent Member Rendering

Use `Html.DocumentationObjectMembers(...)` when a host page needs an exact member list from the generated documentation database:

```cshtml
@(await Html.DocumentationObjectMembers("BadgeBuilder")
    .ForClass()
    .ShowMethods()
    .DisplaySignatures(includeDescription: true)
    .RenderAsync())
```

## Non-goals

- It does not generate documentation from C# source projects.
- It does not parse Roslyn symbols or XML comments directly.
- It does not replace `DMBDocumentationBuilder`.
- It does not mutate source projects or generated documentation databases.

## Main folders

- `Configuration/`: package configuration and application registration.
- `Controllers/`: MVC documentation routes and MCP tool endpoints.
- `Managers/`: SQLite query services and MCP text formatting helpers.
- `Models/`: view models and query result models.
- `Sidebar/`: sidebar provider contract and default factory.
- `Views/Documentation/`: embedded Razor views used by the runtime viewer.

## Documentation expectations

All public and protected contract APIs must have English XML documentation. Documentation rules are defined in `DOCUMENTATION_RULES.md`; example, tutorial, and concept-page rules are defined in `EXAMPLES_AND_TUTORIALS_RULES.md`.
