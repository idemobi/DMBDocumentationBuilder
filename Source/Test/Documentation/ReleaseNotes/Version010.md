# Version 0.1.0 Release Notes

This release note validates Markdown release-note rendering in DocumentationViewer.

## Included Coverage

- Markdown files are rendered during prebuild.
- Rendered HTML is stored in the documentation SQLite database.
- Release notes appear in the documentation sidebar.
- The page is served from the database, not from the filesystem.

## Compatibility

Release notes use the same `PackageId` and `Version` metadata as the generated API reference.
