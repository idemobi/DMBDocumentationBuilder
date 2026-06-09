#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Text;

#endregion

namespace DMBDocumentationBuilder
{
    internal static class DocumentationInterfacePageRenderer
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

        private static int GetAccessibilityOrder(string accessibility)
        {
            return accessibility switch
            {
                "public" => 1,
                "internal" => 2,
                "protected" => 3,
                "protected internal" => 4,
                "private protected" => 5,
                "private" => 6,
                _ => 7
            };
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

        private static string RenderEventBody(DocumentationInterfaceEventItem eventItem)
        {
            StringBuilder sb = new();

            sb.AppendLine(DocumentationVisualHelper.RenderMemberSignature(
                RenderMemberIcons(
                    eventItem.Accessibility,
                    isStatic: eventItem.IsStatic),
                eventItem.Signature,
                eventItem.IsObsolete));

            if (eventItem.IsObsolete)
            {
                sb.Append("<div class=\"text-danger fw-bold small mb-1\"><i class=\"bi bi-exclamation-triangle-fill me-1\"></i>Obsolete</div>");
                if (!string.IsNullOrWhiteSpace(eventItem.ObsoleteMessage)) sb.Append("<div class=\"text-danger small mb-2\">").Append(Html(eventItem.ObsoleteMessage)).Append("</div>");
            }

            if (eventItem.XmlDoc.HasSummary)
            {
                sb.Append("<div class=\"text-body-secondary mt-2\">")
                    .Append(eventItem.XmlDoc.SummaryHtml)
                    .AppendLine("</div>");
            }

            if (eventItem.XmlDoc.HasRemarks)
            {
                sb.Append("<div class=\"mt-2 small\">")
                    .AppendLine("<span class=\"text-body-secondary fw-bold\">Remarks:</span>")
                    .Append("<div class=\"ms-3\">")
                    .Append(eventItem.XmlDoc.RemarksHtml)
                    .AppendLine("</div></div>");
            }

            if (eventItem.XmlDoc.HasExample)
            {
                sb.AppendLine("<div class=\"mt-2 small\">");
                sb.AppendLine("    <span class=\"text-body-secondary fw-bold\">Example:</span>");
                sb.Append("<div class=\"ms-3\">")
                    .Append(eventItem.XmlDoc.ExampleHtml)
                    .AppendLine("</div></div>");
            }

            return sb.ToString();
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
            DocumentationInterfacePageModel item,
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
                item.InterfaceName,
                "_Before_Page.html"));

            sb.AppendLine("            <div class=\"mb-4\">");

            sb.AppendLine(ReadPartialContent(
                sharedDocumentationRootDirectory,
                item.NamespaceName,
                item.InterfaceName,
                "_After_Header.html"));

            sb.Append("                <h1 class=\"display-6 fw-bold mb-3\">")
                .Append(Html(item.InterfaceName))
                .AppendLine("</h1>");

            if (item.IsObsolete)
            {
                sb.AppendLine("                <div class=\"alert alert-danger mb-4 shadow-sm\">");
                sb.AppendLine("                    <h2 class=\"alert-heading fw-bold mb-1\"><i class=\"bi bi-exclamation-triangle-fill me-2\"></i>Obsolete</h2>");
                sb.Append("                    <div class=\"small\">")
                    .Append(string.IsNullOrWhiteSpace(item.ObsoleteMessage) ? "This interface is obsolete." : Html(item.ObsoleteMessage))
                    .AppendLine("</div>");
                sb.AppendLine("                </div>");
            }

            if (item.XmlDoc.HasSummary)
            {
                sb.Append("                <p class=\"lead text-body-secondary mb-4\">")
                    .Append(item.XmlDoc.SummaryHtml)
                    .AppendLine("</p>");
            }

            if (item.XmlDoc.HasRemarks)
            {
                sb.AppendLine("                <div class=\"mb-4\">");
                sb.AppendLine("                    <h2 class=\"text-body-secondary\">Remarks</h2>");
                sb.Append("                    <div>").Append(item.XmlDoc.RemarksHtml).AppendLine("</div>");
                sb.AppendLine("                </div>");
            }

            if (item.XmlDoc.HasExample)
            {
                sb.AppendLine("                <div class=\"mb-4\">");
                sb.AppendLine("                    <h2 class=\"text-body-secondary\">Example</h2>");
                sb.Append("                    <div>").Append(item.XmlDoc.ExampleHtml).AppendLine("</div>");
                sb.AppendLine("                </div>");
            }

