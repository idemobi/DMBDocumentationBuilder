# Getting Started With Documentation Coverage

This tutorial validates that Markdown tutorials can live next to API reference pages.

## Goal

Use this page to confirm that DocumentationBuilder can:

- discover a top-level Markdown tutorial;
- render the Markdown content as HTML;
- store the rendered page in SQLite with package and version metadata;
- expose the page through the DocumentationViewer sidebar.

## Example

The generated route keeps the same package identity as the API reference:

```text
groupName=Documentation coverage
packageId=DMBDocumentationTest
version=0.1
```

This makes the tutorial follow the same version navigation rules as documented classes and namespaces.
