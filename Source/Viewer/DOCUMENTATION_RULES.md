# DMBDocumentationViewer Documentation Rules

## Language

- Documentation must be written in English.
- XML documentation comments must be written in English.

## Target audience

- Primary: developers maintaining or integrating `DMBDocumentationViewer`.
- Secondary: developers browsing generated documentation in `labs_idemobi_com`.
- Tertiary: AI assistants consuming structured project rules, MCP tools, and technical context.

Documentation must be useful without private chat context. A reader should understand what the API queries, which generated records it consumes, how route values flow into SQLite lookup services, and what stability constraints apply before reading the implementation.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBDocumentationViewer`
- Primary API families: MVC documentation controller, MCP tools, SQLite query services, MCP text formatter, sidebar provider/factory, runtime configuration, view models, and query result models.
- Important types to reference when relevant: `DocumentationController`, `DocumentationMcpTools`, `DocumentationQueryService`, `SourceCodeQueryService`, `ProjectContextQueryService`, `DocumentationMcpTextFormatter`, `IDocumentationSidebarProvider`, `DocumentationSidebarFactory`, `DocumentationViewerConfiguration`, and the models under `Models/`.
- Publication host: `labs_idemobi_com`
- Documentation strategy: viewer-first; AI prepares content, the developer executes generation or verification commands when requested.

## Strict C# XML documentation policy

- Always write XML HeaderDoc for:
  - public classes,
  - public interfaces,
  - public structs,
  - public methods,
  - public constructors,
  - public properties,
  - public fields,
  - public constants,
  - public events,
  - public delegates,
  - public enums,
  - public enum values,
  - public extension methods.
- Also write XML HeaderDoc for protected members when they are part of an inheritance, MVC, configuration, or rendering contract.
- Internal and private members do not require XML HeaderDoc unless they explain complex routing, query, formatting, rendering, sidebar, or security behavior that would otherwise be difficult to maintain.
- XML documentation must use valid C# XML syntax.
- Prefer these tags:
  - `<summary>`
  - `<param>`
  - `<typeparam>`
  - `<returns>`
  - `<value>`
  - `<remarks>`
  - `<exception>`
  - `<see cref="..."/>`
  - `<seealso cref="..."/>`
- Use `<inheritdoc/>` only when the inherited documentation is accurate for the current member. Do not hide different behavior behind inherited text.

## XML documentation quality standard

XML documentation must explain the public contract, not repeat the member name.

For classes and interfaces, document:

- the type's role in routing, viewing, querying, formatting, sidebar integration, MCP exposure, or configuration,
- the generated records, view models, or tool outputs it consumes or returns,
- the relationship with important types such as `DocumentationController`, query services, MCP tools, sidebar provider/factory, and view models,
- lifecycle expectations, including whether the type is called by MVC, MCP, the host application, or another service.

For methods and constructors, document:

- what the member changes or returns in routed output, query results, formatted text, sidebar output, or configuration,
- every parameter and the expected format when relevant,
- the returned value when the method produces a view result, query result, formatted MCP text, sidebar, or configuration value,
- side effects such as reading SQLite databases, resolving embedded views, setting global sidebar providers, or registering endpoints,
- validation rules and exceptions,
- whether `null`, empty strings, missing records, missing databases, duplicate matches, or repeated calls have special behavior.

For properties, fields, and constants, document:

- the meaning of the value,
- the default value when meaningful,
- whether consumers may set it directly,
- how it affects routing, query filtering, ordering, sidebar construction, MCP output, or page rendering.

For enums and enum values, document:

- where the enum is used,
- how each value maps to route behavior, query filtering, rendering state, or ordering,
- default or fallback behavior when applicable.

For extension methods, document the receiver type, returned artifact, intended usage pattern, and how it connects to viewing, querying, or rendering behavior.

## Project API documentation requirements

- Controller APIs must document route values, fallback behavior, selected view model, and rendered result.
- MCP APIs must document required and optional parameters, returned text format, empty-result behavior, and query scope.
- Query services must document database paths, read-only behavior, filtering rules, result ordering, and missing-database behavior.
- Sidebar APIs must document host-provided provider behavior and fallback behavior when no provider is configured.
- Configuration APIs must document endpoint registration, default route values, embedded views, and application setup behavior.
- View models and query result models must document the source record represented by each property.
- Security-sensitive APIs must mention HTML rendering, file path, route, SQLite, and source-content rendering risks when consumer-provided values are rendered.

## Examples in XML documentation

Use `<example>` when it materially improves understanding of:

- MCP tool calls,
- documentation route construction,
- query service usage,
- sidebar provider implementation,
- controller integration.

Examples must be short, realistic, and compile-oriented. Prefer C# examples that show viewer setup, query calls, or route values.

## Markdown documentation policy

- Follow DocumentationViewer markdown conventions in:
  - `MARKDOWN_GUIDELINES.md`
- Keep this structure where applicable:
  1. Context
  2. Explanation
  3. Example
  4. Notes / constraints

## Draw.io diagrams for conceptual documentation

Information pages, instruction pages, concept pages, architecture pages, request-flow pages, query-flow pages, and MCP integration pages may use Draw.io diagrams when they clarify a real model or flow.

Draw.io diagrams must follow:

- `DRAWIO_DIAGRAM_RULES.md`

Required baseline:

- save diagrams as enriched `.drawio.svg` files that remain editable in Draw.io,
- store diagrams under `labs_idemobi_com/wwwroot/drawio/{Area}/{diagram-name}.drawio.svg`,
- align shapes and connectors to the Draw.io grid,
- keep diagrams compatible with both light and dark page themes,
- include meaningful alternative text and surrounding explanatory text when rendered in a page,
- start from `labs_idemobi_com/wwwroot/drawio/_templates/concept-flow-template.drawio.svg` when a simple concept-flow template is appropriate.

Do not use Draw.io diagrams in XML documentation comments. XML documentation may reference concepts that are diagrammed on pages, but the diagram artifact belongs to the website documentation layer.

## DocumentationViewer-first rule

Documentation in this module must be authored with a **DocumentationViewer-first** objective.

- Write docs so they can be extracted and rendered without manual rewrite.
- Keep headings deterministic and stable.
- Keep examples self-contained and realistically useful.
- Avoid implicit references to chat history or hidden context.
- Prefer stable type and member names that DocumentationViewer can cross-reference.
- Use `<see cref="..."/>` and `<seealso cref="..."/>` for related viewer, query, sidebar, model, and MCP types whenever it improves navigation.

## Separation from examples and tutorials

`EXAMPLES_AND_TUTORIALS_RULES.md` is not a general documentation rule source.

- Use this file for API documentation, XML HeaderDoc, README updates, reference pages, and DocumentationViewer-ready documentation.
- Use `MARKDOWN_GUIDELINES.md` for general Markdown formatting rules.
- Use `EXAMPLES_AND_TUTORIALS_RULES.md` only when the task explicitly creates or updates example pages, demo pages, tutorials, or tutorial-like walkthroughs.
- Do not import example-page requirements into XML documentation or reference documentation unless the task also changes examples or tutorials.

### Target publication project

- `../../../labs_idemobi_com` (from `DMBDocumentationBuilder/Source/Viewer`)

### Execution responsibility

- AI prepares documentation content, structure, and metadata.
- The developer executes generation or verification commands when needed.
- AI must not claim build/test, database generation, or DocumentationBuilder execution unless it was actually run.

## Minimum update policy

If public routing behavior, query behavior, MCP output, sidebar behavior, model shape, embedded view behavior, or configuration behavior changes, update in the same change set:

- local `README.md`,
- relevant XML docs,
- impacted guidance/examples.

If a new user-facing explanation, instruction, concept, example, or tutorial page is added, apply `EXAMPLES_AND_TUTORIALS_RULES.md` to that work.

## Review checklist for documentation changes

- The documentation names the real DocumentationViewer concept, not a copied source project concept.
- All public and protected-contract API members touched by the change have valid XML documentation.
- Summaries are specific enough to help IntelliSense users choose the right API.
- Parameters, return values, generic parameters, exceptions, and side effects are documented where applicable.
- Examples reflect current code behavior and realistic DocumentationViewer usage.
- Draw.io diagrams, when added, follow `DRAWIO_DIAGRAM_RULES.md`.
- DocumentationViewer can extract the content without needing hidden context or manual rewrite.
