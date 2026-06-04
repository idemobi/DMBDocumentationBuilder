# DMBDocumentationViewer Glossary

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBDocumentationViewer`
- Project role: runtime documentation viewing and generated API documentation query package.
- Main vocabulary domain: MVC routes, generated SQLite records, documentation objects, sidebar navigation, MCP tools, source snippets, and project context.

## DMBDocumentationViewer

Package that displays generated API documentation and exposes read-only query tools for documentation, source code, and project context.

## Documentation Object

A generated documentation record identified by package id, version, namespace, object name, and object type.

## Documentation Database

SQLite database produced by the documentation generation pipeline and consumed by the viewer at runtime.

## Documentation Route

MVC route that identifies a documentation group, namespace, object, package id, version, or object type.

## Documentation Query Service

Read-only service that retrieves documentation records, search results, namespace objects, and related objects from the generated database.

## Source Code Query Service

Read-only service that retrieves generated source-code snippets associated with a documented object.

## Project Context Query Service

Read-only service that retrieves generated project-context files and context search results for a package version.

## MCP Tool

AI-facing method exposed through Model Context Protocol so an assistant can search documentation and retrieve related context.

## MCP Text Formatter

Helper that converts query results into compact text blocks suitable for AI tool responses.

## Sidebar Provider

Host-provided implementation that returns sidebar models for documentation root, group, and namespace pages.

## AI Render Source

Optional model describing the provider, model, and database source used to render AI-generated documentation summaries.
