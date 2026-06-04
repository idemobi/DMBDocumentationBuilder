# DMBExampleToRaw

`DMBExampleToRaw` generates escaped raw-code Razor partials from source example partials.

The package contains the generation mechanics only. Website-specific paths are passed through `DMBExampleToRawOptions` by an orchestrator such as `PreBuilding` or a future `PrepareWebsite` project.

## Usage

```csharp
using DMBExampleToRaw;

new DMBExampleToRawAgent().GenerateRawFiles(new DMBExampleToRawOptions
{
    SourceDirectoryPath = "/path/to/Views/Shared/Examples",
    TargetDirectoryPath = "/path/to/Views/Shared/Examples_Raw"
});
```

## Generated files

- Source files are read from `SourceDirectoryPath` using `SourceSearchPattern`.
- Generated files preserve the relative source folder structure.
- Generated file names use the `_Raw.cshtml` suffix.
- Razor `@` characters are escaped as `@@`.
- HTML-sensitive characters are encoded for display inside code blocks.
