#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBDocumentationViewer;
using NUnit.Framework;

#endregion

namespace DMBDocumentationViewerUnitTest;

[TestFixture]
public sealed class DocumentationMcpTextFormatterTests
{
    private static DocumentationSearchResultItem CreateResultItem()
    {
        return new DocumentationSearchResultItem
        {
            NamespaceName = "DMB.Sample",
            ObjectName = "Widget",
            ObjectType = "class",
            PackageId = "Package",
            Version = "1.0.0",
            RoutePath = "/docs/widget"
        };
    }

    [Test]
    public void FormatDocumentationIncludesMetadataKeywordsAndLimitedHtmlContent()
    {
        DocumentationQueryResult result = new()
        {
            Id = 42,
            PackageId = "DMBDocumentationViewer",
            Version = "1.2.3",
            NamespaceName = "DMB.Sample",
            ObjectName = "Widget",
            ObjectType = "class",
            RoutePath = "/docs/widget",
            Builder = "builder",
            TechnicalKeywords = "class method",
            Keywords = "widget sample",
            HtmlContent = "0123456789"
        };

        string text = DocumentationMcpTextFormatter.FormatDocumentation(result);

        Assert.Multiple(() =>
        {
            Assert.That(text, Does.Contain("Documentation found for DMB.Sample.Widget (class)"));
            Assert.That(text, Does.Contain("Id: 42"));
            Assert.That(text, Does.Contain("Package: DMBDocumentationViewer"));
            Assert.That(text, Does.Contain("Technical keywords:"));
            Assert.That(text, Does.Contain("class method"));
            Assert.That(text, Does.Contain("Keywords:"));
            Assert.That(text, Does.Contain("widget sample"));
            Assert.That(text, Does.Contain("HTML content:"));
            Assert.That(text, Does.Contain("0123456789"));
        });
    }

    [Test]
    public void FormatDocumentationReturnsNotFoundMessageForNullResult()
    {
        string text = DocumentationMcpTextFormatter.FormatDocumentation(null);

        Assert.That(text, Is.EqualTo("Documentation object not found."));
    }

    [Test]
    public void FormatDocumentationSearchReportsEmptyAndPopulatedResults()
    {
        DocumentationSearchResultItem item = CreateResultItem();

        string emptyText = DocumentationMcpTextFormatter.FormatDocumentationSearch("missing", []);
        string resultText = DocumentationMcpTextFormatter.FormatDocumentationSearch("widget", [item]);

        Assert.Multiple(() =>
        {
            Assert.That(emptyText, Is.EqualTo("No documentation result found for 'missing'."));
            Assert.That(resultText, Does.Contain("1 result(s) found for 'widget':"));
            Assert.That(resultText, Does.Contain("- DMB.Sample.Widget (class) [Package 1.0.0]"));
            Assert.That(resultText, Does.Contain("/docs/widget"));
        });
    }

    [Test]
    public void FormatNamespaceObjectsAndRelatedObjectsUseSpecificEmptyMessages()
    {
        DocumentationSearchResultItem item = CreateResultItem();

        string namespaceEmptyText = DocumentationMcpTextFormatter.FormatNamespaceObjects("DMB.Empty", []);
        string namespaceText = DocumentationMcpTextFormatter.FormatNamespaceObjects("DMB.Sample", [item]);
        string relatedEmptyText = DocumentationMcpTextFormatter.FormatRelatedObjects("Missing", []);
        string relatedText = DocumentationMcpTextFormatter.FormatRelatedObjects("Widget", [item]);

        Assert.Multiple(() =>
        {
            Assert.That(namespaceEmptyText, Is.EqualTo("No documented object found in namespace 'DMB.Empty'."));
            Assert.That(namespaceText, Does.Contain("Namespace 'DMB.Sample' contains 1 object(s):"));
            Assert.That(namespaceText, Does.Contain("- Widget (class) [Package 1.0.0]"));
            Assert.That(relatedEmptyText, Is.EqualTo("No related object found for 'Missing'."));
            Assert.That(relatedText, Does.Contain("Related object(s) found for 'Widget':"));
            Assert.That(relatedText, Does.Contain("- DMB.Sample.Widget (class) [Package 1.0.0]"));
        });
    }

    [Test]
    public void LimitTextReturnsEmptyForBlankAndAddsMarkerWhenTruncated()
    {
        Assert.Multiple(() =>
        {
            Assert.That(DocumentationMcpTextFormatter.LimitText("  ", 5), Is.Empty);
            Assert.That(DocumentationMcpTextFormatter.LimitText("abcdef", 3), Is.EqualTo("abc" + Environment.NewLine + Environment.NewLine + "[truncated]"));
            Assert.That(DocumentationMcpTextFormatter.LimitText("abc", 3), Is.EqualTo("abc"));
        });
    }
}