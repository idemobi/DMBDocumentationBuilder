#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Generates rendered Markdown documentation pages into the documentation database.
    /// </summary>
    public static class DocumentationMarkdownPageManager
    {
        #region Static methods

        private static string BuildKeywords(string markdown, DocumentationMarkdownContentItem item)
        {
            string compactMarkdown = markdown
                .Replace('\r', ' ')
                .Replace('\n', ' ');

            return $"{item.Title} {item.SectionTitle} {item.FolderTitle} {compactMarkdown}".Trim();
        }

        private static string BuildRoutePath(
            string groupName,
            string packageId,
            string version,
            string sectionTitle,
            string slug
        )
        {
            return "/DocumentationContent/ShowContent?groupName=" + WebUtility.UrlEncode(groupName)
                                                                  + "&packageId=" + WebUtility.UrlEncode(packageId)
                                                                  + "&version=" + WebUtility.UrlEncode(version)
                                                                  + "&namespaceName=" + WebUtility.UrlEncode(sectionTitle)
                                                                  + "&objectName=" + WebUtility.UrlEncode(slug);
        }

        private static string BuildTechnicalKeywords(
            string groupName,
            DocumentationProjectDescriptor project,
            DocumentationMarkdownContentItem item
        )
        {
            return $"{groupName} {project.PackageId} {project.Version} {item.ObjectType} {item.SectionTitle} {item.FolderTitle} {item.Title} {item.Slug}".Trim();
        }

        /// <summary>
        ///     Generates configured Markdown pages for the supplied documentation groups.
        /// </summary>
        /// <param name="groups">The documentation groups whose projects may contain Markdown folders.</param>
        /// <param name="sqliteDatabasePath">The SQLite database path that receives rendered Markdown pages.</param>
        public static void Generate(
            IEnumerable<DocumentationGroupDescriptor> groups,
            string sqliteDatabasePath
        )
        {
            foreach (DocumentationGroupDescriptor group in groups)
            {
                foreach (DocumentationProjectDescriptor project in group.Projects)
                {
                    foreach (DocumentationMarkdownContentItem item in DocumentationMarkdownContentScanner.Scan(project))
                    {
                        string markdown = File.ReadAllText(item.SourceFilePath);
                        string html = RenderPageHtml(item, DocumentationMarkdownHtmlRenderer.Render(markdown));
                        string routePath = BuildRoutePath(group.GroupName, project.PackageId, project.Version, item.SectionTitle, item.Slug);

                        DocumentationDatabaseManager.SaveObject(
                            sqliteDatabasePath,
                            project.PackageId,
                            project.Version,
                            item.SectionTitle,
                            item.Slug,
                            item.ObjectType,
                            item,
                            html,
                            BuildTechnicalKeywords(group.GroupName, project, item),
                            BuildKeywords(markdown, item),
                            routePath);
                    }
                }
            }
        }

        private static string Html(string? value)
        {
            return WebUtility.HtmlEncode(value ?? string.Empty);
        }

        private static string RemoveDuplicateTitleHeading(DocumentationMarkdownContentItem item, string bodyHtml)
        {
            string titleHeading = $"<h1>{Html(item.Title)}</h1>";
            string trimmedBodyHtml = bodyHtml.TrimStart();

            if (!trimmedBodyHtml.StartsWith(titleHeading, StringComparison.Ordinal))
            {
                return bodyHtml;
            }

            return trimmedBodyHtml[titleHeading.Length..].TrimStart('\r', '\n');
        }

        private static string RenderPageHtml(DocumentationMarkdownContentItem item, string bodyHtml)
        {
            StringBuilder html = new();
            string normalizedBodyHtml = RemoveDuplicateTitleHeading(item, bodyHtml);

            html.AppendLine("<div class=\"ex-container py-3 ex-py-lg-5 documentation-markdown-page\">");
            html.AppendLine("    <div class=\"row g-3\">");
            html.AppendLine("        <div class=\"col-12\">");
            html.AppendLine("            <div class=\"mb-4\">");
            html.AppendLine("                <div class=\"d-flex flex-wrap align-items-center gap-2 mb-3\">");
            html.Append("                    <span class=\"badge text-bg-primary rounded-pill px-2 py-1\">")
                .Append(Html(item.SectionTitle))
                .AppendLine("</span>");

            if (!string.IsNullOrWhiteSpace(item.FolderTitle))
            {
                html.Append("                    <span class=\"badge text-bg-secondary rounded-pill px-2 py-1\">")
                    .Append(Html(item.FolderTitle))
                    .AppendLine("</span>");
            }

            html.AppendLine("                </div>");
            html.Append("                <h1>")
                .Append(Html(item.Title))
                .AppendLine("</h1>");
            html.AppendLine("            </div>");
            html.AppendLine("            <article class=\"documentation-markdown-body\">");
            html.AppendLine(normalizedBodyHtml);
            html.AppendLine("            </article>");
            html.AppendLine("        </div>");
            html.AppendLine("    </div>");
            html.AppendLine("</div>");

            return html.ToString();
        }

        #endregion
    }
}