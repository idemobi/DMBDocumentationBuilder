# DMBDocumentationBuilder Documentation Rules

## Language

- Documentation must be written in English.
- XML documentation comments must be written in English.

## Target audience

- Primary: developers maintaining or integrating `DMBDocumentationBuilder`.
- Secondary: developers generating API documentation for PageBuilder ecosystem packages.
- Tertiary: AI assistants consuming structured project rules and technical context.

Documentation must be useful without private chat context. A reader should understand what the API extracts, which generated artifact it produces, how Roslyn symbols and XML comments flow through the system, and what stability constraints apply before reading the implementation.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBDocumentationBuilder`
- Primary API families: launchers, extraction managers, page managers, page renderers, page models, member item models, XML comment rendering, keyword extraction, path helpers, sidebar generation, SQLite persistence, and type registry helpers.
- Important types to reference when relevant: `DocumentationLauncher`, `DocumentationExtractionManager`, `DocumentationProjectDescriptor`, `DocumentationProjectItem`, `DocumentationXmlCommentRenderer`, `DocumentationXmlModel`, `DocumentationDatabaseManager`, `DocumentationPathHelper`, and renderer or manager types for classes, records, structs, interfaces, and enums.
- Publication host: `labs_idemobi_com`
- Documentation generation strategy: DocumentationBuilder-first; AI prepares content, the developer executes generation.

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
- Also write XML HeaderDoc for protected members when they are part of an inheritance or rendering contract.
- Internal and private members do not require XML HeaderDoc unless they explain complex extraction, rendering, path, link, keyword, or persistence behavior that would otherwise be difficult to maintain.
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

- the type's role in extraction, modeling, rendering, indexing, persistence, or launch orchestration,
- the generated artifact or metadata it produces when applicable,
- the relationship with important types such as `DocumentationLauncher`, `DocumentationExtractionManager`, `DocumentationXmlCommentRenderer`, page managers, page renderers, and page models,
- lifecycle expectations, including whether the type is called directly by the launcher, by a page manager, by a renderer, or by a shared helper.

For methods and constructors, document:

- what the member changes in generated output, extracted metadata, path construction, link resolution, keyword generation, or SQLite persistence,
- every parameter and the expected format when relevant,
- the returned value when the method produces generated content, extracted metadata, or a helper result,
- side effects such as creating directories, writing files, updating SQLite records, reading XML documentation, resolving symbols, or generating links,
- validation rules and exceptions,
- whether `null`, empty strings, duplicate keys, or repeated calls have special behavior.

For properties, fields, and constants, document:

- the meaning of the value,
- the default value when meaningful,
- whether consumers may set it directly,
- how it affects generated output, ordering, deduplication, navigation, search metadata, path stability, or page composition.

For enums and enum values, document:

- where the enum is used,
- how each value maps to extraction mode, project reference handling, generated metadata, behavior, or ordering,
- default or fallback behavior when applicable.

For extension methods, document the receiver type, returned artifact, intended usage pattern, and how it connects to extraction or rendering behavior.

## Project API documentation requirements

- Launcher APIs must document input paths, target output paths, cleanup behavior, and generated artifacts.
- Extraction APIs must document Roslyn compilation inputs, symbol filtering, XML documentation lookup, and project reference handling.
- Page manager APIs must document which type family they generate and which renderer/model they coordinate.
- Renderer APIs must document whether they return HTML strings, write files, generate partials, or update shared navigation artifacts.
- XML documentation APIs must document supported XML tags, HTML encoding behavior, link resolution, and fallback behavior for missing comments.
- Path APIs must document path normalization, generated directory names, anchor stability, and file naming rules.
- SQLite persistence APIs must document tables, keys, upsert behavior, and stored metadata.
- Security-sensitive APIs must mention HTML injection, file path, URL, XML parsing, and source-content rendering risks when consumer-provided values are rendered.

## Examples in XML documentation

Use `<example>` when it materially improves understanding of:

- launcher entry points,
- project descriptor setup,
- XML comment rendering,
- path construction,
- page manager generation calls,
- SQLite metadata persistence.

Examples must be short, realistic, and compile-oriented. Prefer C# examples that show generation setup or helper usage.

## Markdown documentation policy

- Follow DocumentationBuilder markdown conventions in:
  - `../MARKDOWN_GUIDELINES.md`
- Keep this structure where applicable:
  1. Context
  2. Explanation
  3. Example
  4. Notes / constraints

## Draw.io diagrams for conceptual documentation

Information pages, instruction pages, concept pages, architecture pages, extraction pipeline pages, and rendering pipeline pages may use Draw.io diagrams when they clarify a real model or flow.

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

## DocumentationBuilder-first rule

Documentation in this module must be authored with a **DocumentationBuilder-first** objective.

- Write docs so they can be extracted and rendered without manual rewrite.
- Keep headings deterministic and stable.
- Keep examples self-contained and realistically useful.
- Avoid implicit references to chat history or hidden context.
- Prefer stable type and member names that DocumentationBuilder can cross-reference.
- Use `<see cref="..."/>` and `<seealso cref="..."/>` for related DocumentationBuilder types whenever it improves navigation.

## Separation from examples and tutorials

`EXAMPLES_AND_TUTORIALS_RULES.md` is not a general documentation rule source.

- Use this file for API documentation, XML HeaderDoc, README updates, reference pages, and DocumentationBuilder-ready documentation.
- Use `../MARKDOWN_GUIDELINES.md` for general Markdown formatting rules.
- Use `EXAMPLES_AND_TUTORIALS_RULES.md` only when the task explicitly creates or updates example pages, demo pages, tutorials, or tutorial-like walkthroughs.
- Do not import example-page requirements into XML documentation or reference documentation unless the task also changes examples or tutorials.

### Target publication project

- `../labs_idemobi_com` (from DocumentationBuilder root)

### Execution responsibility

- AI prepares documentation content, structure, and metadata.
- The developer executes DocumentationBuilder.
- AI must not claim DocumentationBuilder execution unless it was actually run.

### Personalization policy

When DocumentationBuilder offers customization hooks, use them only to improve:

- clarity,
- discoverability (keywords/meta),
- structure quality,
- examples/tutorial quality.

Customizations must remain:

- technically accurate,
- aligned with current code behavior,
- consistent with naming and localization conventions.

## Minimum update policy

If public extraction behavior, rendering behavior, documentation metadata behavior, path behavior, sidebar behavior, or persistence behavior changes, update in the same change set:

- local `README.md`,
- relevant XML docs,
- impacted guidance/examples.

If a new generated page family or user-facing documentation artifact is added, update or add an explanation/example page as a separate examples task and then apply `EXAMPLES_AND_TUTORIALS_RULES.md` to that examples work.

## Review checklist for documentation changes

- The documentation names the real DocumentationBuilder concept, not a copied source project concept.
- All public and protected-contract API members touched by the change have valid XML documentation.
- Summaries are specific enough to help IntelliSense users choose the right API.
- Parameters, return values, generic parameters, exceptions, and side effects are documented where applicable.
- Examples reflect current code behavior and realistic DocumentationBuilder usage.
- Draw.io diagrams, when added, follow `DRAWIO_DIAGRAM_RULES.md`.
- DocumentationBuilder can extract the content without needing hidden context or manual rewrite.
