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
public sealed class DocumentationPathHelperTests
{
    [Test]
    public void NamespaceToFolderConvertsAllNamespaceSeparatorsToPlatformSeparator()
    {
        string result = DocumentationPathHelper.NamespaceToFolder(@"Company.Product/Feature.Area");

        Assert.That(result, Is.EqualTo(string.Join(
            Path.DirectorySeparatorChar,
            "Company",
            "Product",
            "Feature",
            "Area")));
    }

    [Test]
    public void NamespaceToPathUsesForwardSlashForNamespaceDots()
    {
        string result = DocumentationPathHelper.NamespaceToPath("Company.Product.Feature");

        Assert.That(result, Is.EqualTo("Company/Product/Feature"));
    }

    [Test]
    public void ToSafeNameReplacesInvalidFileNameCharacters()
    {
        char invalidCharacter = Path.GetInvalidFileNameChars().First(c => c != '\0');

        string result = DocumentationPathHelper.ToSafeName($"Before{invalidCharacter}After");

        Assert.That(result, Is.EqualTo("Before_After"));
    }
}