#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBDocumentationBuilder;
using NUnit.Framework;

#endregion

namespace DMBDocumentationBuilderUnitTest;

[TestFixture]
public sealed class DocumentationAIContextOptionExtractorTests
{
    [Test]
    public void ExtractReadsNearestAIContextOptions()
    {
        string rootPath = Path.Combine(Path.GetTempPath(), $"DMBContextOptions_{Guid.NewGuid():N}");
        string sourcePath = Path.Combine(rootPath, "Source");
        string optionsPath = Path.Combine(rootPath, "AIContextOptions");
        Directory.CreateDirectory(sourcePath);
        Directory.CreateDirectory(optionsPath);

        string projectPath = Path.Combine(sourcePath, "DMBTest.csproj");
        File.WriteAllText(projectPath, "<Project Sdk=\"Microsoft.NET.Sdk\" />");
        File.WriteAllText(
            Path.Combine(optionsPath, "configuration.json"),
            """
            {
              "title": "Configuration rule",
              "description": "Configuration description.",
              "scenarioName": "Configuration",
              "projectStyles": [ "website" ],
              "tags": [ "configuration" ],
              "sortOrder": 10,
              "contextText": "Use the documented configuration lifecycle."
            }
            """);

        try
        {
            DocumentationAIContextOptionModel model = DocumentationAIContextOptionExtractor.Extract(
                new DocumentationProjectDescriptor
                {
                    DisplayName = "DMBTest",
                    ProjectFilePath = projectPath,
                    PackageId = "DMBTest",
                    Version = "1.2.3"
                },
                "TestGroup");

            Assert.Multiple(() =>
            {
                Assert.That(model.GroupName, Is.EqualTo("TestGroup"));
                Assert.That(model.PackageId, Is.EqualTo("DMBTest"));
                Assert.That(model.Version, Is.EqualTo("1.2.3"));
                Assert.That(model.Files.Count, Is.EqualTo(1));
                Assert.That(model.Files[0].RuleName, Is.EqualTo("configuration"));
                Assert.That(model.Files[0].Title, Is.EqualTo("Configuration rule"));
                Assert.That(model.Files[0].ProjectStyles, Is.EquivalentTo(new[] { "website" }));
                Assert.That(model.Files[0].ContextText, Does.Contain("configuration lifecycle"));
            });
        }
        finally
        {
            Directory.Delete(rootPath, recursive: true);
        }
    }
}
