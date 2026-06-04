# DMBExampleToRaw Examples and Tutorials Rules

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBExampleToRaw`
- Project root folder: `DMBDocumentationBuilder/Source/ExampleToRaw`
- Example focus: prebuild integration and generated raw-code partials.
- Documentation target: `labs_idemobi_com`

## Example rules

- Use realistic paths in examples, but avoid hardcoding repository-only paths in reusable package API examples.
- Show `DMBExampleToRawOptions` as the integration surface.
- Include source and target directories when useful.
- Mention `_Raw.cshtml` naming and preserved folder structure.
- Keep examples deterministic and easy to copy into a prebuild orchestrator.

## Tutorial rules

- Start from source example partials and finish with generated raw partials in a website `Views/Shared/Examples_Raw` folder.
- Explain where the call belongs in `PreBuilding` or future `PrepareWebsite` orchestration.
- Do not instruct readers to modify package internals for website-specific path changes.
- State any manual verification steps, such as checking escaped Razor syntax.
