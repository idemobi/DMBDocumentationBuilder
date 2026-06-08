# DMBDocumentationImprovementByAI

## Context

`DMBDocumentationImprovementByAI` enriches generated DocumentationBuilder SQLite databases with AI summaries, short summaries, and keywords.

The generated AI databases are consumed by `DMBDocumentationViewer` through the `DocumentationAIRenderSources` table stored in the main documentation database.

## Explanation

AI runtimes select `LatestVersion` documentation objects by default. This matches the viewer behavior where omitted route versions resolve to the latest generated documentation version.

The `All` selection mode is still available for maintenance runs that intentionally process historical versions.

Generated AI result rows store both the `DocumentationObjectId` and the versioned object identity:

```text
PackageId, Version, NamespaceName, ObjectName, ObjectType
```

`DMBDocumentationViewer` uses these identity columns when they exist, so stale AI rows are not displayed for a different versioned documentation object.

## Example

```csharp
LMStudioRuntime.Run(new LMStudioOptions
{
    DatabasePath = "Documentation/data.db",
    ApiToken = Environment.GetEnvironmentVariable("AI_RESUME_LMSTUDIO_API_TOKEN") ?? string.Empty,
    ObjectSelectionMode = DocumentationAIObjectSelectionMode.LatestVersion,
    MaxObjectsToProcess = 60
});
```

## Notes

- Existing AI databases are migrated automatically when a runtime opens them.
- Existing AI databases without identity columns remain readable for backward compatibility.
- Run the normal documentation prebuild before running AI improvement so the main documentation database contains current objects.
