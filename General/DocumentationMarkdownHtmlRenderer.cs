#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationMarkdownHtmlRenderer.cs create at 2026/05/18 22:05:00
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using System.Net;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Renders DocumentationBuilder Markdown content into safe HTML.
    /// </summary>
    public static class DocumentationMarkdownHtmlRenderer
    {
        #region Static fields and properties

        private static readonly Regex BoldRegex = new(@"\*\*(.+?)\*\*", RegexOptions.Compiled);
        private static readonly Regex CodeRegex = new(@"`([^`]+)`", RegexOptions.Compiled);
        private static readonly Regex ItalicRegex = new(@"(?<!\*)\*([^*]+)\*(?!\*)", RegexOptions.Compiled);
        private static readonly Regex LinkRegex = new(@"\[([^\]]+)\]\(([^)]+)\)", RegexOptions.Compiled);

        #endregion

        #region Static methods

        /// <summary>
        /// Renders Markdown text into the HTML subset used by generated documentation pages.
        /// </summary>
        /// <param name="markdown">The Markdown text to render.</param>
        /// <returns>The rendered HTML content.</returns>
        public static string Render(string markdown)
        {
            StringBuilder html = new();
            string[] lines = markdown.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            bool inCodeBlock = false;
            bool inList = false;
            string codeLanguage = string.Empty;
            List<string> paragraph = [];

            void FlushParagraph()
            {
                if (paragraph.Count == 0)
                {
                    return;
                }

                html.Append("<p>")
                    .Append(RenderInline(string.Join(" ", paragraph)))
                    .AppendLine("</p>");
                paragraph.Clear();
            }

            void CloseList()
            {
                if (!inList)
                {
                    return;
                }

                html.AppendLine("</ul>");
                inList = false;
            }

            foreach (string rawLine in lines)
            {
                string line = rawLine.TrimEnd();
                string trimmed = line.Trim();

                if (trimmed.StartsWith("```", StringComparison.Ordinal))
                {
                    FlushParagraph();
                    CloseList();

                    if (inCodeBlock)
                    {
                        html.AppendLine("</code></pre>");
                        inCodeBlock = false;
                        codeLanguage = string.Empty;
                    }
                    else
                    {
                        codeLanguage = trimmed[3..].Trim();
                        html.Append("<pre class=\"documentation-markdown-code\"><code");

                        if (!string.IsNullOrWhiteSpace(codeLanguage))
                        {
                            html.Append(" class=\"language-")
                                .Append(Html(codeLanguage))
                                .Append('"');
                        }

                        html.AppendLine(">");
                        inCodeBlock = true;
                    }

                    continue;
                }

                if (inCodeBlock)
                {
                    html.AppendLine(Html(rawLine));
                    continue;
                }

                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    FlushParagraph();
                    CloseList();
                    continue;
                }

                int headingLevel = GetHeadingLevel(trimmed);

                if (headingLevel > 0)
                {
                    FlushParagraph();
                    CloseList();

                    string heading = trimmed[(headingLevel + 1)..].Trim();
                    html.Append("<h")
                        .Append(headingLevel)
                        .Append('>')
                        .Append(RenderInline(heading))
                        .Append("</h")
                        .Append(headingLevel)
                        .AppendLine(">");
                    continue;
                }

                if (trimmed.StartsWith("- ", StringComparison.Ordinal) ||
                    trimmed.StartsWith("* ", StringComparison.Ordinal))
                {
                    FlushParagraph();

                    if (!inList)
                    {
                        html.AppendLine("<ul>");
                        inList = true;
                    }

                    html.Append("<li>")
                        .Append(RenderInline(trimmed[2..].Trim()))
                        .AppendLine("</li>");
                    continue;
                }

                paragraph.Add(trimmed);
            }

            FlushParagraph();
            CloseList();

            if (inCodeBlock)
            {
                html.AppendLine("</code></pre>");
            }

            return html.ToString();
        }

        private static int GetHeadingLevel(string line)
        {
            int count = 0;

            while (count < line.Length && line[count] == '#')
            {
                count++;
            }

            return count is > 0 and <= 6 &&
                   count < line.Length &&
                   line[count] == ' '
                ? count
                : 0;
        }

        private static string Html(string? value)
        {
            return WebUtility.HtmlEncode(value ?? string.Empty);
        }

        private static string RenderInline(string value)
        {
            string encoded = Html(value);

            encoded = LinkRegex.Replace(
                encoded,
                match => $"<a href=\"{match.Groups[2].Value}\">{match.Groups[1].Value}</a>");
            encoded = CodeRegex.Replace(encoded, "<code>$1</code>");
            encoded = BoldRegex.Replace(encoded, "<strong>$1</strong>");
            encoded = ItalicRegex.Replace(encoded, "<em>$1</em>");

            return encoded;
        }

        #endregion
    }
}
