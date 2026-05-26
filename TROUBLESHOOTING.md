# DMBDocumentationBuilder Troubleshooting

## Project-specific section

When copying this file to another DocumentationBuilder ecosystem project, update this section first.

- Project name: `DMBDocumentationBuilder`
- Main troubleshooting areas: Roslyn compilation inputs, XML documentation extraction, generated paths, generated HTML, sidebar source, SQLite metadata, project references, and host project integration.
- Resource folder: `Resources/`
- Documentation target: `labs_idemobi_com`

## Localization key fallback text appears

### Symptoms

- UI displays token-like fallback text instead of translated labels.

### Checks

1. Confirm key exists in `Resources/*.resx`.
2. Confirm key naming follows `LOCALIZATION_NOMENCLATURE.md`.
3. Confirm the expected localizer context is used.

## Page metadata does not appear in the rendered output

### Checks

1. Confirm the target `.csproj` path is correct and source files are included by the selected compile item mode.
2. Confirm XML documentation comments are available in the documented project source.
3. Confirm metadata keys are not overwritten later in the request.

## Fluent builder output is missing expected attributes or classes

### Checks

1. Confirm the fluent method sets, replaces, or removes the value as expected.
2. Confirm conditional builder calls are executed before rendering.
3. Confirm custom class composition does not override the expected class.

## Script or stylesheet is missing or duplicated

### Checks

1. Verify the generated route path, package id, version, namespace, object name, and object type stored in SQLite.
2. Verify asset ordering and location rules.
3. Verify the layout renders the corresponding asset region.

## Documentation output quality is weak

### Checks

1. Confirm docs follow `DOCUMENTATION_RULES.md` structure.
2. Confirm examples are self-contained and realistic.
3. Confirm headings are deterministic for extraction.
4. Confirm audience needs (developers + AI) are addressed.