            if (item.XmlDoc.HasSeeAlsos)
            {
                sb.AppendLine("                <div class=\"mb-4\">");
                sb.AppendLine("                    <h2 class=\"text-body-secondary\">See Also</h2>");
                sb.AppendLine("                    <ul class=\"list-unstyled\">");
                foreach (var link in item.XmlDoc.SeeAlsos)
                {
                    sb.Append("                        <li>").Append(RenderSeeAlsoItem(link)).AppendLine("</li>");
                }

                sb.AppendLine("                    </ul>");
                sb.AppendLine("                </div>");
            }

            sb.AppendLine(ReadPartialContent(
                sharedDocumentationRootDirectory,
                item.NamespaceName,
                item.InterfaceName,
                "_After_Introduction.html"));

            sb.AppendLine("                <section id=\"metadata\" class=\"card border-0 shadow-sm mb-3\">");
            sb.AppendLine("                    <div class=\"card-body p-2\">");
            sb.AppendLine("                        <h2>Metadata</h2>");
            sb.AppendLine("                        <dl class=\"row mb-0\">");

            sb.AppendLine("                            <dt class=\"col-sm-3\">Kind</dt>");
            sb.AppendLine("                            <dd class=\"col-sm-9\"><code>Interface</code></dd>");

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

            if (!string.IsNullOrWhiteSpace(item.Accessibility))
            {
                sb.AppendLine("                            <dt class=\"col-sm-3\">Accessibility</dt>");
                sb.Append("                            <dd class=\"col-sm-9\"><code>")
                    .Append(Html(item.Accessibility))
                    .AppendLine("</code></dd>");
            }

            sb.AppendLine("                            <dt class=\"col-sm-3\">Obsolete</dt>");
            sb.Append("                            <dd class=\"col-sm-9\"><code>")
                .Append(item.IsObsolete ? "Yes" : "No")
                .AppendLine("</code></dd>");

            sb.AppendLine("                            <dt class=\"col-sm-3\">Sealed</dt>");
            sb.AppendLine("                            <dd class=\"col-sm-9\"><code>No</code></dd>");

