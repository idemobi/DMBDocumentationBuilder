#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.IO;
using DMBDocumentationImprovementByAI;
using NUnit.Framework;

#endregion

namespace DMBDocumentationImprovementByAIUnitTest;

[TestFixture]
public sealed class ProjectFileHelperTests
{
    private static string CreateTemporaryProjectFile()
    {
        string directory = Path.Combine(Path.GetTempPath(), "dmb-documentation-ai-unit-test", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string projectFilePath = Path.Combine(directory, "Sample.csproj");
        File.WriteAllText(projectFilePath, "<Project Sdk=\"Microsoft.NET.Sdk\"><ItemGroup /></Project>");
        return projectFilePath;
    }

    [Test]
    public void DisabledOperationsReturnWithoutValidatingThePath()
    {
        Assert.Multiple(() =>
        {
            Assert.DoesNotThrow(() => ProjectFileHelper.EnsureDocumentationDatabasesCopyAlways(string.Empty, false));
            Assert.DoesNotThrow(() => ProjectFileHelper.RemoveDocumentationDatabasesCopyAlways(string.Empty, false));
        });
    }

    [Test]
    public void EnsureDocumentationDatabasesCopyAlwaysAddsExpectedNoneEntries()
    {
        string projectFilePath = CreateTemporaryProjectFile();

        ProjectFileHelper.EnsureDocumentationDatabasesCopyAlways(projectFilePath, true);

        string content = File.ReadAllText(projectFilePath);

        Assert.Multiple(() =>
        {
            Assert.That(content, Does.Contain("Update=\"Documentation\\data.db\""));
            Assert.That(content, Does.Contain("Update=\"Documentation\\*.db\""));
            Assert.That(content, Does.Contain("Update=\"Documentation\\**\\*.db\""));
            Assert.That(content, Does.Contain("<CopyToOutputDirectory>Always</CopyToOutputDirectory>"));
        });
    }

    [Test]
    public void RemoveDocumentationDatabasesCopyAlwaysRemovesGeneratedEntries()
    {
        string projectFilePath = CreateTemporaryProjectFile();

        ProjectFileHelper.EnsureDocumentationDatabasesCopyAlways(projectFilePath, true);
        ProjectFileHelper.RemoveDocumentationDatabasesCopyAlways(projectFilePath, true);

        string content = File.ReadAllText(projectFilePath);

        Assert.Multiple(() =>
        {
            Assert.That(content, Does.Not.Contain("Documentation\\data.db"));
            Assert.That(content, Does.Not.Contain("Documentation\\*.db"));
            Assert.That(content, Does.Not.Contain("Documentation\\**\\*.db"));
        });
    }
}