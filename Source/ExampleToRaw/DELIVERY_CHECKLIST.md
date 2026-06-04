# DMBExampleToRaw Delivery Checklist

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBExampleToRaw`
- Project root folder: `DMBDocumentationBuilder/Source/ExampleToRaw`
- Main role: reusable raw example generation package.
- Documentation target: `labs_idemobi_com`

## Before handoff

- Confirm public classes, methods, properties, and enums have English XML documentation.
- Confirm package mechanics do not contain website-specific paths.
- Confirm output file names remain compatible with `Examples_Raw` consumers.
- Confirm `DMBExampleToRaw.csproj` includes package metadata, signing, icon, README, and NuGet packing settings.
- Confirm `DMBExampleToRaw.snk`, `DMBExampleToRaw.png`, `README.md`, `AGENTS.md`, and `AI_CONTEXT.md` exist beside the `.csproj`.
- Confirm the full AI/documentation context set exists beside the `.csproj`.
- Confirm the project is registered in `PageBuilder.slnx` when it belongs to this solution.
- Confirm package distribution configuration includes this project when it must be published.
- Confirm documentation generation configuration includes this project when it must appear in generated docs.
- State explicitly whether build, test, restore, format, raw example generation, or DocumentationBuilder execution was skipped.

## Manual verification when requested

- Generate raw examples from a small known source directory.
- Verify `@` is escaped as `@@`.
- Verify `<`, `>`, and `&` are encoded.
- Verify relative folder structure is preserved.
- Verify generated files use `_Raw.cshtml`.
