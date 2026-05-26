#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationGroupAndNamespacePageRenderer.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using System.Text;

#endregion

namespace DMBDocumentationBuilder
{
    internal static class DocumentationGroupAndNamespacePageRenderer
    {
        #region Static methods

        private static string EscapeCSharp(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private static string Html(string? value)
        {
            return System.Net.WebUtility.HtmlEncode(value ?? string.Empty);
        }

        private static void AppendRouteValue(StringBuilder sb, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            sb.Append("&")
                .Append(key)
                .Append("=")
                .Append(System.Net.WebUtility.UrlEncode(value));
        }

        private static string ReadPartialContent(
            string sharedDocumentationRootDirectory,
            string subFolder,
            string path,
            string partialName
        )
        {
            string filePath = Path.Combine(
                sharedDocumentationRootDirectory,
                subFolder,
                path,
                partialName);

            string result = $@"<!-- Custom documentation slot -->
<!-- subFolder: {subFolder} -->
<!-- path: {path} -->
<!-- partialName: {partialName} -->
<!-- filePath: {filePath} -->
";
            if (File.Exists(filePath))
            {
                result += $"<div class=\"badge rounded-pill d-flex text-bg-info theme-debug-only my-2\">You can edit /Documentation/{subFolder}/{path}/{partialName}</div>";
                result += File.ReadAllText(filePath);
            }
            else
            {
                result += $"<div class=\"badge rounded-pill d-flex text-bg-info theme-debug-only my-2\">You can create and edit /Documentation/{subFolder}/{path}/{partialName}</div>";
                //result += $"<p class=\"text-break\">FILE=&quot;{filePath}&quot;</p><p>mkdir -p &quot;$(dirname &quot;$FILE&quot;)&quot; && touch &quot;$FILE&quot;</p>";
            }

            return result;
        }

        /// <summary>
        /// Renders the HTML content for a documentation group page.
        /// </summary>
        /// <param name="item">The item value used by the documentation generation operation.</param>
        /// <param name="safeGroupName">The safeGroupName value used by the documentation generation operation.</param>
        /// <param name="sharedDocumentationRootDirectory">The sharedDocumentationRootDirectory value used by the documentation generation operation.</param>
        /// <returns>The RenderGroupHtml result produced by DocumentationBuilder generation.</returns>
        public static string RenderGroupHtml(DocumentationGroupPageModel item, string safeGroupName, string sharedDocumentationRootDirectory)
        {
            StringBuilder sb = new();

            sb.AppendLine("<div class=\"ex-container py-3 ex-py-lg-5\">");
            sb.AppendLine("    <div class=\"row g-3\">");
            sb.AppendLine("        <div class=\"col-12\">");
            sb.AppendLine("            <div class=\"mb-4\">");
            sb.AppendLine("                <div class=\"d-flex flex-wrap align-items-center gap-2 mb-3\">");
            sb.AppendLine("                    <span class=\"badge text-bg-primary rounded-pill px-2 py-1\">Group</span>");
            sb.AppendLine("                </div>");

            sb.Append("                <h1 class=\"display-6 fw-bold mb-3\">")
                .Append(Html(item.GroupName))
                .AppendLine("</h1>");

            sb.AppendLine(ReadPartialContent(sharedDocumentationRootDirectory, "Groups", safeGroupName, "_At_Begin_Of_Group.html"));

            sb.AppendLine("                <section id=\"introduction\" class=\"card border-0 shadow-sm mb-3\">");
            sb.AppendLine("                    <div class=\"card-body p-2\">");
            sb.AppendLine("                        <h2>Introduction</h2>");
            sb.AppendLine("                        <p class=\"text-body-secondary mb-0\">This page groups the namespaces associated with this documentation section.</p>");
            sb.AppendLine("                    </div>");
            sb.AppendLine(ReadPartialContent(sharedDocumentationRootDirectory, "Groups", safeGroupName, "_After_Introduction.html"));
            sb.AppendLine("                </section>");

            sb.AppendLine("                <section id=\"namespaces\" class=\"card border-0 shadow-sm\">");
            sb.AppendLine("                    <div class=\"card-body p-2\">");
            sb.AppendLine("                        <h2>Namespaces</h2>");

            List<DocumentationNamespaceObjectLinkItem> namespaceLinks = item.NamespaceLinks.Count > 0
                ? item.NamespaceLinks
                : item.NamespaceNames
                    .Select(namespaceName => new DocumentationNamespaceObjectLinkItem
                    {
                        PackageId = item.PackageId,
                        Version = item.Version,
                        Name = namespaceName,
                        KindLabel = "Namespace"
                    })
                    .ToList();

            if (namespaceLinks.Count == 0)
            {
                sb.AppendLine("                        <p class=\"text-body-secondary mb-0\">No namespaces available.</p>");
            }
            else
            {
                sb.AppendLine("                        <ul class=\"list-group list-group-flush\">");

                foreach (DocumentationNamespaceObjectLinkItem namespaceLink in namespaceLinks
                             .OrderBy(x => x.PackageId, StringComparer.Ordinal)
                             .ThenBy(x => x.Name, StringComparer.Ordinal))
                {
                    sb.AppendLine("                            <li class=\"list-group-item px-0\">");
                    sb.Append("                                <a href=\"/Documentation/ShowNamespace?groupName=")
                        .Append(System.Net.WebUtility.UrlEncode(item.GroupName));
                    AppendRouteValue(sb, "packageId", namespaceLink.PackageId);
                    AppendRouteValue(sb, "version", namespaceLink.Version);
                    AppendRouteValue(sb, "namespaceName", namespaceLink.Name);
                    sb.AppendLine("\">");
                    sb.Append("                                    <code>")
                        .Append(Html(namespaceLink.Name))
                        .AppendLine("</code>");
                    sb.AppendLine("                                </a>");
                    sb.AppendLine("                            </li>");
                }

                sb.AppendLine("                        </ul>");
            }

            sb.AppendLine("                    </div>");
            sb.AppendLine("                </section>");

            sb.AppendLine(ReadPartialContent(sharedDocumentationRootDirectory, "Groups", safeGroupName, "_At_End_Of_Group.html"));

            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");

            sb.AppendLine("    </div>");
            // sb.AppendLine("    <div class=\"mt-5 pt-3 border-top text-center text-muted small\">");
            // sb.Append("        Documentation generated by DMBDocumentationBuilder v")
            //     .Append(DocumentationVisualHelper.DocumentationBuilderVersion)
            //     .AppendLine();
            // sb.AppendLine("    </div>");
            sb.AppendLine("</div>");

            return sb.ToString();
        }

        /// <summary>
        /// Writes generated documentation group pages.
        /// </summary>
        /// <param name="items">The items value used by the documentation generation operation.</param>
        /// <param name="pageOutputDirectory">The pageOutputDirectory value used by the documentation generation operation.</param>
        /// <param name="sharedDocumentationRootDirectory">The sharedDocumentationRootDirectory value used by the documentation generation operation.</param>
        public static void RenderGroupPages(
            IEnumerable<DocumentationGroupPageModel> items,
            string pageOutputDirectory,
            string sharedDocumentationRootDirectory
        )
        {
            foreach (DocumentationGroupPageModel item in items)
            {
                DocumentationContextPartialGenerator.EnsureGroupPartials(
                    sharedDocumentationRootDirectory,
                    item.GroupName);
            }
        }

        /// <summary>
        /// Renders the HTML content for a documentation namespace page.
        /// </summary>
        /// <param name="item">The item value used by the documentation generation operation.</param>
        /// <param name="namespacePath">The namespacePath value used by the documentation generation operation.</param>
        /// <param name="sharedDocumentationRootDirectory">The sharedDocumentationRootDirectory value used by the documentation generation operation.</param>
        /// <returns>The RenderNamespaceHtml result produced by DocumentationBuilder generation.</returns>
        public static string RenderNamespaceHtml(DocumentationNamespacePageModel item, string namespacePath, string sharedDocumentationRootDirectory)
        {
            StringBuilder sb = new();

            sb.AppendLine("<div class=\"ex-container py-3 ex-py-lg-5\">");
            sb.AppendLine("    <div class=\"row g-3\">");
            sb.AppendLine("        <div class=\"col-12\">");
            sb.AppendLine("            <div class=\"mb-4\">");
            sb.AppendLine("                <div class=\"d-flex flex-wrap align-items-center gap-2 mb-3\">");
            sb.AppendLine("                    <span class=\"badge text-bg-primary rounded-pill px-2 py-1\">Namespace</span>");
            sb.Append("                    <code class=\"small\">")
                .Append(Html(item.GroupName))
                .AppendLine("</code>");
            sb.AppendLine("                </div>");

            sb.Append("                <h1 class=\"display-6 fw-bold mb-3\">")
                .Append(Html(item.NamespaceName))
                .AppendLine("</h1>");

            sb.AppendLine(ReadPartialContent(sharedDocumentationRootDirectory, "Namespaces", namespacePath, "_At_Begin_Of_Namespace.html"));

            sb.AppendLine("                <section id=\"introduction\" class=\"card border-0 shadow-sm mb-3\">");
            sb.AppendLine("                    <div class=\"card-body p-2\">");
            sb.AppendLine("                        <h2>Introduction</h2>");
            sb.AppendLine("                        <p class=\"text-body-secondary mb-0\">This page lists the documented objects available in this namespace.</p>");
            sb.AppendLine("                    </div>");
            sb.AppendLine(ReadPartialContent(sharedDocumentationRootDirectory, "Namespaces", namespacePath, "_After_Introduction.html"));
            sb.AppendLine("                </section>");

            RenderObjectSection(sb, item.PackageId, item.Version, item.GroupName, item.NamespaceName, "classes", "Classes", item.Classes);
            sb.AppendLine(ReadPartialContent(sharedDocumentationRootDirectory, "Namespaces", namespacePath, "_After_Classes.html"));
            RenderObjectSection(sb, item.PackageId, item.Version, item.GroupName, item.NamespaceName, "structs", "Structs", item.Structs);
            sb.AppendLine(ReadPartialContent(sharedDocumentationRootDirectory, "Namespaces", namespacePath, "_After_Structs.html"));
            RenderObjectSection(sb, item.PackageId, item.Version, item.GroupName, item.NamespaceName, "records", "Records", item.Records);
            sb.AppendLine(ReadPartialContent(sharedDocumentationRootDirectory, "Namespaces", namespacePath, "_After_Records.html"));
            RenderObjectSection(sb, item.PackageId, item.Version, item.GroupName, item.NamespaceName, "enums", "Enums", item.Enums);
            sb.AppendLine(ReadPartialContent(sharedDocumentationRootDirectory, "Namespaces", namespacePath, "_After_Enums.html"));
            RenderObjectSection(sb, item.PackageId, item.Version, item.GroupName, item.NamespaceName, "interfaces", "Interfaces", item.Interfaces);
            sb.AppendLine(ReadPartialContent(sharedDocumentationRootDirectory, "Namespaces", namespacePath, "_After_Interfaces.html"));

            sb.AppendLine(ReadPartialContent(sharedDocumentationRootDirectory, "Namespaces", namespacePath, "_At_End_Of_Namespace.html"));

            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");

            sb.AppendLine("    </div>");
            // sb.AppendLine("    <div class=\"mt-5 pt-3 border-top text-center text-muted small\">");
            // sb.Append("        Documentation generated by DMBDocumentationBuilder v")
            //     .Append(DocumentationVisualHelper.DocumentationBuilderVersion)
            //     .AppendLine();
            // sb.AppendLine("    </div>");
            sb.AppendLine("</div>");

            return sb.ToString();
        }

        /// <summary>
        /// Writes generated documentation namespace pages.
        /// </summary>
        /// <param name="items">The items value used by the documentation generation operation.</param>
        /// <param name="pageOutputDirectory">The pageOutputDirectory value used by the documentation generation operation.</param>
        /// <param name="sharedDocumentationRootDirectory">The sharedDocumentationRootDirectory value used by the documentation generation operation.</param>
        public static void RenderNamespacePages(
            IEnumerable<DocumentationNamespacePageModel> items,
            string pageOutputDirectory,
            string sharedDocumentationRootDirectory
        )
        {
            foreach (DocumentationNamespacePageModel item in items)
            {
                DocumentationContextPartialGenerator.EnsureNamespacePartials(
                    sharedDocumentationRootDirectory,
                    item.NamespaceName);
            }
        }

        private static void RenderObjectSection(
            StringBuilder sb,
            string packageId,
            string version,
            string groupName,
            string namespaceName,
            string sectionId,
            string sectionTitle,
            IReadOnlyCollection<DocumentationNamespaceObjectLinkItem> items
        )
        {
            if (items.Count == 0) return;

            sb.Append("                <section id=\"")
                .Append(sectionId)
                .AppendLine("\" class=\"card border-0 shadow-sm mb-3\">");
            sb.AppendLine("                    <div class=\"card-body p-2\">");
            sb.Append("                        <h2>")
                .Append(Html(sectionTitle))
                .AppendLine("</h2>");
            sb.AppendLine("                        <ul class=\"list-group list-group-flush\">");

            foreach (DocumentationNamespaceObjectLinkItem item in items.OrderBy(x => x.Name, StringComparer.Ordinal))
            {
                sb.AppendLine("                            <li class=\"list-group-item px-0\">");
                sb.Append("                                <a href=\"/Documentation/Show?packageId=")
                    .Append(System.Net.WebUtility.UrlEncode(packageId))
                    .Append("&version=")
                    .Append(System.Net.WebUtility.UrlEncode(version))
                    .Append("&groupName=")
                    .Append(System.Net.WebUtility.UrlEncode(groupName))
                    .Append("&namespaceName=")
                    .Append(System.Net.WebUtility.UrlEncode(namespaceName))
                    .Append("&objectName=")
                    .Append(System.Net.WebUtility.UrlEncode(item.Name))
                    .AppendLine("\">");
                sb.Append("                                    <code>")
                    .Append(Html(item.Name))
                    .AppendLine("</code>");
                sb.AppendLine("                                </a>");
                sb.AppendLine("                            </li>");
            }

            sb.AppendLine("                        </ul>");
            sb.AppendLine("                    </div>");
            sb.AppendLine("                </section>");
        }

        #endregion
    }
}
