# DMBExampleToRaw Draw.io Diagram Rules

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBExampleToRaw`
- Project root folder: `DMBDocumentationBuilder/Source/ExampleToRaw`
- Diagram topics: raw example generation flow, prebuild integration flow, and package boundary diagrams.
- Documentation target: `labs_idemobi_com`

## Rules

- Use editable `.drawio.svg` files when adding diagrams for documentation pages.
- Store website documentation diagrams under `labs_idemobi_com/wwwroot/drawio/ExampleToRaw/`.
- Keep diagrams focused on package boundaries, inputs, generated outputs, escaping, and orchestrator responsibility.
- Do not use diagrams as a substitute for XML documentation or README integration notes.
- Keep labels in English.

## Suggested diagrams

- Source examples to generated raw partials.
- `PrepareWebsite` or `PreBuilding` orchestration calling `DMBExampleToRaw`.
- Package boundary showing that website paths are owned by the orchestrator.
