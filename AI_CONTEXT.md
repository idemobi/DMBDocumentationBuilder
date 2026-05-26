# DMBDocumentationBuilder AI Context

## What This Module Is

`DMBDocumentationBuilder` is the documentation generation engine used to publish API documentation for PageBuilder ecosystem packages.

It should be treated as infrastructure-level tooling: changes can affect generated documentation paths, public API pages, search data, sidebar output, and the developer workflow used by `labs_idemobi_com`.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBDocumentationBuilder`
- Project role: Roslyn/XML documentation extraction, static documentation page generation, index/sidebar generation, and SQLite documentation metadata storage.
- Primary consumers: `labs_idemobi_com` and PageBuilder ecosystem packages.
- Main documentation target: generated documentation rendered in `labs_idemobi_com`.
- Main source inputs: `.csproj` files, C# source files, XML documentation comments, package metadata, and project context files.

## What This Module Is Not

- Not a runtime MVC UI package.
- Not a Bootstrap component library.
- Not a form builder or page builder.
- Not responsible for inventing missing source documentation on behalf of target packages.

## Main Responsibilities

- Build project compilations through Roslyn.
- Extract public type and member metadata for classes, records, structs, interfaces, and enums.
- Render XML documentation comments into HTML-safe documentation fragments.
- Generate documentation pages for types, namespaces, groups, and project context.
- Keep group and namespace documentation scoped by package version when writing routes, sidebar items, and SQLite objects.
- Generate sidebar source files and searchable SQLite metadata.
- Keep generated file paths, anchors, labels, and ordering deterministic.

## Change Strategy For AI

1. Identify whether a change affects extraction, grouping, rendering, paths, anchors, sidebar generation, or SQLite metadata.
2. Preserve generated output compatibility unless the user explicitly asks for a breaking format change.
3. Prefer shared helpers for repeated rendering or extraction behavior.
4. Update XML documentation and local rules in the same change set when behavior changes.

## Documentation Strategy For AI

- Produce extraction-ready XML documentation.
- Explain public contracts in terms of input project metadata, generated output, and stability constraints.
- Use `<see cref="..."/>` for related DocumentationBuilder models and renderers when helpful.
- Do not claim DocumentationBuilder generation, build, or tests unless actually run.
