# DMBExampleToRaw Architecture Decisions

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBExampleToRaw`
- Project root folder: `DMBDocumentationBuilder/Source/ExampleToRaw`
- Main role: reusable raw example generation package for DMB and GDF website prebuild pipelines.
- Important files: `DMBExampleToRawAgent.cs`, `DMBExampleToRawOptions.cs`, `DMBExampleToRaw.csproj`, and `README.md`.
- Documentation target: `labs_idemobi_com`

## Decisions

### Keep generation mechanics in a package

`DMBExampleToRaw` owns file scanning, relative output mapping, source escaping, HTML encoding, and raw partial writing.

Orchestrator projects such as `PreBuilding` and future `PrepareWebsite` projects only provide paths and execution order.

### Keep website paths out of the package

The package must not reference `labs_idemobi_com`, repository-relative paths, or `AppContext.BaseDirectory`.

All paths are supplied through `DMBExampleToRawOptions`.

### Preserve raw example naming

Generated files must keep the `_Raw.cshtml` suffix and relative folder structure expected by website helpers.

Breaking changes to names, output locations, or escaping rules require an explicit migration decision.

### Keep wrappers configurable

The default HTML wrapper matches the original PageBuilder demo code block, while options allow another repository to provide its own wrapper without changing package internals.
