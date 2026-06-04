# AI Rules - DMBExampleToRaw

## Scope

- Applies to the `DMBDocumentationBuilder/Source/ExampleToRaw` folder and descendants.
- This project is autonomous enough to be reused by PageBuilder and other GDF repositories.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBExampleToRaw`
- Project folder: `DMBDocumentationBuilder/Source/ExampleToRaw`
- Project role: reusable source-example to raw-code partial generator.
- Primary consumers: `PreBuilding`, future `PrepareWebsite` executables, and GDF website prebuild pipelines.
- Publication host: `labs_idemobi_com`
- Documentation generation strategy: DocumentationBuilder-first; AI prepares content, the developer executes generation.

## Module intent

- Generate deterministic raw-code mirrors from source example partials.
- Keep website-specific paths outside the package and pass them through explicit options.
- Preserve the generated `_Raw.cshtml` naming convention consumed by website demo helpers.

## Key constraints

- Keep public APIs backward compatible unless a change request explicitly allows breakage.
- Prefer additive options over hardcoded website assumptions.
- Treat source and output paths as untrusted inputs.
- Keep generated file naming compatible with `Examples_Raw` consumers.
- Do not run `dotnet build`, `dotnet test`, `dotnet restore`, or `dotnet format` unless explicitly requested.
- Do not run raw example generation unless explicitly requested.

## Documentation objective

- Documentation must be authored so it can be extracted and rendered by DocumentationBuilder.
- Publication target is `labs_idemobi_com`.
- Documentation output must serve both developers and AI assistants.
- AI prepares documentation content and structure; the developer runs DocumentationBuilder.
- XML documentation comments must be written in English.
- Public classes, public methods, public properties, public constants, public enums, public enum values, and other public members must have useful XML documentation.
- README and integration documentation must explain required input paths, generated outputs, and expected prebuild usage.

## Local rule sources

- Use [DOCUMENTATION_RULES.md](DOCUMENTATION_RULES.md) for XML HeaderDoc, README/reference documentation, and DocumentationBuilder-ready documentation.
- Use [EXAMPLES_AND_TUTORIALS_RULES.md](EXAMPLES_AND_TUTORIALS_RULES.md) only when creating or updating example, demo, information, instruction, concept, or tutorial pages.
- Use [DRAWIO_DIAGRAM_RULES.md](DRAWIO_DIAGRAM_RULES.md) when adding editable Draw.io diagrams to information, instruction, concept, architecture, generation-flow, example, or tutorial pages.
- Use [PROJECT_MAP.md](PROJECT_MAP.md) to orient file ownership and documentation responsibilities.
- Use [ARCHITECTURE_DECISIONS.md](ARCHITECTURE_DECISIONS.md) for local package boundaries and compatibility decisions.
- Use [DELIVERY_CHECKLIST.md](DELIVERY_CHECKLIST.md) before handoff.
- Use [TROUBLESHOOTING.md](TROUBLESHOOTING.md) for known issue notes.

## Localization

- Follow local [LOCALIZATION_NOMENCLATURE.md](LOCALIZATION_NOMENCLATURE.md).
- Do not assume external localization rules unless duplicated here.

## Before delivery

- Update local docs when behavior or public options change.
- State untested areas explicitly.
- Do not claim build/test, restore, format, raw example generation, or DocumentationBuilder execution when they were not run.
