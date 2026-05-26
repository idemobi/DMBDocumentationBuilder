# DMBDocumentationBuilder Examples and Tutorials Rules

## Objective

Define how example pages, demo pages, and tutorials are created for `DMBDocumentationBuilder` and related DocumentationBuilder ecosystem packages.

These rules apply only when the task explicitly creates or updates:

- example pages,
- generated-artifact demo pages,
- tutorial pages,
- walkthrough pages,
- example partials rendered through `RenderExamplePartialAsync`.

Do not use this file as the rule source for XML API documentation or reference documentation. Use `DOCUMENTATION_RULES.md` for that work.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBDocumentationBuilder`
- Default documentation area: `DocumentationBuilder`
- Publication target: `../labs_idemobi_com`
- Shared UI stack: `DMBBootstrapBuilder`, `DMBComponentBuilder`, `DMBFormBuilder`, and `DMBDocumentationBuilder`
- Default DocumentationViewer package id for this project: `DMBDocumentationBuilder`
- Default DocumentationViewer namespace for this project: `DMBDocumentationBuilder`

## Publication target

Examples and tutorials must be written in `../labs_idemobi_com`.

Use the existing MVC conventions in that project:

- controller actions in `labs_idemobi_com/Controllers`,
- full pages in `labs_idemobi_com/Views/{FeatureOrComponent}/`,
- reusable example partials in `labs_idemobi_com/Views/Shared/Examples/`,
- generated raw-code mirrors in `labs_idemobi_com/Views/Shared/Examples_Raw/`.

AI may create or update source example partials under `Views/Shared/Examples/`. The developer or prebuild step is responsible for regenerating `Views/Shared/Examples_Raw/` when required.

## Shared UI stack

Example and tutorial pages must use the existing DocumentationBuilder ecosystem components instead of ad-hoc layout markup when a suitable component exists.

Prefer:

- `DMBBootstrapBuilder` for layout, titles, cards, rows, columns, alerts, badges, buttons, tables, tabs, and Bootstrap-oriented UI.
- `DMBComponentBuilder` for reusable non-form documentation generators available in the project.
- `DMBFormBuilder` for forms, fields, validation examples, and form-state demonstrations.
- `DMBDocumentationBuilder` for documentation generation concepts, project descriptors, generated output examples, XML comment rendering, and navigation/search metadata concepts.

Do not introduce a new frontend framework or independent demo system for examples.

## Page categories

There are two distinct page formats:

- general information pages,
- generated artifact or feature pages.

Do not merge the two formats unless the user explicitly asks for a hybrid page.

## General information page format

Use this format for package introductions, architecture pages, extraction pipeline pages, rendering pipeline pages, getting started pages, conceptual guides, and tutorials that are not focused on one generated artifact or feature.

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
- should not include generated artifact galleries unless the page is explicitly an overview gallery,
- should avoid long inline API listings that belong in DocumentationViewer.

### Draw.io diagrams on information and tutorial pages

Information pages, instruction pages, concept pages, architecture pages, rendering pipeline pages, and tutorials may include Draw.io diagrams when a visual model clarifies the explanation.

Draw.io diagrams must follow `DRAWIO_DIAGRAM_RULES.md`.

In particular:

- use enriched `.drawio.svg` files that remain editable in Draw.io,
- store diagrams under `labs_idemobi_com/wwwroot/drawio/{Area}/{diagram-name}.drawio.svg`,
- keep geometry aligned to the Draw.io grid,
- keep diagrams compatible with light and dark page themes,
- include meaningful alternative text when rendering the diagram in a page,
- use `labs_idemobi_com/wwwroot/drawio/_templates/concept-flow-template.drawio.svg` as the starting template when useful.

Do not use Draw.io diagrams as decoration on generated artifact pages. Add a diagram only when it explains a real extraction flow, rendering flow, persistence model, navigation model, or integration concern.

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

## Generated Artifact Page Format

Use this format for a page dedicated to one generated documentation artifact, one generator feature, or one tightly scoped DocumentationBuilder page family.

Every generated artifact page must follow the same high-level order:

1. Title.
2. Explanation of the generated artifact or feature.
3. DocumentationViewer button linking to the relevant API documentation.
4. Gallery of examples rendered with `@await Html.RenderExamplePartialAsync(...)`.
5. Showcase list.

### Title

Use `TitleBuilder` for the main title.

The title should name the generated artifact or feature family clearly. Prefer the public class name when the page is class-specific, for example ``DocumentationLauncher`, `DocumentationXmlCommentRenderer`, or `DocumentationDatabaseManager`.

