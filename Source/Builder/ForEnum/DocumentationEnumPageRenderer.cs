#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Collections.Generic;
using System.IO;
using System.Text;

#endregion

namespace DMBDocumentationBuilder
{
    internal static class DocumentationEnumPageRenderer
    {
        #region Static methods

        private static string BuildPartialRoot(string namespaceName, string objectName)
        {
            string namespacePath = namespaceName.Replace('.', '/');
            return $"~/Views/Shared/Documentation/{namespacePath}/{objectName}";
        }

        private static string EscapeCSharp(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private static string Html(string? value)
        {
            return System.Net.WebUtility.HtmlEncode(value ?? string.Empty);
        }

        private static string ReadPartialContent(
            string sharedDocumentationRootDirectory,
            string namespaceName,
            string objectName,
            string partialName
        )
        {
            string namespaceFolder = DocumentationPathHelper.NamespaceToFolder(namespaceName);
            string filePath = Path.Combine(
                sharedDocumentationRootDirectory,
                namespaceFolder,
                objectName,
                partialName);

            string result = $@"<!-- Custom documentation slot -->
<!-- Namespace: {namespaceName} -->
<!-- Object: {objectName} -->
<!-- Slot: {partialName} -->
";

            if (File.Exists(filePath))
            {
                result += $"<div class=\"badge rounded-pill d-flex text-bg-info theme-debug-only my-2\">You can edit /Documentation/{namespaceFolder}/{objectName}/{partialName}</div>";
                result += File.ReadAllText(filePath);
            }
            else
            {
                result += $"<div class=\"badge rounded-pill d-flex text-bg-info theme-debug-only my-2\">You can create and edit /Documentation/{namespaceFolder}/{objectName}/{partialName}</div>";
                //result += $"<p class=\"text-break\">FILE=&quot;{filePath}&quot;</p><p>mkdir -p &quot;$(dirname &quot;$FILE&quot;)&quot; && touch &quot;$FILE&quot;</p>";
            }

            return result;
        }

        /// <summary>
        ///     Renders the HTML content for one generated documentation page.
        /// </summary>
        /// <param name="item">The item value used by the documentation generation operation.</param>
        /// <param name="sharedDocumentationRootDirectory">
        ///     The sharedDocumentationRootDirectory value used by the documentation
        ///     generation operation.
        /// </param>
        /// <returns>The RenderHtmlPage result produced by DocumentationBuilder generation.</returns>
        public static string RenderHtmlPage(
            DocumentationEnumPageModel item,
            string sharedDocumentationRootDirectory
        )
        {
            StringBuilder sb = new();

            sb.AppendLine("<div class=\"ex-container py-3 ex-py-lg-5\">");
            sb.AppendLine("    <div class=\"row g-3\">");

            sb.AppendLine("        <div class=\"col-12\">");

            sb.AppendLine(ReadPartialContent(
                sharedDocumentationRootDirectory,
                item.NamespaceName,
                item.EnumName,
                "_Before_Page.html"));

            sb.AppendLine("            <div class=\"mb-4\">");

            sb.AppendLine(ReadPartialContent(
                sharedDocumentationRootDirectory,
                item.NamespaceName,
                item.EnumName,
                "_After_Header.html"));

            sb.Append("                <h1 class=\"display-6 fw-bold mb-3\">")
                .Append(Html(item.EnumName))
                .AppendLine("</h1>");

            if (item.IsFlags) sb.AppendLine("                <p class=\"text-body-secondary\">This enumeration supports bitwise combination of values.</p>");

            if (item.IsObsolete)
            {
                sb.AppendLine("                <div class=\"alert alert-danger mb-4 shadow-sm\">");
                sb.AppendLine("                    <h2 class=\"alert-heading fw-bold mb-1\"><i class=\"bi bi-exclamation-triangle-fill me-2\"></i>Obsolete</h2>");
                sb.Append("                    <div class=\"small\">")
                    .Append(string.IsNullOrWhiteSpace(item.ObsoleteMessage) ? "This enum is obsolete." : Html(item.ObsoleteMessage))
                    .AppendLine("</div>");
                sb.AppendLine("                </div>");
            }

            if (item.XmlDoc.HasSummary)
            {
                sb.Append("                <p class=\"lead text-body-secondary mb-4\">")
                    .Append(item.XmlDoc.SummaryHtml)
                    .AppendLine("</p>");
            }

            sb.AppendLine(ReadPartialContent(
                sharedDocumentationRootDirectory,
                item.NamespaceName,
                item.EnumName,
                "_After_Introduction.html"));

            if (item.XmlDoc.HasRemarks)
            {
                sb.AppendLine("                <section id=\"remarks\" class=\"card border-0 shadow-sm mb-3\">");
                sb.AppendLine("                    <div class=\"card-body p-2\">");
                sb.AppendLine("                        <h2>Remarks</h2>");
                sb.Append("                        <div class=\"text-body-secondary\">")
                    .Append(item.XmlDoc.RemarksHtml)
                    .AppendLine("</div>");
                sb.AppendLine("                    </div>");
                sb.AppendLine(ReadPartialContent(
                    sharedDocumentationRootDirectory,
                    item.NamespaceName,
                    item.EnumName,
                    "_After_Remarks.cshtml"));
                sb.AppendLine("                </section>");
            }

            if (item.XmlDoc.HasExample)
            {
                sb.AppendLine("                <section id=\"example\" class=\"card border-0 shadow-sm mb-3\">");
                sb.AppendLine("                    <div class=\"card-body p-2\">");
                sb.AppendLine("                        <h2>Example</h2>");
                sb.Append("                        <div>")
                    .Append(item.XmlDoc.ExampleHtml)
                    .AppendLine("</div>");
                sb.AppendLine("                    </div>");
                sb.AppendLine(ReadPartialContent(
                    sharedDocumentationRootDirectory,
                    item.NamespaceName,
                    item.EnumName,
                    "_After_Example.cshtml"));
                sb.AppendLine("                </section>");
            }

            if (item.XmlDoc.HasSeeAlsos)
            {
                sb.AppendLine("                <section id=\"see-also\" class=\"card border-0 shadow-sm mb-3\">");
                sb.AppendLine("                    <div class=\"card-body p-2\">");
                sb.AppendLine("                        <h2>See Also</h2>");
                sb.AppendLine("                        <ul class=\"list-group list-group-flush\">");

                foreach (DocumentationXmlLinkItem link in item.XmlDoc.SeeAlsos)
                {
                    sb.AppendLine("                            <li class=\"list-group-item px-0\">");
                    sb.Append("                                ")
                        .Append(RenderSeeAlsoItem(link))
                        .AppendLine();
                    sb.AppendLine("                            </li>");
                }

                sb.AppendLine("                        </ul>");
                sb.AppendLine("                    </div>");
                sb.AppendLine(ReadPartialContent(
                    sharedDocumentationRootDirectory,
                    item.NamespaceName,
                    item.EnumName,
                    "_After_SeeAlso.cshtml"));
                sb.AppendLine("                </section>");
            }

            sb.AppendLine("                <section id=\"metadata\" class=\"card border-0 shadow-sm mb-3\">");
            sb.AppendLine("                    <div class=\"card-body p-2\">");
            sb.AppendLine("                        <h2>Metadata</h2>");
            sb.AppendLine("                        <dl class=\"row mb-0\">");

            sb.AppendLine("                            <dt class=\"col-sm-3\">Kind</dt>");
            sb.AppendLine("                            <dd class=\"col-sm-9\"><code>Enum</code></dd>");

            sb.AppendLine("                            <dt class=\"col-sm-3\">Package ID</dt>");
            sb.Append("                            <dd class=\"col-sm-9\"><code>")
                .Append(Html(item.PackageId))
                .AppendLine("</code></dd>");

            sb.AppendLine("                            <dt class=\"col-sm-3\">Version</dt>");
            sb.Append("                            <dd class=\"col-sm-9\"><code>")
                .Append(Html(item.Version))
                .AppendLine("</code></dd>");

            sb.AppendLine("                            <dt class=\"col-sm-3\">Namespace</dt>");
            sb.Append("                            <dd class=\"col-sm-9\"><code>")
                .Append(Html(item.NamespaceName))
                .AppendLine("</code></dd>");

            sb.AppendLine("                            <dt class=\"col-sm-3\">Assembly</dt>");
            sb.Append("                            <dd class=\"col-sm-9\"><code>")
                .Append(Html(item.AssemblyName))
                .AppendLine("</code></dd>");

            sb.AppendLine("                            <dt class=\"col-sm-3\">Accessibility</dt>");
            sb.Append("                            <dd class=\"col-sm-9\"><code>")
                .Append(Html(item.Accessibility))
                .AppendLine("</code></dd>");

            sb.AppendLine("                            <dt class=\"col-sm-3\">Obsolete</dt>");
            sb.Append("                            <dd class=\"col-sm-9\"><code>")
                .Append(item.IsObsolete ? "Yes" : "No")
                .AppendLine("</code></dd>");

            sb.AppendLine("                            <dt class=\"col-sm-3\">Sealed</dt>");
            sb.AppendLine("                            <dd class=\"col-sm-9\"><code>Yes</code></dd>");

            sb.AppendLine("                            <dt class=\"col-sm-3\">Flags</dt>");
            sb.Append("                            <dd class=\"col-sm-9\"><code>")
                .Append(item.IsFlags ? "Yes" : "No")
                .AppendLine("</code></dd>");

            sb.AppendLine("                            <dt class=\"col-sm-3\">Underlying Type</dt>");
            sb.Append("                            <dd class=\"col-sm-9\"><code>")
                .Append(Html(item.UnderlyingType))
                .AppendLine("</code></dd>");

            sb.AppendLine("                        </dl>");
            sb.AppendLine("                    </div>");
            sb.AppendLine(ReadPartialContent(
                sharedDocumentationRootDirectory,
                item.NamespaceName,
                item.EnumName,
                "_After_Metadata.cshtml"));
            sb.AppendLine("                </section>");

            sb.AppendLine("                <section id=\"declaration\" class=\"card border-0 shadow-sm\">");
            sb.AppendLine("                    <div class=\"card-body p-2\">");
            sb.AppendLine("                        <h2>Declaration</h2>");
            sb.Append("                        <pre class=\"bg-body-tertiary border rounded-3 p-3 mb-0\"><code>")
                .Append(Html(item.Declaration))
                .AppendLine("</code></pre>");
            sb.AppendLine("                    </div>");

            sb.AppendLine(ReadPartialContent(
                sharedDocumentationRootDirectory,
                item.NamespaceName,
                item.EnumName,
                "_After_Declaration.cshtml"));
            sb.AppendLine("                </section>");

            if (item.ExtensionMethods.Count > 0)
            {
                sb.AppendLine("                <section id=\"extension-methods\" class=\"card border-0 shadow-sm mt-3\">");
                sb.AppendLine("                    <div class=\"card-body p-2\">");
                sb.AppendLine("                        <h2>Extension Methods</h2>");
                sb.AppendLine("                        <ul class=\"list-group list-group-flush\">");

                foreach (DocumentationExtensionMethodItem method in item.ExtensionMethods)
                {
                    sb.AppendLine("                            <li class=\"list-group-item px-0\">");
                    sb.Append("                                <a href=\"/Documentation/Show?packageId=")
                        .Append(System.Net.WebUtility.UrlEncode(item.PackageId))
                        .Append("&version=")
                        .Append(System.Net.WebUtility.UrlEncode(item.Version))
                        .Append("&groupName=")
                        .Append(System.Net.WebUtility.UrlEncode(item.GroupName))
                        .Append("&namespaceName=")
                        .Append(EscapeCSharp(method.ExtensionNamespaceName))
                        .Append("&objectName=")
                        .Append(EscapeCSharp(method.ExtensionTypeName))
                        .AppendLine("\">");
                    sb.Append("                                    ")
                        .Append(DocumentationVisualHelper.ExtensionIconHtml)
                        .Append(DocumentationVisualHelper.RenderAccessibilityIcon(method.Accessibility))
                        .Append("<code>")
                        .Append(Html(method.Signature))
                        .AppendLine("</code>");
                    sb.AppendLine("                                </a>");
                    sb.AppendLine("                            </li>");
                }

                sb.AppendLine("                        </ul>");
                sb.AppendLine("                    </div>");
                sb.AppendLine(ReadPartialContent(
                    sharedDocumentationRootDirectory,
                    item.NamespaceName,
                    item.EnumName,
                    "_After_ExtensionMethods.cshtml"));
                sb.AppendLine("                </section>");
            }

            sb.AppendLine("            </div>");

            sb.AppendLine("            <section id=\"fields\" class=\"card border-0 shadow-sm\">");
            sb.AppendLine("                <div class=\"card-body p-2\">");
            sb.AppendLine("                    <h2>Fields</h2>");

            if (item.Values.Count == 0)
            {
                sb.AppendLine("                    <p class=\"text-body-secondary mb-0\">No values available.</p>");
            }
            else
            {
                sb.AppendLine("                    <div class=\"table-responsive\">");
                sb.AppendLine("                        <table class=\"table align-middle mb-0\">");
                sb.AppendLine("                            <thead>");
                sb.AppendLine("                            <tr>");
                sb.AppendLine("                                <th style=\"width: 140px;\">Name</th>");
                sb.AppendLine("                                <th style=\"width: 80px;\">Value</th>");

                if (item.IsFlags)
                {
                    sb.AppendLine("                                <th style=\"width: 100px;\">Hex</th>");
                    sb.AppendLine("                                <th style=\"width: 140px;\">Binary</th>");
                    sb.AppendLine("                                <th style=\"width: 100px;\">Bit</th>");
                }

                sb.AppendLine("                                <th>Description</th>");
                sb.AppendLine("                            </tr>");
                sb.AppendLine("                            </thead>");
                sb.AppendLine("                            <tbody>");

                foreach (DocumentationEnumValueItem value in item.Values)
                {
                    sb.Append("                            <tr id=\"")
                        .Append(Html(value.Name))
                        .AppendLine("\">");
                    sb.Append("                                <td>");
                    if (value.IsObsolete) sb.Append("<span class=\"text-decoration-line-through text-danger\">");

                    sb.Append("<code>")
                        .Append(Html(value.Name))
                        .Append("</code>");

                    if (value.IsObsolete) sb.Append("</span>");

                    sb.AppendLine("</td>");
                    sb.Append("                                <td><code>")
                        .Append(Html(value.Value))
                        .AppendLine("</code></td>");

                    if (item.IsFlags)
                    {
                        sb.Append("                                <td><code>")
                            .Append(Html(value.HexValue))
                            .AppendLine("</code></td>");
                        sb.Append("                                <td><code>")
                            .Append(Html(value.BinaryValue))
                            .AppendLine("</code></td>");
                        sb.Append("                                <td><code>")
                            .Append(Html(value.BitShiftValue))
                            .AppendLine("</code></td>");
                    }

                    sb.Append("                                <td class=\"text-body-secondary\">");
                    if (value.IsObsolete)
                    {
                        sb.Append("<div class=\"text-danger fw-bold small mb-1\"><i class=\"bi bi-exclamation-triangle-fill me-1\"></i>Obsolete</div>");
                        if (!string.IsNullOrWhiteSpace(value.ObsoleteMessage)) sb.Append("<div class=\"text-danger small mb-2\">").Append(Html(value.ObsoleteMessage)).Append("</div>");
                    }

                    sb.Append(RenderValueDescription(value))
                        .AppendLine("</td>");
                    sb.AppendLine("                            </tr>");
                }

                sb.AppendLine("                            </tbody>");
                sb.AppendLine("                        </table>");
                sb.AppendLine("                    </div>");
            }

            sb.AppendLine("                </div>");
            sb.AppendLine(ReadPartialContent(
                sharedDocumentationRootDirectory,
                item.NamespaceName,
                item.EnumName,
                "_After_Fields.cshtml"));
            sb.AppendLine("            </section>");

            sb.AppendLine(ReadPartialContent(
                sharedDocumentationRootDirectory,
                item.NamespaceName,
                item.EnumName,
                "_After_Page.cshtml"));

            sb.AppendLine("        </div>");

            sb.AppendLine("    </div>");
            // sb.AppendLine("    <div class=\"mt-5 pt-3 border-top text-center text-muted small\">");
            // sb.Append("        Documentation generated by DMBDocumentationBuilder v")
            //     .Append(DocumentationVisualHelper.DocumentationBuilderVersion)
            //     .AppendLine();
            // sb.AppendLine("    </div>");
            sb.AppendLine("</div>");

            return DocumentationCodeBlockRenderer.EnhanceCSharpCodeBlocks(sb.ToString());
        }

        /// <summary>
        ///     Writes generated documentation pages for the supplied documentation models.
        /// </summary>
        /// <param name="items">The items value used by the documentation generation operation.</param>
        /// <param name="pageOutputDirectory">The pageOutputDirectory value used by the documentation generation operation.</param>
        /// <param name="sharedDocumentationRootDirectory">
        ///     The sharedDocumentationRootDirectory value used by the documentation
        ///     generation operation.
        /// </param>
        public static void RenderPages(
            IEnumerable<DocumentationEnumPageModel> items,
            string pageOutputDirectory,
            string sharedDocumentationRootDirectory
        )
        {
            Directory.CreateDirectory(sharedDocumentationRootDirectory);

            foreach (DocumentationEnumPageModel item in items)
            {
                DocumentationPartialGenerator.EnsurePartials(
                    sharedDocumentationRootDirectory,
                    item.NamespaceName,
                    item.EnumName);
            }
        }

        private static string RenderSeeAlsoItem(DocumentationXmlLinkItem item)
        {
            if (item.IsKeyword) return $"<code>{Html(item.Label)}</code>";

            if (string.IsNullOrWhiteSpace(item.Href)) return $"<code>{Html(item.Label)}</code>";

            return $"<a href=\"{item.Href}\"><code>{Html(item.Label)}</code></a>";
        }

        private static string RenderValueDescription(DocumentationEnumValueItem value)
        {
            StringBuilder sb = new();

            if (!value.XmlDoc.HasSummary && !value.XmlDoc.HasRemarks)
            {
                sb.Append("No description yet.");
                return sb.ToString();
            }

            if (value.XmlDoc.HasSummary)
            {
                sb.Append("<div>")
                    .Append(value.XmlDoc.SummaryHtml)
                    .Append("</div>");
            }

            if (value.XmlDoc.HasRemarks)
            {
                sb.Append("<div class=\"mt-2\">")
                    .Append(value.XmlDoc.RemarksHtml)
                    .Append("</div>");
            }

            return sb.ToString();
        }

        #endregion
    }
}