            sb.AppendLine("                        </dl>");
            sb.AppendLine("                    </div>");
            sb.AppendLine(ReadPartialContent(
                sharedDocumentationRootDirectory,
                item.NamespaceName,
                item.InterfaceName,
                "_After_Metadata.html"));
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
                item.InterfaceName,
                "_After_Declaration.html"));
            sb.AppendLine("                </section>");

            sb.Append(DocumentationDependencyGraphRenderer.RenderHtml(item.DependencyEdges));
            sb.AppendLine(ReadPartialContent(
                sharedDocumentationRootDirectory,
                item.NamespaceName,
                item.InterfaceName,
                "_After_Dependency_Graph.html"));

            if (item.ExtensionMethods.Count > 0)
            {
                sb.Append("                <section id=\"extension-methods\" class=\"card border-0 shadow-sm mt-3\" ")
                    .Append(DocumentationDisplayFilterHtml.SectionAttributes("extension-method"))
                    .AppendLine(">");
                sb.AppendLine("                    <div class=\"card-body p-2\">");
                sb.AppendLine("                        <h2>Extension Methods</h2>");
                sb.AppendLine("                        <ul class=\"list-group list-group-flush\">");

                foreach (DocumentationExtensionMethodItem method in item.ExtensionMethods)
                {
                    sb.Append("                            <li class=\"list-group-item px-0 py-3 my-2\" ")
                        .Append(DocumentationDisplayFilterHtml.MemberAttributes("extension-method", method.Accessibility))
                        .AppendLine(">");
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
                        .Append(DocumentationVisualHelper.RenderMemberSignature(
                            DocumentationVisualHelper.ExtensionIconHtml + DocumentationVisualHelper.RenderAccessibilityIcon(method.Accessibility),
                            method.Signature,
                            false))
                        .AppendLine();
                    sb.AppendLine("                                </a>");
                    sb.AppendLine("                            </li>");
                }

                sb.AppendLine("                        </ul>");
                sb.AppendLine("                    </div>");
                sb.AppendLine(ReadPartialContent(
                    sharedDocumentationRootDirectory,
                    item.NamespaceName,
                    item.InterfaceName,
                    "_After_ExtensionMethods.html"));
                sb.AppendLine("                </section>");
            }

            if (item.Properties.Count > 0)
            {
                sb.Append("                <section id=\"properties\" class=\"card border-0 shadow-sm mt-3\" ")
                    .Append(DocumentationDisplayFilterHtml.SectionAttributes("property"))
                    .AppendLine(">");
                sb.AppendLine("                    <div class=\"card-body p-2\">");
                sb.AppendLine("                        <h2>Properties</h2>");

                var groups = item.Properties
                    .GroupBy(p => p.IsStatic ? "Static Properties" : "Instance Properties")
                    .OrderByDescending(g => g.Key);

                foreach (var group in groups)
                {
                    sb.Append("                        <h3 class=\"mt-3 mb-2 ms-2\" data-doc-member-group-heading=\"true\">").Append(group.Key).AppendLine("</h3>");
                    var accessibilityGroups = group.GroupBy(p => p.Accessibility).OrderBy(g => GetAccessibilityOrder(g.Key));
                    foreach (var accGroup in accessibilityGroups)
                    {
                        sb.Append("                        <h4 class=\"mt-2 mb-2 ms-3 text-body-secondary\" data-doc-access-heading=\"true\">").Append(accGroup.Key).AppendLine("</h4>");
                        sb.AppendLine("                        <ul class=\"list-group list-group-flush\">");
                        foreach (var property in accGroup.OrderBy(p => p.PropertyName))
                        {
                            sb.Append("                            <li class=\"list-group-item px-0 py-3 my-2 ms-4\" ")
                                .Append(DocumentationDisplayFilterHtml.MemberAttributes("property", property.Accessibility))
                                .Append(" id=\"").Append(Html(property.PropertyName)).AppendLine("\">");
                            sb.Append(RenderPropertyBody(property));
                            sb.AppendLine("                            </li>");
                        }

                        sb.AppendLine("                        </ul>");
                    }
                }

                sb.AppendLine("                    </div>");
                sb.AppendLine(ReadPartialContent(
                    sharedDocumentationRootDirectory,
                    item.NamespaceName,
                    item.InterfaceName,
                    "_After_Properties.html"));
                sb.AppendLine("                </section>");
            }

            if (item.Methods.Count > 0)
            {
                sb.Append("                <section id=\"methods\" class=\"card border-0 shadow-sm mt-3\" ")
                    .Append(DocumentationDisplayFilterHtml.SectionAttributes("method"))
                    .AppendLine(">");
                sb.AppendLine("                    <div class=\"card-body p-2\">");
                sb.AppendLine("                        <h2>Methods</h2>");

                var groups = item.Methods
                    .GroupBy(m => m.IsStatic ? "Static Methods" : "Instance Methods")
                    .OrderByDescending(g => g.Key);

                foreach (var group in groups)
                {
                    sb.Append("                        <h3 class=\"mt-3 mb-2 ms-2\" data-doc-member-group-heading=\"true\">").Append(group.Key).AppendLine("</h3>");
                    var accessibilityGroups = group.GroupBy(m => m.Accessibility).OrderBy(g => GetAccessibilityOrder(g.Key));
                    foreach (var accGroup in accessibilityGroups)
                    {
                        sb.Append("                        <h4 class=\"mt-2 mb-2 ms-3 text-body-secondary\" data-doc-access-heading=\"true\">").Append(accGroup.Key).AppendLine("</h4>");
                        sb.AppendLine("                        <ul class=\"list-group list-group-flush\">");
                        foreach (var method in accGroup.OrderBy(m => m.MethodName))
                        {
                            sb.Append("                            <li class=\"list-group-item px-0 py-3 my-2 ms-4\" ")
                                .Append(DocumentationDisplayFilterHtml.MemberAttributes("method", method.Accessibility))
                                .Append(" id=\"").Append(Html(method.MethodName)).AppendLine("\">");
                            sb.Append(RenderMethodBody(method));
                            sb.AppendLine("                            </li>");
                        }

                        sb.AppendLine("                        </ul>");
                    }
                }

                sb.AppendLine("                    </div>");
                sb.AppendLine(ReadPartialContent(
                    sharedDocumentationRootDirectory,
                    item.NamespaceName,
                    item.InterfaceName,
                    "_After_Methods.html"));
                sb.AppendLine("                </section>");
            }

            if (item.Events.Count > 0)
            {
                sb.Append("                <section id=\"events\" class=\"card border-0 shadow-sm mt-3\" ")
                    .Append(DocumentationDisplayFilterHtml.SectionAttributes("event"))
                    .AppendLine(">");
                sb.AppendLine("                    <div class=\"card-body p-2\">");
                sb.AppendLine("                        <h2>Events</h2>");

                var groups = item.Events
                    .GroupBy(e => e.IsStatic ? "Static Events" : "Instance Events")
                    .OrderByDescending(g => g.Key);

                foreach (var group in groups)
                {
                    sb.Append("                        <h3 class=\"mt-3 mb-2 ms-2\" data-doc-member-group-heading=\"true\">").Append(group.Key).AppendLine("</h3>");
                    var accessibilityGroups = group.GroupBy(e => e.Accessibility).OrderBy(g => GetAccessibilityOrder(g.Key));
                    foreach (var accGroup in accessibilityGroups)
                    {
                        sb.Append("                        <h4 class=\"mt-2 mb-2 ms-3 text-body-secondary\" data-doc-access-heading=\"true\">").Append(accGroup.Key).AppendLine("</h4>");
                        sb.AppendLine("                        <ul class=\"list-group list-group-flush\">");
                        foreach (var eventItem in accGroup.OrderBy(e => e.EventName))
                        {
                            sb.Append("                            <li class=\"list-group-item px-0 py-3 my-2 ms-4\" ")
                                .Append(DocumentationDisplayFilterHtml.MemberAttributes("event", eventItem.Accessibility))
                                .Append(" id=\"").Append(Html(eventItem.EventName)).AppendLine("\">");
                            sb.Append(RenderEventBody(eventItem));
                            sb.AppendLine("                            </li>");
                        }

                        sb.AppendLine("                        </ul>");
                    }
                }

                sb.AppendLine("                    </div>");
                sb.AppendLine(ReadPartialContent(
                    sharedDocumentationRootDirectory,
                    item.NamespaceName,
                    item.InterfaceName,
                    "_After_Events.html"));
                sb.AppendLine("                </section>");
            }

            sb.AppendLine(ReadPartialContent(
                sharedDocumentationRootDirectory,
                item.NamespaceName,
                item.InterfaceName,
                "_After_Page.html"));

            sb.AppendLine("            </div>");
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

        private static string RenderMemberIcons(
            string? accessibility,
            bool isStatic = false,
            bool isReadOnly = false,
            bool isAbstract = false,
            bool isVirtual = false,
            bool isOverride = false,
            bool isSealed = false,
            bool isExtension = false
        )
        {
            StringBuilder sb = new();

            sb.Append(DocumentationVisualHelper.RenderAccessibilityIcon(accessibility));

            if (isExtension) sb.Append(DocumentationVisualHelper.ExtensionIconHtml);

            if (isStatic) sb.Append(DocumentationVisualHelper.StaticIconHtml);

            if (isReadOnly) sb.Append(DocumentationVisualHelper.ReadOnlyIconHtml);

            if (isAbstract) sb.Append(DocumentationVisualHelper.AbstractIconHtml);

            if (isVirtual) sb.Append(DocumentationVisualHelper.VirtualIconHtml);

            if (isOverride) sb.Append(DocumentationVisualHelper.OverrideIconHtml);

            if (isSealed) sb.Append(DocumentationVisualHelper.SealedIconHtml);

            return sb.ToString();
        }

        private static string RenderMethodBody(DocumentationInterfaceMethodItem method)
        {
            StringBuilder sb = new();

            sb.AppendLine(DocumentationVisualHelper.RenderMemberSignature(
                RenderMemberIcons(
                    method.Accessibility,
                    isStatic: method.IsStatic,
                    isAbstract: method.IsAbstract,
                    isVirtual: method.IsVirtual,
                    isOverride: method.IsOverride,
                    isSealed: method.IsSealed),
                method.Signature,
                method.IsObsolete));

            if (method.IsObsolete)
            {
                sb.Append("<div class=\"text-danger fw-bold small mb-1\"><i class=\"bi bi-exclamation-triangle-fill me-1\"></i>Obsolete</div>");
                if (!string.IsNullOrWhiteSpace(method.ObsoleteMessage)) sb.Append("<div class=\"text-danger small mb-2\">").Append(Html(method.ObsoleteMessage)).Append("</div>");
            }

            if (method.XmlDoc.HasSummary)
            {
                sb.Append("<div class=\"text-body-secondary mt-2\">")
                    .Append(method.XmlDoc.SummaryHtml)
                    .AppendLine("</div>");
            }

            if (method.XmlDoc.HasRemarks)
            {
                sb.Append("<div class=\"mt-2 small\">")
                    .AppendLine("<span class=\"text-body-secondary fw-bold\">Remarks:</span>")
                    .Append("<div class=\"ms-3\">")
                    .Append(method.XmlDoc.RemarksHtml)
                    .AppendLine("</div></div>");
            }

            if (method.XmlDoc.HasTypeParameters)
            {
                sb.AppendLine("<div class=\"mt-2 small\">");
                sb.AppendLine("    <div class=\"text-body-secondary fw-bold mb-1\">Type Parameters:</div>");
                sb.AppendLine("    <dl class=\"row mb-0 ms-1\">");
                foreach (var item in method.XmlDoc.TypeParameters)
                {
                    sb.Append("        <dt class=\"col-sm-3\"><code>").Append(Html(item.Name)).AppendLine("</code></dt>");
                    sb.Append("        <dd class=\"col-sm-9\">").Append(item.Html).AppendLine("</dd>");
                }

                sb.AppendLine("    </dl>");
                sb.AppendLine("</div>");
            }

            if (method.XmlDoc.HasParameters)
            {
                sb.AppendLine("<div class=\"mt-2 small\">");
                sb.AppendLine("    <div class=\"text-body-secondary fw-bold mb-1\">Parameters:</div>");
                sb.AppendLine("    <dl class=\"row mb-0 ms-1\">");
                foreach (var item in method.XmlDoc.Parameters)
                {
                    sb.Append("        <dt class=\"col-sm-3\"><code>").Append(Html(item.Name)).AppendLine("</code></dt>");
                    sb.Append("        <dd class=\"col-sm-9\">").Append(item.Html).AppendLine("</dd>");
                }

                sb.AppendLine("    </dl>");
                sb.AppendLine("</div>");
            }

            if (method.XmlDoc.HasReturns)
            {
                sb.AppendLine("<div class=\"mt-2 small\">");
                sb.AppendLine("    <span class=\"text-body-secondary fw-bold\">Returns:</span>");
                sb.Append("<div class=\"ms-3\">")
                    .Append(method.XmlDoc.ReturnsHtml)
                    .AppendLine("</div></div>");
            }

            if (method.XmlDoc.HasExceptions)
            {
                sb.AppendLine("<div class=\"mt-2 small\">");
                sb.AppendLine("    <div class=\"text-body-secondary fw-bold mb-1\">Exceptions:</div>");
                sb.AppendLine("    <dl class=\"row mb-0 ms-1\">");
                foreach (var item in method.XmlDoc.Exceptions)
                {
                    sb.Append("        <dt class=\"col-sm-3\"><code>").Append(Html(item.Name)).AppendLine("</code></dt>");
                    sb.Append("        <dd class=\"col-sm-9\">").Append(item.Html).AppendLine("</dd>");
                }

                sb.AppendLine("    </dl>");
                sb.AppendLine("</div>");
            }

            if (method.XmlDoc.HasExample)
            {
                sb.AppendLine("<div class=\"mt-2 small\">");
                sb.AppendLine("    <span class=\"text-body-secondary fw-bold\">Example:</span>");
                sb.Append("<div class=\"ms-3\">")
                    .Append(method.XmlDoc.ExampleHtml)
                    .AppendLine("</div></div>");
            }

            if (method.XmlDoc.HasSeeAlsos)
            {
                sb.AppendLine("<div class=\"mt-2 small\">");
                sb.AppendLine("    <span class=\"text-body-secondary fw-bold\">See Also:</span>");
                sb.AppendLine("    <ul class=\"list-unstyled ms-3 mb-0\">");

                foreach (DocumentationXmlLinkItem link in method.XmlDoc.SeeAlsos)
                {
                    sb.Append("        <li>")
                        .Append(RenderSeeAlsoItem(link))
                        .AppendLine("</li>");
                }

                sb.AppendLine("    </ul></div>");
            }

            return sb.ToString();
        }

        private static string RenderNamedItemsList(IEnumerable<DocumentationXmlNamedItem> items)
        {
            StringBuilder sb = new();

            sb.AppendLine("<ul class=\"list-group list-group-flush\">");

            foreach (DocumentationXmlNamedItem item in items)
            {
                sb.AppendLine("    <li class=\"list-group-item px-0\">");
                sb.Append("        <div><code>")
                    .Append(Html(item.Name))
                    .AppendLine("</code></div>");
                sb.Append("        <div class=\"text-body-secondary mt-2\">")
                    .Append(item.Html)
                    .AppendLine("</div>");
                sb.AppendLine("    </li>");
            }

            sb.AppendLine("</ul>");

            return sb.ToString();
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
            IEnumerable<DocumentationInterfacePageModel> items,
            string pageOutputDirectory,
            string sharedDocumentationRootDirectory
        )
        {
            Directory.CreateDirectory(sharedDocumentationRootDirectory);

            foreach (DocumentationInterfacePageModel item in items)
            {
                DocumentationPartialGenerator.EnsurePartials(
                    sharedDocumentationRootDirectory,
                    item.NamespaceName,
                    item.InterfaceName);
            }
        }

        private static string RenderPropertyBody(DocumentationInterfacePropertyItem property)
        {
            StringBuilder sb = new();

            sb.AppendLine(DocumentationVisualHelper.RenderMemberSignature(
                RenderMemberIcons(
                    property.Accessibility,
                    isStatic: property.IsStatic,
                    isReadOnly: property.IsReadOnly,
                    isAbstract: property.IsAbstract,
                    isVirtual: property.IsVirtual,
                    isOverride: property.IsOverride,
                    isSealed: property.IsSealed),
                property.Signature,
                property.IsObsolete));

            if (property.IsObsolete)
            {
                sb.Append("<div class=\"text-danger fw-bold small mb-1\"><i class=\"bi bi-exclamation-triangle-fill me-1\"></i>Obsolete</div>");
                if (!string.IsNullOrWhiteSpace(property.ObsoleteMessage)) sb.Append("<div class=\"text-danger small mb-2\">").Append(Html(property.ObsoleteMessage)).Append("</div>");
            }

            if (property.XmlDoc.HasSummary)
            {
                sb.Append("<div class=\"text-body-secondary mt-2\">")
                    .Append(property.XmlDoc.SummaryHtml)
                    .AppendLine("</div>");
            }

            if (property.XmlDoc.HasValue)
            {
                sb.AppendLine("<div class=\"mt-2 small\">");
                sb.AppendLine("    <span class=\"text-body-secondary fw-bold\">Value:</span>");
                sb.Append("    <span>")
                    .Append(property.XmlDoc.ValueHtml)
                    .AppendLine("</span></div>");
            }

            if (property.XmlDoc.HasRemarks)
            {
                sb.Append("<div class=\"mt-2 small\">")
                    .AppendLine("<span class=\"text-body-secondary fw-bold\">Remarks:</span>")
                    .Append("<div class=\"ms-3\">")
                    .Append(property.XmlDoc.RemarksHtml)
                    .AppendLine("</div></div>");
            }

            if (property.XmlDoc.HasExample)
            {
                sb.AppendLine("<div class=\"mt-2 small\">");
                sb.AppendLine("    <span class=\"text-body-secondary fw-bold\">Example:</span>");
                sb.Append("<div class=\"ms-3\">")
                    .Append(property.XmlDoc.ExampleHtml)
                    .AppendLine("</div></div>");
            }

            if (property.XmlDoc.HasSeeAlsos)
            {
                sb.AppendLine("<div class=\"mt-2 small\">");
                sb.AppendLine("    <span class=\"text-body-secondary fw-bold\">See Also:</span>");
                sb.AppendLine("    <ul class=\"list-unstyled ms-3 mb-0\">");

                foreach (DocumentationXmlLinkItem link in property.XmlDoc.SeeAlsos)
                {
                    sb.Append("        <li>")
                        .Append(RenderSeeAlsoItem(link))
                        .AppendLine("</li>");
                }

                sb.AppendLine("    </ul></div>");
            }

            return sb.ToString();
        }

        private static string RenderSeeAlsoItem(DocumentationXmlLinkItem item)
        {
            if (item.IsKeyword) return $"<code>{Html(item.Label)}</code>";

            if (string.IsNullOrWhiteSpace(item.Href)) return $"<code>{Html(item.Label)}</code>";

            return $"<a href=\"{item.Href}\"><code>{Html(item.Label)}</code></a>";
        }

        #endregion
    }
}
