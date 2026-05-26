# DMBDocumentationBuilder Glossary

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBDocumentationBuilder`
- Project role: documentation extraction and generated API documentation package.
- Main vocabulary domain: Roslyn symbols, XML comments, generated pages, navigation metadata, and SQLite persistence.

## DMBDocumentationBuilder

Package that turns C# project source and XML documentation comments into generated API documentation pages and metadata.

## DocumentationLauncher

Public orchestration entry point used to clean generated artifacts and run documentation generation for one or more groups.

## Documentation Group

Named collection of projects rendered together in generated navigation.

## Project Descriptor

Configuration object that identifies a `.csproj`, display name, package id, version, compile item mode, and project reference mode.

## Page Manager

Coordinator that extracts models for one page family and invokes the matching renderer.

## Page Renderer

Renderer that turns an extracted model into generated HTML or `.cshtml` output.

## XML Comment Model

Structured representation of XML documentation sections such as summary, remarks, example, parameters, returns, value, exceptions, and see-also links.

## Type Registry

Lookup structure used to decide whether a Roslyn type symbol is part of the generated documentation set.

## Sidebar Source

Generated C# source file that exposes documentation navigation to the host website.

## Documentation Database

SQLite database storing generated object metadata, route paths, technical keywords, rendered content, and source snippets.
