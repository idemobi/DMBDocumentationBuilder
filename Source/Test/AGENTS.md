# Codex Instructions

## General

- Follow the existing architecture and naming conventions.
- Do not introduce new frameworks without explicit approval.
- Prefer minimal, localized changes.
- Keep public APIs backward compatible unless explicitly requested.

## .NET / C#

- Do not run `dotnet test`, `dotnet build`, `dotnet restore`, or `dotnet format` unless explicitly requested.
- Do not modify `.csproj` files unless the task requires it.
- Use English for XML documentation comments.
- Always write XML HeaderDoc for public classes, public methods, public properties, and public enums.
- Use `<see cref="..."/>` references where relevant.
- Prefer explicit types for public API clarity.

## Visual components

- Every new visual component must include a test/demo page.
- The test/demo page must cover the normal state, empty state, error state, and at least one realistic data example.
- Do not leave visual components without a manual preview route/page.

## Naming conventions

- Use `GDF` prefix for Game-Data-Forge framework types.
- Use `Agent` for backend components, not `Service`, unless matching ASP.NET infrastructure.
- Use clear domain names: `PlayerData`, `StudioData`, `Organization`, `Role`, `Project`, etc.
- Avoid abbreviations unless already established in the codebase.

## Documentation

- Documentation must be written in English.
- XML documentation must use valid C# XML doc syntax.
- Prefer:
  - `<summary>`
  - `<param>`
  - `<returns>`
  - `<remarks>`
  - `<exception>`
  - `<see cref="..."/>`
  - `<seealso cref="..."/>`

## Documentation

- All documentation must follow `MARKDOWN_GUIDELINES.md`
- Use the appropriate template:
  - General documentation → `DOC_PAGE_TEMPLATE.md`
  - Tutorials → `TUTORIAL_TEMPLATE.md`
  - Examples → `EXAMPLE_TEMPLATE.md`
- Do not invent new structures
- Keep documentation consistent across the project

## Package extraction checklist

When extracting a reusable tool into a DMB package project, keep the new project aligned with the existing package layout:

- Create the package folder at the solution root, next to the other `.csproj` folders.
- Add a package `.csproj` with package metadata, `PackageId`, `PackageIcon`, `PackageReadmeFile`, XML documentation output, `SignAssembly`, `PublicSign`, and `AssemblyOriginatorKeyFile`.
- Add the project `.snk`, package icon `.png`, `README.md`, `AGENTS.md`, and `AI_CONTEXT.md` beside the `.csproj`.
- Add the full AI/documentation context set beside the `.csproj`: `ARCHITECTURE_DECISIONS.md`, `DELIVERY_CHECKLIST.md`, `DOCUMENTATION_RULES.md`, `DRAWIO_DIAGRAM_RULES.md`, `EXAMPLES_AND_TUTORIALS_RULES.md`, `GLOSSARY.md`, `LOCALIZATION_NOMENCLATURE.md`, `LOCAL_DEVELOPMENT_RUNBOOK.md`, `PROJECT_MAP.md`, and `TROUBLESHOOTING.md`.
- Keep reusable package mechanics free of repository-specific paths such as `labs_idemobi_com` and `AppContext.BaseDirectory`; pass paths through explicit options.
- Add the project to `PageBuilder.slnx`, the distribution project list, and documentation generation configuration when it is a published package.
- Remove dependencies from orchestrator projects when they move into the extracted package.

## Localization rules

- Follow `LOCALIZATION_NOMENCLATURE.md` for all localization key creation.
- Do not create ad-hoc localization key names.
- Keep key tokens uppercase and separated by `_`.
- Reuse existing key families before adding new ones (`VIEW`, `ACTIONITEM`, `FORM`, `TABLE`, `EXCEPTION`, `ALERT`, `DATAANNOTATION`, etc.).
- Optional `cshtml` pages in `Views/Shared` may use raw localization calls directly; still enforce the same key nomenclature.
- For `_BeforeContent.cshtml` and `_AfterContent.cshtml`, keep keys in the host view family, do not create isolated naming styles.

## Before finishing

- Summarize changed files.
- Mention anything that could not be tested.
- Do not claim tests were run if they were not run.
