# DMBExampleToRaw Glossary

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBExampleToRaw`
- Project root folder: `DMBDocumentationBuilder/Source/ExampleToRaw`
- Main role: raw example generation package.

## Terms

- Source example: Razor partial that renders a real documentation or demo example.
- Raw example: generated Razor partial that displays escaped source code.
- Raw target directory: directory that receives generated raw partials.
- `_Raw.cshtml`: generated file suffix used for raw-code mirror partials.
- Wrapper: HTML fragment placed before and after escaped source code.
- HTML encoding: escaping `&`, `<`, and `>` so source code can be displayed safely.
- Razor escaping: replacing `@` with `@@` so generated raw partials render the literal Razor source.
- Orchestrator: executable project such as `PreBuilding` or future `PrepareWebsite` that supplies paths and calls the package.
- Package mechanics: reusable generation logic owned by `DMBExampleToRaw`.
