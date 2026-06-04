# DMBDocumentationViewer Examples and Tutorials Rules

## Objective

Define how example pages, demo pages, information pages, concept pages, instruction pages, and tutorials are created for `DMBDocumentationViewer` and related PageBuilder ecosystem packages.

These rules apply only when the task explicitly creates or updates:

- information pages,
- instruction pages,
- concept pages,
- example pages,
- viewer feature pages,
- tutorial pages,
- walkthrough pages,
- example partials rendered through `RenderExamplePartialAsync`.

Do not use this file as the rule source for XML API documentation or reference documentation. Use `DOCUMENTATION_RULES.md` for that work.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBDocumentationViewer`
- Default documentation area: `DocumentationViewer`
- Publication target: `../../../labs_idemobi_com`
- Shared UI stack: `DMBBootstrapBuilder`, `DMBComponentBuilder`, `DMBFormBuilder`, and `DMBDocumentationViewer`
- Default DocumentationViewer package id for this project: `DMBDocumentationViewer`
- Default DocumentationViewer namespace for this project: `DMBDocumentationViewer`

## Publication target

Examples and tutorials must be written in `../../../labs_idemobi_com`.

Use the existing MVC conventions in that project:

- controller actions in `labs_idemobi_com/Controllers`,
- full pages in `labs_idemobi_com/Views/{FeatureOrComponent}/`,
- reusable example partials in `labs_idemobi_com/Views/Shared/Examples/`,
- generated raw-code mirrors in `labs_idemobi_com/Views/Shared/Examples_Raw/`.

AI may create or update source example partials under `Views/Shared/Examples/`. The developer or prebuild step is responsible for regenerating `Views/Shared/Examples_Raw/` when required.

## Shared UI stack

Example, tutorial, information, instruction, and concept pages must use the existing PageBuilder ecosystem components instead of ad-hoc layout markup when a suitable component exists.

Prefer:

- `DMBBootstrapBuilder` for layout, titles, cards, rows, columns, alerts, badges, buttons, tables, tabs, and Bootstrap-oriented UI.
- `DMBComponentBuilder` for reusable non-form documentation components available in the project.
- `DMBFormBuilder` for forms, fields, validation examples, and form-state demonstrations.
- `DMBDocumentationViewer` for API documentation links, documentation lookup concepts, sidebar/search concepts, and viewer integration examples.

Do not introduce a new frontend framework or independent demo system for examples.

## Page categories

There are two distinct page formats:

- general information pages,
- viewer feature pages.

Do not merge the two formats unless the user explicitly asks for a hybrid page.

## General information page format

Use this format for package introductions, architecture pages, request-flow pages, query-flow pages, MCP integration pages, getting started pages, conceptual guides, and tutorials that are not focused on one viewer feature.

Required structure:

1. Title.
2. Short context paragraph explaining the topic and audience.
3. Explanation sections with deterministic headings.
4. Practical integration or usage section when relevant.
5. Notes, constraints, or next steps.
6. Links to related documentation pages or API reference when useful.

General information pages:

- may include short code snippets rendered with `CodeBlockBuilder`,
- may include diagrams or structured lists when they clarify architecture, instructions, or concepts,
- should not include feature galleries unless the page is explicitly an overview gallery,
- should avoid long inline API listings that belong in DocumentationViewer.

### Draw.io diagrams on information and tutorial pages

Information pages, instruction pages, concept pages, architecture pages, request-flow pages, query-flow pages, MCP integration pages, and tutorials may include Draw.io diagrams when a visual model clarifies the explanation.

Draw.io diagrams must follow `DRAWIO_DIAGRAM_RULES.md`.

In particular:

- use enriched `.drawio.svg` files that remain editable in Draw.io,
- store diagrams under `labs_idemobi_com/wwwroot/drawio/{Area}/{diagram-name}.drawio.svg`,
- keep geometry aligned to the Draw.io grid,
- keep diagrams compatible with light and dark page themes,
- include meaningful alternative text when rendering the diagram in a page,
- use `labs_idemobi_com/wwwroot/drawio/_templates/concept-flow-template.drawio.svg` as the starting template when useful.

Do not use Draw.io diagrams as decoration on feature pages. Add a diagram only when it explains a real route flow, query flow, persistence model, sidebar model, MCP flow, or integration concern.

### Code examples on information pages

Code examples on general information pages must use `CodeBlockBuilder` or the existing `Html.CodeBlock(...)` helper when available.

Do not write raw `<pre><code>` blocks by hand when `CodeBlockBuilder` can render the example.

Code examples must:

- specify the language,
- have a useful title when the page contains more than one snippet,
- enable copy behavior when that is consistent with existing pages,
- stay focused on the concept explained by the page.

### Links and actions on information pages

Links that behave like page actions must be generated through `ActionItem` implementations and rendered with `ButtonRender` when possible.

Prefer:

- `AspRouteActionItem` or `ActionItemFactory.AspRoute(...)` for MVC route links,
- `UrlActionItem` for external or absolute URLs,
- `ButtonRender` to render action buttons consistently with BootstrapBuilder.

Use plain inline anchors only for natural text links inside paragraphs where a button/action would be visually inappropriate.

## Viewer Feature Page Format

Use this format for a page dedicated to one viewer feature, one integration point, or one tightly scoped DocumentationViewer page family.

Every viewer feature page must follow the same high-level order:

1. Title.
2. Explanation of the viewer feature.
3. DocumentationViewer button linking to the relevant API documentation.
4. Gallery of examples rendered with `@await Html.RenderExamplePartialAsync(...)`.
5. Showcase list.

### Title

Use `TitleBuilder` for the main title.

The title should name the viewer feature clearly. Prefer the public class name when the page is class-specific, for example `DocumentationController`, `DocumentationMcpTools`, `DocumentationQueryService`, or `DocumentationSidebarFactory`.

### Feature explanation

The explanation must be short and practical.

It should cover:

- what the viewer feature does,
- the primary use case,
- the most important route values, query filters, database inputs, or host integration points,
- any important path, sidebar, MCP, SQLite, source-content, or output stability concern.

Do not duplicate the full API reference on the feature page.

### DocumentationViewer button

Every viewer feature page must include a visible button linking to the generated API documentation.

Use the existing DocumentationViewer route through the `Documentation` controller:

```csharp
@Url.Action("Show", "Documentation", new
{
    groupName = "NuGet",
    packageId = "DMBDocumentationViewer",
    version = "0.9",
    namespaceName = "DMBDocumentationViewer.Controllers",
    objectName = "DocumentationController"
})
```

Adjust `packageId`, `namespaceName`, and `objectName` to the viewer feature or API being documented.

Use a Bootstrap-compatible button style and an icon such as `bi-journal-code`.

When rendering the DocumentationViewer button in a viewer feature page, prefer an `ActionItem` implementation rendered through `ButtonRender` instead of a handwritten `<a class="btn ...">` element.

### Example gallery

The gallery must render example partials through:

```csharp
@await Html.RenderExamplePartialAsync("Examples/{FeatureName}/{ExamplePartialName}", "Readable example title", model: Model)
```

The source partials must live under:

```text
labs_idemobi_com/Views/Shared/Examples/{FeatureName}/{ExamplePartialName}.cshtml
```

The generated raw-code mirrors are expected under:

```text
labs_idemobi_com/Views/Shared/Examples_Raw/{FeatureName}/{ExamplePartialName}_Raw.cshtml
```

Use the public viewer type or feature name as the folder name for new feature pages. For example:

- `Views/Shared/Examples/DocumentationController/_B001ShowObjectRoute.cshtml`
- `Views/Shared/Examples/DocumentationMcpTools/_B001SearchDocumentation.cshtml`
- `Views/Shared/Examples/DocumentationQueryService/_B001SearchByPackageAndVersion.cshtml`
- `Views/Shared/Examples/DocumentationSidebarFactory/_B001HostProvidedSidebar.cshtml`

Existing legacy folders may be migrated gradually, but new feature work must use the class-name folder convention.

### Minimum viewer feature example coverage

Every viewer feature page must include at least:

- normal state,
- empty state,
- error or invalid state,
- one realistic data example.

Add more examples when the feature has important route variants, missing database behavior, missing records, sidebar fallback behavior, MCP formatting behavior, source lookup behavior, or project-context behavior.

### Showcase list

After the gallery, include a showcase list that demonstrates the viewer feature surface beyond the basic examples.

The showcase may be:

- a table of route variants,
- a list of common compositions,
- grouped cards,
- tabs by feature family,
- query result matrix,
- MCP response matrix.

The showcase must remain readable and should not replace the example gallery. The gallery shows copyable usage; the showcase helps compare available states.

## Tutorial expectations

Use tutorials for step-by-step workflows, not for simple API reference pages.

Tutorials must:

- explain prerequisites and assumptions,
- provide step-by-step usage,
- state the expected result for each step,
- include common mistakes and recovery guidance,
- link to the relevant feature pages or DocumentationViewer API pages.

## Naming and maintainability

- Keep page, controller, and folder names aligned with the public viewer type or feature name.
- Prefer clear names over abbreviations.
- Keep examples self-contained and realistic.
- Keep example partials small enough to be readable in the rendered code panel.
- Keep repeated page structure consistent across features so future viewer features can be added mechanically.
- Do not leave placeholder pages such as `NEED TO SHOW EXAMPLES`.

## Delivery checklist for examples

Before finishing an example or tutorial task, verify:

- the page is under `labs_idemobi_com`,
- the page uses existing BootstrapBuilder, ComponentBuilder, FormBuilder, or DocumentationViewer components where appropriate,
- viewer feature pages follow the required viewer feature page format,
- example partials live under `Views/Shared/Examples/{FeatureName}/`,
- `RenderExamplePartialAsync` paths match the source partial paths,
- the DocumentationViewer button targets the correct package, namespace, and object,
- normal, empty, error, and realistic states are covered for viewer feature pages,
- Draw.io diagrams, when added, follow `DRAWIO_DIAGRAM_RULES.md`,
- any raw example generation that was not run is explicitly reported.
