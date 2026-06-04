# DMBExampleToRaw Project Map

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBExampleToRaw`
- Project root folder: `DMBDocumentationBuilder/Source/ExampleToRaw`
- Main role: reusable raw example generation package.
- Important files: `DMBExampleToRawAgent.cs`, `DMBExampleToRawOptions.cs`, `DMBExampleToRaw.csproj`, and `README.md`.
- Documentation target: `labs_idemobi_com`

## Folder responsibilities

- Root files
  - `DMBExampleToRawAgent.cs`: generation agent that scans source examples and writes raw-code partials.
  - `DMBExampleToRawOptions.cs`: public options model for source paths, target paths, wrappers, and search pattern.
  - `DMBExampleToRaw.csproj`: package metadata, signing, package icon, README, and NuGet settings.
  - `DMBExampleToRaw.png`: NuGet package icon.
  - `DMBExampleToRaw.snk`: package signing key.
  - `README.md`: package overview and usage example.

- `bin/` and `obj/`
  - Build outputs and intermediate files. Do not use these folders as documentation or source-of-truth inputs.

## Documentation-related files

- `AGENTS.md`: local AI rules and scope for this package.
- `AI_CONTEXT.md`: additional context for AI-assisted maintenance.
- `DOCUMENTATION_RULES.md`: strict documentation policy.
- `DRAWIO_DIAGRAM_RULES.md`: rules for editable Draw.io diagrams.
- `EXAMPLES_AND_TUTORIALS_RULES.md`: rules for example pages and tutorials.
- `DELIVERY_CHECKLIST.md`: final quality gate before handoff.
- `ARCHITECTURE_DECISIONS.md`: local architecture decisions and constraints.
- `GLOSSARY.md`: shared vocabulary for this package.
- `LOCAL_DEVELOPMENT_RUNBOOK.md`: local workflow notes and handoff checks.
- `LOCALIZATION_NOMENCLATURE.md`: localization key naming rules for this package.
- `TROUBLESHOOTING.md`: known issues and recovery notes.
