#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Collections.Generic;
using DMBDocumentationBuilder;
using NUnit.Framework;

#endregion

namespace DMBDocumentationBuilderUnitTest;

[TestFixture]
public sealed class DocumentationKeywordExtractorTests
{
    [Test]
    public void ExtractKeywordsAsStringReturnsSpaceSeparatedKeywords()
    {
        string result = DocumentationKeywordExtractor.ExtractKeywordsAsString("Gamma beta alpha beta");

        Assert.That(result, Is.EqualTo("alpha beta gamma"));
    }

    [Test]
    public void ExtractKeywordsRemovesHtmlDecodesTextFiltersUselessWordsAndOrdersDistinctValues()
    {
        const string html = "<div>Search search &amp; DocumentationBuilder API <span>class</span> la the</div>";

        IReadOnlyList<string> keywords = DocumentationKeywordExtractor.ExtractKeywords(html);

        Assert.That(keywords, Is.EqualTo(new[] { "api", "documentationbuilder", "search" }));
    }

    [Test]
    public void ExtractKeywordsReturnsEmptyListForBlankContent()
    {
        IReadOnlyList<string> keywords = DocumentationKeywordExtractor.ExtractKeywords("  ");

        Assert.That(keywords, Is.Empty);
    }
}