### Artifact explanation

The explanation must be short and practical.

It should cover:

- what the generator extracts or renders,
- the primary use case,
- the most important project descriptor or generation inputs,
- any important path, link, sidebar, SQLite, or output stability concern.

Do not duplicate the full API reference on the generated artifact page.

### DocumentationViewer button

Every generated artifact page must include a visible button linking to the generated API documentation.

Use the existing DocumentationViewer route through the `Documentation` controller:

```csharp
@Url.Action("Show", "Documentation", new
{
    groupName = "NuGet",
    packageId = "DMBDocumentationBuilder",
    version = "0.9",
    namespaceName = "DMBDocumentationBuilder",
    objectName = "DocumentationLauncher"
})
```

Adjust `packageId`, `namespaceName`, and `objectName` to the generated artifact or API being documented.

Use a Bootstrap-compatible button style and an icon such as `bi-journal-code`.

When rendering the DocumentationViewer button in a generated artifact page, prefer an `ActionItem` implementation rendered through `ButtonRender` instead of a handwritten `<a class="btn ...">` element.

### Example gallery

The gallery must render example partials through:

```csharp
@await Html.RenderExamplePartialAsync("Examples/{GeneratorOrFeatureName}/{ExamplePartialName}", "Readable example title", model: Model)
```

The source partials must live under:

```text
labs_idemobi_com/Views/Shared/Examples/{GeneratorOrFeatureName}/{ExamplePartialName}.cshtml
```

The generated raw-code mirrors are expected under:

```text
labs_idemobi_com/Views/Shared/Examples_Raw/{GeneratorOrFeatureName}/{ExamplePartialName}_Raw.cshtml
```

Use the public generator type or feature name as the folder name for new generated artifact pages. For example:

- `Views/Shared/Examples/DocumentationLauncher/_B001BasicGeneration.cshtml`
- `Views/Shared/Examples/DocumentationLauncher/_B002CleanOutput.cshtml`
- `Views/Shared/Examples/DocumentationXmlCommentRenderer/_B001SummaryAndRemarks.cshtml`
- `Views/Shared/Examples/DocumentationDatabaseManager/_B001SavedObjectMetadata.cshtml`

Existing legacy folders may be migrated gradually, but new component work must use the class-name folder convention.

### Minimum generated artifact example coverage

Every generated artifact page must include at least:

- normal state,
- empty state,
- error or invalid state,
- one realistic data example.

Add more examples when the generated artifact has important generation modes, empty inputs, missing XML docs, project reference modes, persistence behavior, or generated navigation behavior.

### Showcase list

After the gallery, include a showcase list that demonstrates the generated artifact surface beyond the basic examples.

The showcase may be:

- a table of variants,
- a list of common compositions,
- grouped cards,
- tabs by feature family,
- form state matrix,
- visual state matrix.

The showcase must remain readable and should not replace the example gallery. The gallery shows copyable usage; the showcase helps compare available states.

## Tutorial expectations

Use tutorials for step-by-step workflows, not for simple component reference pages.

Tutorials must:

- explain prerequisites and assumptions,
- provide step-by-step usage,
- state the expected result for each step,
- include common mistakes and recovery guidance,
- link to the relevant generated artifact pages or DocumentationViewer API pages.

## Naming and maintainability

- Keep page, controller, and folder names aligned with the public generator type or feature name.
- Prefer clear names over abbreviations.
- Keep examples self-contained and realistic.
- Keep example partials small enough to be readable in the rendered code panel.
- Keep repeated page structure consistent across features so future artifacts can be added mechanically.
- Do not leave placeholder pages such as `NEED TO SHOW EXAMPLES`.

## Delivery checklist for examples

Before finishing an example or tutorial task, verify:

- the page is under `labs_idemobi_com`,
- the page uses existing BootstrapBuilder, ComponentBuilder, FormBuilder, or DocumentationBuilder components where appropriate,
- generated artifact pages follow the required generated artifact page format,
- example partials live under `Views/Shared/Examples/{GeneratorOrFeatureName}/`,
- `RenderExamplePartialAsync` paths match the source partial paths,
- the DocumentationViewer button targets the correct package, namespace, and object,
- normal, empty, error, and realistic generation states are covered for generated artifact pages,
- Draw.io diagrams, when added, follow `DRAWIO_DIAGRAM_RULES.md`,
- any raw example generation that was not run is explicitly reported.
