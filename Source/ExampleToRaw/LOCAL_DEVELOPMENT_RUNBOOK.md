# DMBExampleToRaw Local Development Runbook

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBExampleToRaw`
- Project root folder: `DMBDocumentationBuilder/Source/ExampleToRaw`
- Main role: reusable raw example generation package.
- Primary local consumer: `PreBuilding`

## Local workflow

1. Inspect the requested change and identify whether it affects public options, output names, output folder structure, escaping, encoding, wrappers, or packaging metadata.
2. Keep website-specific path changes in the orchestrator project.
3. Keep reusable generation behavior in `DMBExampleToRaw`.
4. Update XML documentation for touched public API.
5. Update README and context files when integration behavior changes.
6. Do not run `dotnet build`, `dotnet test`, `dotnet restore`, `dotnet format`, or raw example generation unless explicitly requested.

## Manual generation when explicitly requested

- Run the prebuild orchestrator only when the user asks for generation.
- Inspect generated `_Raw.cshtml` files after generation.
- Report generated files and any skipped checks.

## Handoff notes

- State whether commands were skipped because repository instructions disallow them by default.
- State whether raw examples were generated or only code/documentation was changed.
