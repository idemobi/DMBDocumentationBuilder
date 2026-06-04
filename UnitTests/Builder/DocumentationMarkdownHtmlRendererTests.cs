#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using DMBDocumentationBuilder;
using NUnit.Framework;

#endregion

namespace DMBDocumentationBuilderUnitTest;

[TestFixture]
public sealed class DocumentationMarkdownHtmlRendererTests
{
    [Test]
    public void RenderClosesAnUnterminatedCodeBlock()
    {
        const string markdown = """
                                ```json
                                { "name": "value" }
                                """;

        string html = DocumentationMarkdownHtmlRenderer.Render(markdown);

        Assert.That(html, Does.EndWith("</code></pre>" + Environment.NewLine));
    }

    [Test]
    public void RenderConvertsSupportedMarkdownBlocksAndInlineFormatting()
    {
        const string markdown = """
                                # Title <unsafe>

                                Paragraph with **bold**, *italic*, `code`, and [link](https://example.com?q=1&v=2).

                                - First item
                                - Second item

                                ```csharp
                                <tag>
                                ```
                                """;

        string html = DocumentationMarkdownHtmlRenderer.Render(markdown);

        Assert.Multiple(() =>
        {
            Assert.That(html, Does.Contain("<h1>Title &lt;unsafe&gt;</h1>"));
            Assert.That(html, Does.Contain("<p>Paragraph with <strong>bold</strong>, <em>italic</em>, <code>code</code>, and <a href=\"https://example.com?q=1&amp;v=2\">link</a>.</p>"));
            Assert.That(html, Does.Contain("<ul>"));
            Assert.That(html, Does.Contain("<li>First item</li>"));
            Assert.That(html, Does.Contain("<pre class=\"documentation-markdown-code\"><code class=\"language-csharp\">"));
            Assert.That(html, Does.Contain("&lt;tag&gt;"));
        });
    }
}