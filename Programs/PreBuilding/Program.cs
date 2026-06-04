#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBDocumentationBuilder;

#endregion

string moduleRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
string websiteRoot = Path.Combine(moduleRoot, "Website");

List<DocumentationGroupDescriptor> groups =
[
    new()
    {
        GroupName = "NuGet",
        Projects =
        [
            new()
            {
                ProjectFilePath = Path.Combine(moduleRoot, "Source", "Builder", "DMBDocumentationBuilder.csproj"),
                DisplayName = "DMBDocumentationBuilder"
            },
            new()
            {
                ProjectFilePath = Path.Combine(moduleRoot, "Source", "Viewer", "DMBDocumentationViewer.csproj"),
                DisplayName = "DMBDocumentationViewer"
            },
            new()
            {
                ProjectFilePath = Path.Combine(moduleRoot, "Source", "ImprovementByAI", "DMBDocumentationImprovementByAI.csproj"),
                DisplayName = "DMBDocumentationImprovementByAI"
            },
            new()
            {
                ProjectFilePath = Path.Combine(moduleRoot, "Source", "ExampleToRaw", "DMBExampleToRaw.csproj"),
                DisplayName = "DMBExampleToRaw"
            }
        ]
    },
    new()
    {
        GroupName = "Documentation coverage",
        Projects =
        [
            new()
            {
                ProjectFilePath = Path.Combine(moduleRoot, "Source", "Test", "DMBDocumentationTest.csproj"),
                DisplayName = "DMBDocumentationTest",
                MarkdownContents =
                {
                    new()
                    {
                        RootDirectoryPath = Path.Combine(moduleRoot, "Source", "Test", "Documentation", "Tutorials"),
                        SectionTitle = "Tutorials",
                        ObjectType = "Tutorial",
                        Icon = "bi-journal-text"
                    },
                    new()
                    {
                        RootDirectoryPath = Path.Combine(moduleRoot, "Source", "Test", "Documentation", "ReleaseNotes"),
                        SectionTitle = "Release Notes",
                        ObjectType = "ReleaseNote",
                        Icon = "bi-megaphone"
                    }
                },
                OpenApiDocuments =
                {
                    new()
                    {
                        JsonFilePath = Path.Combine(moduleRoot, "Source", "Test", "OpenApi", "documentation-test-openapi.json"),
                        DocumentName = "documentation-test",
                        SectionTitle = "REST API",
                        Icon = "bi-hdd-network"
                    }
                }
            }
        ]
    }
];

DocumentationLauncher.Run(websiteRoot, groups);
