# Sidebar Integration

This tutorial validates the second documentation level supported by Markdown content scanning.

## Sidebar Shape

The page should appear under:

- DMBDocumentationTest;
- Tutorials;
- Advanced;
- Sidebar Integration.

## Reader Workflow

A reader should be able to switch between API reference pages and tutorial pages without losing the current documentation package context.

```csharp
DocumentationLauncher.Run("../../../../labs_idemobi_com", groups);
```

The prebuild updates the sidebar entries for the generated package version while preserving older versions already stored in the database.
