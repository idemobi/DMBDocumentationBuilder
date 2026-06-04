# DMBExampleToRaw Documentation Rules

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBExampleToRaw`
- Project root folder: `DMBDocumentationBuilder/Source/ExampleToRaw`
- Main role: raw example generation package.
- Documentation target: `labs_idemobi_com`

## XML documentation

- Write XML documentation in English.
- Document every public class, public method, public property, public enum, and public enum value.
- Use `<see cref="..."/>` for references to package types such as `DMBExampleToRawAgent` and `DMBExampleToRawOptions`.
- Use `<param>`, `<returns>`, `<remarks>`, and `<exception>` where they add useful information.
- Keep comments focused on behavior, compatibility, inputs, outputs, and failure modes.

## README and reference documentation

- Explain source directory requirements.
- Explain generated file naming and folder mapping.
- Explain escaping and encoding rules.
- Explain how prebuild orchestrators should pass paths through options.
- Avoid repository-specific assumptions in reusable package documentation.
- Mention when generation has not been run.

## DocumentationBuilder posture

- AI prepares documentation content and structure.
- The developer runs DocumentationBuilder unless explicitly asking Codex to do it.
- Do not claim generated documentation was updated unless the generation command was actually run.
