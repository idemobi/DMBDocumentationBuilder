# AI Rules - DMBDocumentationImprovementByAI

## Scope

- Applies to `DMBDocumentationBuilder/Source/ImprovementByAI` and descendants.
- This project is a console tool for improving generated documentation database content with local or remote AI runtimes.

## Intent

- Improve documentation summaries, short summaries, keywords, and project context stored in generated documentation databases.
- Keep generated text precise, technical, and useful for developers and AI assistants.
- Preserve the documentation database schema unless a task explicitly requires a migration.

## Key Constraints

- Query the DMBFrameworks MCP before changing DocumentationBuilder, DocumentationViewer, MCP, or generated documentation behavior.
- Do not add a new AI provider, database format, or orchestration model without explicit approval.
- Keep prompts deterministic and focused on architectural role, responsibility, usage, and integration context.
- Avoid marketing language in generated documentation guidance.
- Do not hard-code new repository-specific paths; prefer explicit options when adding reusable behavior.
- Do not run `dotnet build`, `dotnet test`, `dotnet restore`, or `dotnet format` unless explicitly requested.

## Documentation

- Write documentation and XML documentation in English.
- Follow local `MARKDOWN_GUIDELINES.md` and `LOCALIZATION_NOMENCLATURE.md` when editing documentation or localizable text.
- Public APIs must have useful XML documentation with `<see cref="..."/>` references where relevant.

## Before Delivery

- State whether prompts, database update behavior, or provider runtime behavior changed.
- Mention database generation or AI runtime execution only when it was actually run.
