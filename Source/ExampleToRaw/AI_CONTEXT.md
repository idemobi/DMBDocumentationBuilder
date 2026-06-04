# DMBExampleToRaw AI Context

## Context

`DMBExampleToRaw` is the reusable raw-example generation package extracted from `PreBuilding`.

It should be treated as prebuild infrastructure: changes can affect generated demo source displays, Razor escaping, HTML encoding, file naming, and compatibility with website helpers that load `Examples_Raw` partials.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBExampleToRaw`
- Project role: source example partial to escaped raw-code partial generator.
- Primary consumers: `PreBuilding`, future `PrepareWebsite` executables, and other GDF website repositories.
- Main documentation target: DocumentationBuilder output rendered in `labs_idemobi_com`.
- Main source inputs: source examples directory, target raw examples directory, search pattern, and raw code wrapper fragments.
- Main outputs: `_Raw.cshtml` partials preserving the source relative folder structure.

## Important behavior

- Source files are read as UTF-8.
- Generated files are written as UTF-8.
- CRLF line endings are normalized to LF before trimming.
- `&`, `<`, and `>` are encoded for HTML display.
- Razor `@` characters are escaped as `@@`.
- The package must not know about `labs_idemobi_com` or `AppContext.BaseDirectory`.
- Website-specific orchestration belongs in `PreBuilding` or future `PrepareWebsite` projects.

## Maintenance posture

1. Identify whether a change affects output file names, folder structure, escaping, encoding, wrappers, or path handling.
2. Preserve generated partial compatibility unless the user explicitly asks for a new raw-code strategy.
3. Update XML documentation for every touched public member.
4. Update README or integration documentation when public integration behavior changes.
5. Do not run build, test, restore, format, raw example generation, or DocumentationBuilder commands unless explicitly requested.

## Documentation strategy for AI

- Produce extraction-ready docs for DocumentationBuilder.
- Explain source directory requirements, generated outputs, escaping rules, and prebuild integration.
- Use `<see cref="..."/>` references for related raw generator types when useful.
- State untested areas and skipped generation steps explicitly.
