# DMBDocumentationViewer AI Context

## What This Module Is

`DMBDocumentationViewer` is the runtime MVC and MCP layer that displays documentation generated for PageBuilder ecosystem packages.

It should be treated as read-only documentation infrastructure: changes can affect routes, sidebar output, documentation lookup, source-code lookup, project-context lookup, MCP responses, and the experience of `labs_idemobi_com`.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBDocumentationViewer`
- Project role: documentation viewing, SQLite query services, sidebar runtime integration, MVC documentation routes, and MCP documentation tools.
- Primary consumers: `labs_idemobi_com`, developers browsing generated documentation, and AI assistants querying documentation context.
- Main source inputs: generated SQLite databases, embedded Razor views, route values, package metadata, and optional sidebar providers.
- Main output: rendered documentation pages, sidebar models, query results, formatted MCP text, and view models.

## What This Module Is Not

- Not a documentation generator.
- Not a Roslyn extraction package.
- Not a Bootstrap component library.
- Not a form builder or page builder.
- Not responsible for inventing missing source documentation on behalf of target packages.

## Main Responsibilities

- Resolve documentation groups, namespaces, object pages, package ids, versions, and object types from route values.
- Resolve documentation group sidebars from the requested version, or from the latest generated version when the route omits one.
- Preserve resolved versions when creating group breadcrumbs, sidebar return links, and root navigation links.
- Query generated SQLite documentation databases for documentation content and metadata.
- Query source-code and project-context records stored by the documentation generation pipeline.
- Render documentation pages with embedded MVC views.
- Provide sidebar data through `IDocumentationSidebarProvider`.
- Expose MCP tools that return concise, AI-friendly documentation and source context.

## Change Strategy For AI

1. Identify whether a change affects routing, SQLite queries, sidebar construction, view models, MCP formatting, or embedded view rendering.
2. Preserve route and query-result compatibility unless the user explicitly asks for a breaking format change.
3. Keep query services read-only and deterministic.
4. Update XML documentation and local rules in the same change set when behavior changes.

## Documentation Strategy For AI

- Produce extraction-ready XML documentation for public and protected API members.
- Explain public contracts in terms of route inputs, generated database records, returned view models, and read-only behavior.
- Use `<see cref="..."/>` for related controller, model, service, sidebar, and MCP types when helpful.
- Do not claim `dotnet build`, tests, database generation, or DocumentationBuilder execution unless actually run.
