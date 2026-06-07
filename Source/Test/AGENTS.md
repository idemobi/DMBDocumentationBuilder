# AI Rules - DMBDocumentationTest

## Scope

- Applies to `DMBDocumentationBuilder/Source/Test` and descendants.
- This project is a coverage sample for DocumentationBuilder and DocumentationViewer extraction scenarios.

## Intent

- Provide stable sample C# APIs, XML documentation, Markdown tutorials, release notes, and OpenAPI input for documentation extraction tests.
- Keep examples broad enough to exercise classes, records, structs, enums, interfaces, generics, attributes, extension methods, controllers, and OpenAPI routes.
- Treat odd-looking APIs as possible coverage fixtures before simplifying them.

## Key Constraints

- Query the DMBFrameworks MCP before changing DocumentationBuilder, DocumentationViewer, MCP, or documentation extraction behavior.
- Preserve fixture diversity unless the task explicitly changes coverage expectations.
- Do not remove sample members only because they look artificial; they may cover parser or renderer behavior.
- Keep public APIs backward compatible unless a test fixture change explicitly requires a rename or removal.
- Do not run `dotnet build`, `dotnet test`, `dotnet restore`, or `dotnet format` unless explicitly requested.

## Documentation

- Write documentation and XML documentation in English.
- Follow local `MARKDOWN_GUIDELINES.md` and `LOCALIZATION_NOMENCLATURE.md`.
- Public sample APIs must keep useful XML documentation because this project validates documentation extraction.

## Before Delivery

- State which fixture area changed: C# coverage, Markdown documentation, OpenAPI sample, or project metadata.
- Mention build, test, restore, format, or documentation extraction only when it was actually run.
