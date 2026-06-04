#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationClassPageRenderer type used by DocumentationBuilder generation.
    /// </summary>
    public static class DocumentationClassPageRenderer
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
            DocumentationClassPageModel item,
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
                item.ClassName,
                "_Before_Page.html"));

            sb.AppendLine("            <div class=\"mb-4\">");

            sb.AppendLine(ReadPartialContent(
                sharedDocumentationRootDirectory,
                item.NamespaceName,
                item.ClassName,
                "_After_Header.html"));

            sb.Append("                <h1 class=\"display-6 fw-bold mb-3\">")
                .Append(Html(item.ClassName))
                .AppendLine("</h1>");

            if (item.IsObsolete)
            {
                sb.AppendLine("                <div class=\"alert alert-danger mb-4 shadow-sm\">");
                sb.AppendLine("                    <h2 class=\"alert-heading fw-bold mb-1\"><i class=\"bi bi-exclamation-triangle-fill me-2\"></i>Obsolete</h2>");
                sb.Append("                    <div class=\"small\">")
                    .Append(string.IsNullOrWhiteSpace(item.ObsoleteMessage) ? "This class is obsolete." : Html(item.ObsoleteMessage))
                    .AppendLine("</div>");
                sb.AppendLine("                </div>");
            }

            if (!string.IsNullOrWhiteSpace(item.SummaryHtml))
            {
                sb.Append("                <p class=\"lead text-body-secondary mb-4\">")
                    .Append(item.SummaryHtml)
                    .AppendLine("</p>");
            }

            if (!string.IsNullOrWhiteSpace(item.RemarksHtml))
            {
                sb.AppendLine("                <div class=\"mb-4\">");
                sb.AppendLine("                    <h2 class=\"text-body-secondary\">Remarks</h2>");
                sb.Append("                    <div>").Append(item.RemarksHtml).AppendLine("</div>");
                sb.AppendLine("                </div>");
            }

            if (!string.IsNullOrWhiteSpace(item.ExampleHtml))
            {
                sb.AppendLine("                <div class=\"mb-4\">");
                sb.AppendLine("                    <h2 class=\"text-body-secondary\">Example</h2>");
                sb.Append("                    <div>").Append(item.ExampleHtml).AppendLine("</div>");
                sb.AppendLine("                </div>");
            }

            if (item.SeeAlsos.Count > 0)
            {
                sb.AppendLine("                <div class=\"mb-4\">");
                sb.AppendLine("                    <h2 class=\"text-body-secondary\">See Also</h2>");
                sb.AppendLine("                    <ul class=\"list-unstyled\">");

                foreach (DocumentationXmlLinkItem link in item.SeeAlsos)
                {
                    sb.Append("                        <li>");

                    if (link.IsKeyword)
                    {
                        sb.Append("<code>").Append(Html(link.Label)).Append("</code>");
                    }
                    else if (!string.IsNullOrWhiteSpace(link.Href))
                    {
                        sb.Append("<a href=\"").Append(link.Href).Append("\"><code>").Append(Html(link.Label)).Append("</code></a>");
                    }
                    else
                    {
                        sb.Append("<code>").Append(Html(link.Label)).Append("</code>");
                    }

                    sb.AppendLine("</li>");
                }

                sb.AppendLine("                    </ul>");
                sb.AppendLine("                </div>");
            }

            sb.AppendLine(ReadPartialContent(
                sharedDocumentationRootDirectory,
                item.NamespaceName,
                item.ClassName,
                "_After_Introduction.html"));

            sb.AppendLine("                <section id=\"metadata\" class=\"card border-0 shadow-sm mb-3\">");
            sb.AppendLine("                    <div class=\"card-body p-2\">");
            sb.AppendLine("                        <h2>Metadata</h2>");
            sb.AppendLine("                        <dl class=\"row mb-0\">");

            sb.AppendLine("                            <dt class=\"col-sm-3\">Kind</dt>");
            sb.AppendLine("                            <dd class=\"col-sm-9\"><code>Class</code></dd>");

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

            sb.AppendLine("                            <dt class=\"col-sm-3\">Static</dt>");
            sb.Append("                            <dd class=\"col-sm-9\"><code>")
                .Append(item.IsStatic ? "Yes" : "No")
                .AppendLine("</code></dd>");

            sb.AppendLine("                            <dt class=\"col-sm-3\">Abstract</dt>");
            sb.Append("                            <dd class=\"col-sm-9\"><code>")
                .Append(item.IsAbstract ? "Yes" : "No")
                .AppendLine("</code></dd>");

            sb.AppendLine("                            <dt class=\"col-sm-3\">Sealed</dt>");
            sb.Append("                            <dd class=\"col-sm-9\"><code>")
                .Append(item.IsSealed ? "Yes" : "No")
                .AppendLine("</code></dd>");

            if (item.BaseType is not null)
            {
                sb.AppendLine("                            <dt class=\"col-sm-3\">Base Type</dt>");
                sb.Append("                            <dd class=\"col-sm-9\">");

                if (item.BaseType.IsDocumented)
                {
                    sb.Append("<a href=\"/Documentation/Show?packageId=")
                        .Append(EscapeCSharp(ResolveLinkPackageId(item, item.BaseType)))
                        .Append("&version=")
                        .Append(EscapeCSharp(ResolveLinkVersion(item, item.BaseType)))
                        .Append("&groupName=")
                        .Append(EscapeCSharp(ResolveLinkGroupName(item, item.BaseType)))
                        .Append("&namespaceName=")
                        .Append(EscapeCSharp(item.BaseType.NamespaceName))
                        .Append("&objectName=")
                        .Append(EscapeCSharp(item.BaseType.ObjectName))
                        .Append("\">")
                        .Append(Html(item.BaseType.DisplayName))
                        .Append("</a>");
                }
                else
                {
                    sb.Append(Html(item.BaseType.DisplayName));
                }

                sb.AppendLine("</dd>");
            }

            if (item.ImplementedInterfaces.Count > 0)
            {
                sb.AppendLine("                            <dt class=\"col-sm-3\">Implements</dt>");
                sb.Append("                            <dd class=\"col-sm-9\">");

                for (int i = 0; i < item.ImplementedInterfaces.Count; i++)
                {
                    DocumentationTypeLinkItem implementedInterface = item.ImplementedInterfaces[i];

                    if (implementedInterface.IsDocumented)
                    {
                        sb.Append("<a href=\"/Documentation/Show?packageId=")
                            .Append(EscapeCSharp(ResolveLinkPackageId(item, implementedInterface)))
                            .Append("&version=")
                            .Append(EscapeCSharp(ResolveLinkVersion(item, implementedInterface)))
                            .Append("&groupName=")
                            .Append(EscapeCSharp(ResolveLinkGroupName(item, implementedInterface)))
                            .Append("&namespaceName=")
                            .Append(EscapeCSharp(implementedInterface.NamespaceName))
                            .Append("&objectName=")
                            .Append(EscapeCSharp(implementedInterface.ObjectName))
                            .Append("\">")
                            .Append(Html(implementedInterface.DisplayName))
                            .Append("</a>");
                    }
                    else
                    {
                        sb.Append(Html(implementedInterface.DisplayName));
                    }

                    if (i < item.ImplementedInterfaces.Count - 1) sb.Append(", ");
                }

                sb.AppendLine("</dd>");
            }

            sb.AppendLine("                        </dl>");
            sb.AppendLine("                    </div>");
            sb.AppendLine(ReadPartialContent(
                sharedDocumentationRootDirectory,
                item.NamespaceName,
                item.ClassName,
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
                item.ClassName,
                "_After_Declaration.html"));
            sb.AppendLine("                </section>");

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
                    item.ClassName,
                    "_After_ExtensionMethods.html"));
                sb.AppendLine("                </section>");
            }

            if (item.Constructors.Count > 0)
            {
                sb.Append("                <section id=\"constructors\" class=\"card border-0 shadow-sm mt-3\" ")
                    .Append(DocumentationDisplayFilterHtml.SectionAttributes("constructor"))
                    .AppendLine(">");
                sb.AppendLine("                    <div class=\"card-body p-2\">");
                sb.AppendLine("                        <h2>Constructors</h2>");
                sb.AppendLine("                        <ul class=\"list-group list-group-flush\">");

                foreach (DocumentationClassConstructorItem constructor in item.Constructors)
                {
                    sb.Append("                            <li class=\"list-group-item px-0 py-3 my-2\" ")
                        .Append(DocumentationDisplayFilterHtml.MemberAttributes("constructor", constructor.Accessibility))
                        .Append(" id=\"")
                        .Append(Html(constructor.ConstructorName))
                        .AppendLine("\">");
                    sb.Append("                                ")
                        .Append(DocumentationVisualHelper.RenderMemberSignature(
                            RenderMemberIcons(constructor.Accessibility),
                            constructor.Signature,
                            constructor.IsObsolete))
                        .AppendLine();

                    if (constructor.IsObsolete)
                    {
                        sb.Append("                                <div class=\"text-danger fw-bold small mb-1\"><i class=\"bi bi-exclamation-triangle-fill me-1\"></i>Obsolete</div>");
                        if (!string.IsNullOrWhiteSpace(constructor.ObsoleteMessage)) sb.Append("                                <div class=\"text-danger small mb-2\">").Append(Html(constructor.ObsoleteMessage)).AppendLine("</div>");
                    }

                    if (!string.IsNullOrWhiteSpace(constructor.SummaryHtml))
                    {
                        sb.Append("                                <div class=\"text-body-secondary mt-2\">")
                            .Append(constructor.SummaryHtml)
                            .AppendLine("</div>");
                    }

                    if (constructor.Parameters.Count > 0)
                    {
                        sb.AppendLine("                                <div class=\"mt-2 small\">");
                        sb.AppendLine("                                    <div class=\"text-body-secondary fw-bold mb-1\">Parameters:</div>");
                        sb.AppendLine("                                    <dl class=\"row mb-0 ms-1\">");

                        foreach (DocumentationXmlNamedItem parameter in constructor.Parameters)
                        {
                            sb.Append("                                        <dt class=\"col-sm-3\"><code>")
                                .Append(Html(parameter.Name))
                                .AppendLine("</code></dt>");
                            sb.Append("                                        <dd class=\"col-sm-9\">")
                                .Append(parameter.Html)
                                .AppendLine("</dd>");
                        }

                        sb.AppendLine("                                    </dl>");
                        sb.AppendLine("                                </div>");
                    }

                    if (constructor.Exceptions.Count > 0)
                    {
                        sb.AppendLine("                                <div class=\"mt-2 small\">");
                        sb.AppendLine("                                    <div class=\"text-body-secondary fw-bold mb-1\">Exceptions:</div>");
                        sb.AppendLine("                                    <dl class=\"row mb-0 ms-1\">");

                        foreach (DocumentationXmlNamedItem exception in constructor.Exceptions)
                        {
                            sb.Append("                                        <dt class=\"col-sm-3\"><code>")
                                .Append(Html(exception.Name))
                                .AppendLine("</code></dt>");
                            sb.Append("                                        <dd class=\"col-sm-9\">")
                                .Append(exception.Html)
                                .AppendLine("</dd>");
                        }

                        sb.AppendLine("                                    </dl>");
                        sb.AppendLine("                                </div>");
                    }

                    if (!string.IsNullOrWhiteSpace(constructor.RemarksHtml))
                    {
                        sb.AppendLine("                                <div class=\"mt-2 small\">");
                        sb.AppendLine("                                    <span class=\"text-body-secondary fw-bold\">Remarks:</span>");
                        sb.Append("                                    <div class=\"ms-3\">").Append(constructor.RemarksHtml).AppendLine("</div>");
                        sb.AppendLine("                                </div>");
                    }

                    if (!string.IsNullOrWhiteSpace(constructor.ExampleHtml))
                    {
                        sb.AppendLine("                                <div class=\"mt-2 small\">");
                        sb.AppendLine("                                    <span class=\"text-body-secondary fw-bold\">Example:</span>");
                        sb.Append("                                    <div class=\"ms-3\">").Append(constructor.ExampleHtml).AppendLine("</div>");
                        sb.AppendLine("                                </div>");
                    }

                    sb.AppendLine("                            </li>");
                }

                sb.AppendLine("                        </ul>");
                sb.AppendLine("                    </div>");
                sb.AppendLine(ReadPartialContent(
                    sharedDocumentationRootDirectory,
                    item.NamespaceName,
                    item.ClassName,
                    "_After_Constructors.html"));
                sb.AppendLine("                </section>");
            }

            if (item.Fields.Count > 0)
            {
                sb.Append("                <section id=\"fields\" class=\"card border-0 shadow-sm mt-3\" ")
                    .Append(DocumentationDisplayFilterHtml.SectionAttributes("field"))
                    .AppendLine(">");
                sb.AppendLine("                    <div class=\"card-body p-2\">");
                sb.AppendLine("                        <h2>Fields</h2>");

                var groups = item.Fields
                    .GroupBy(f => f.IsStatic || f.IsConst ? "Static & Const Fields" : "Instance Fields")
                    .OrderByDescending(g => g.Key);

                foreach (var group in groups)
                {
                    sb.Append("                        <h3 class=\"mt-3 mb-2 ms-2\" data-doc-member-group-heading=\"true\">").Append(group.Key).AppendLine("</h3>");

                    var accessibilityGroups = group
                        .GroupBy(f => f.Accessibility)
                        .OrderBy(g => GetAccessibilityOrder(g.Key));

                    foreach (var accGroup in accessibilityGroups)
                    {
                        sb.Append("                        <h4 class=\"mt-2 mb-2 ms-3 text-body-secondary\" data-doc-access-heading=\"true\">").Append(accGroup.Key).AppendLine("</h4>");
                        sb.AppendLine("                        <ul class=\"list-group list-group-flush\">");

                        foreach (DocumentationClassFieldItem field in accGroup.OrderBy(f => f.FieldName))
                        {
                            sb.Append("                            <li class=\"list-group-item px-0 py-3 my-2 ms-4\" ")
                                .Append(DocumentationDisplayFilterHtml.MemberAttributes("field", field.Accessibility))
                                .Append(" id=\"")
                                .Append(Html(field.FieldName))
                                .AppendLine("\">");
                            sb.Append("                                ")
                                .Append(DocumentationVisualHelper.RenderMemberSignature(
                                    RenderMemberIcons(
                                        field.Accessibility,
                                        isStatic: field.IsStatic,
                                        isConst: field.IsConst,
                                        isReadOnly: field.IsReadOnly),
                                    field.Signature,
                                    field.IsObsolete))
                                .AppendLine();

                            if (field.IsObsolete)
                            {
                                sb.Append("                                <div class=\"text-danger fw-bold small mb-1\"><i class=\"bi bi-exclamation-triangle-fill me-1\"></i>Obsolete</div>");
                                if (!string.IsNullOrWhiteSpace(field.ObsoleteMessage)) sb.Append("                                <div class=\"text-danger small mb-2\">").Append(Html(field.ObsoleteMessage)).AppendLine("</div>");
                            }

                            if (!string.IsNullOrWhiteSpace(field.SummaryHtml))
                            {
                                sb.Append("                                <div class=\"text-body-secondary mt-2\">")
                                    .Append(field.SummaryHtml)
                                    .AppendLine("</div>");
                            }

                            if (!string.IsNullOrWhiteSpace(field.RemarksHtml))
                            {
                                sb.AppendLine("                                <div class=\"mt-2 small\">");
                                sb.AppendLine("                                    <span class=\"text-body-secondary fw-bold\">Remarks:</span>");
                                sb.Append("                                    <div class=\"ms-3\">").Append(field.RemarksHtml).AppendLine("</div>");
                                sb.AppendLine("                                </div>");
                            }

                            if (!string.IsNullOrWhiteSpace(field.ExampleHtml))
                            {
                                sb.AppendLine("                                <div class=\"mt-2 small\">");
                                sb.AppendLine("                                    <span class=\"text-body-secondary fw-bold\">Example:</span>");
                                sb.Append("                                    <div class=\"ms-3\">").Append(field.ExampleHtml).AppendLine("</div>");
                                sb.AppendLine("                                </div>");
                            }

                            sb.AppendLine("                            </li>");
                        }

                        sb.AppendLine("                        </ul>");
                    }
                }

                sb.AppendLine("                    </div>");
                sb.AppendLine(ReadPartialContent(
                    sharedDocumentationRootDirectory,
                    item.NamespaceName,
                    item.ClassName,
                    "_After_Fields.html"));
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

                    var accessibilityGroups = group
                        .GroupBy(p => p.Accessibility)
                        .OrderBy(g => GetAccessibilityOrder(g.Key));

                    foreach (var accGroup in accessibilityGroups)
                    {
                        sb.Append("                        <h4 class=\"mt-2 mb-2 ms-3 text-body-secondary\" data-doc-access-heading=\"true\">").Append(accGroup.Key).AppendLine("</h4>");
                        sb.AppendLine("                        <ul class=\"list-group list-group-flush\">");

                        foreach (DocumentationClassPropertyItem property in accGroup.OrderBy(p => p.PropertyName))
                        {
                            sb.Append("                            <li class=\"list-group-item px-0 py-3 my-2 ms-4\" ")
                                .Append(DocumentationDisplayFilterHtml.MemberAttributes("property", property.Accessibility))
                                .Append(" id=\"")
                                .Append(Html(property.PropertyName))
                                .AppendLine("\">");
                            sb.Append("                                ")
                                .Append(DocumentationVisualHelper.RenderMemberSignature(
                                    RenderMemberIcons(
                                        property.Accessibility,
                                        isStatic: property.IsStatic,
                                        isReadOnly: property.IsReadOnly,
                                        isVirtual: property.IsVirtual,
                                        isOverride: property.IsOverride,
                                        isSealed: property.IsSealed),
                                    property.Signature,
                                    property.IsObsolete))
                                .AppendLine();

                            if (property.IsObsolete)
                            {
                                sb.Append("                                <div class=\"text-danger fw-bold small mb-1\"><i class=\"bi bi-exclamation-triangle-fill me-1\"></i>Obsolete</div>");
                                if (!string.IsNullOrWhiteSpace(property.ObsoleteMessage)) sb.Append("                                <div class=\"text-danger small mb-2\">").Append(Html(property.ObsoleteMessage)).AppendLine("</div>");
                            }

                            if (!string.IsNullOrWhiteSpace(property.SummaryHtml))
                            {
                                sb.Append("                                <div class=\"text-body-secondary mt-2\">")
                                    .Append(property.SummaryHtml)
                                    .AppendLine("</div>");
                            }

                            if (!string.IsNullOrWhiteSpace(property.ValueHtml))
                            {
                                sb.AppendLine("                                <div class=\"mt-2 small\">");
                                sb.AppendLine("                                    <span class=\"text-body-secondary fw-bold\">Value:</span>");
                                sb.Append("                                    <span>").Append(property.ValueHtml).AppendLine("</span>");
                                sb.AppendLine("                                </div>");
                            }

                            if (!string.IsNullOrWhiteSpace(property.RemarksHtml))
                            {
                                sb.AppendLine("                                <div class=\"mt-2 small\">");
                                sb.AppendLine("                                    <span class=\"text-body-secondary fw-bold\">Remarks:</span>");
                                sb.Append("                                    <div class=\"ms-3\">").Append(property.RemarksHtml).AppendLine("</div>");
                                sb.AppendLine("                                </div>");
                            }

                            if (!string.IsNullOrWhiteSpace(property.ExampleHtml))
                            {
                                sb.AppendLine("                                <div class=\"mt-2 small\">");
                                sb.AppendLine("                                    <span class=\"text-body-secondary fw-bold\">Example:</span>");
                                sb.Append("                                    <div class=\"ms-3\">").Append(property.ExampleHtml).AppendLine("</div>");
                                sb.AppendLine("                                </div>");
                            }

                            sb.AppendLine("                            </li>");
                        }

                        sb.AppendLine("                        </ul>");
                    }
                }

                sb.AppendLine("                    </div>");
                sb.AppendLine(ReadPartialContent(
                    sharedDocumentationRootDirectory,
                    item.NamespaceName,
                    item.ClassName,
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

                    var accessibilityGroups = group
                        .GroupBy(m => m.Accessibility)
                        .OrderBy(g => GetAccessibilityOrder(g.Key));

                    foreach (var accGroup in accessibilityGroups)
                    {
                        sb.Append("                        <h4 class=\"mt-2 mb-2 ms-3 text-body-secondary\" data-doc-access-heading=\"true\">").Append(accGroup.Key).AppendLine("</h4>");
                        sb.AppendLine("                        <ul class=\"list-group list-group-flush\">");

                        foreach (DocumentationClassMethodItem method in accGroup.OrderBy(m => m.MethodName))
                        {
                            sb.Append("                            <li class=\"list-group-item px-0 py-3 my-2 ms-4\" ")
                                .Append(DocumentationDisplayFilterHtml.MemberAttributes("method", method.Accessibility))
                                .Append(" id=\"")
                                .Append(Html(method.MethodName))
                                .AppendLine("\">");
                            sb.Append("                                ")
                                .Append(DocumentationVisualHelper.RenderMemberSignature(
                                    RenderMemberIcons(
                                        method.Accessibility,
                                        isStatic: method.IsStatic,
                                        isAbstract: method.IsAbstract,
                                        isVirtual: method.IsVirtual,
                                        isOverride: method.IsOverride,
                                        isSealed: method.IsSealed),
                                    method.Signature,
                                    method.IsObsolete))
                                .AppendLine();

                            if (method.IsObsolete)
                            {
                                sb.Append("                                <div class=\"text-danger fw-bold small mb-1\"><i class=\"bi bi-exclamation-triangle-fill me-1\"></i>Obsolete</div>");
                                if (!string.IsNullOrWhiteSpace(method.ObsoleteMessage)) sb.Append("                                <div class=\"text-danger small mb-2\">").Append(Html(method.ObsoleteMessage)).AppendLine("</div>");
                            }

                            if (!string.IsNullOrWhiteSpace(method.SummaryHtml))
                            {
                                sb.Append("                                <div class=\"text-body-secondary mt-2\">")
                                    .Append(method.SummaryHtml)
                                    .AppendLine("</div>");
                            }

                            if (method.Parameters.Count > 0)
                            {
                                sb.AppendLine("                                <div class=\"mt-2 small\">");
                                sb.AppendLine("                                    <div class=\"text-body-secondary fw-bold mb-1\">Parameters:</div>");
                                sb.AppendLine("                                    <dl class=\"row mb-0 ms-1\">");

                                foreach (DocumentationXmlNamedItem parameter in method.Parameters)
                                {
                                    sb.Append("                                        <dt class=\"col-sm-3\"><code>")
                                        .Append(Html(parameter.Name))
                                        .AppendLine("</code></dt>");
                                    sb.Append("                                        <dd class=\"col-sm-9\">")
                                        .Append(parameter.Html)
                                        .AppendLine("</dd>");
                                }

                                sb.AppendLine("                                    </dl>");
                                sb.AppendLine("                                </div>");
                            }

                            if (!string.IsNullOrWhiteSpace(method.ReturnsHtml))
                            {
                                sb.AppendLine("                                <div class=\"mt-2 small\">");
                                sb.AppendLine("                                    <span class=\"text-body-secondary fw-bold\">Returns:</span>");
                                sb.Append("                                    <div class=\"ms-3\">").Append(method.ReturnsHtml).AppendLine("</div>");
                                sb.AppendLine("                                </div>");
                            }

                            if (method.Exceptions.Count > 0)
                            {
                                sb.AppendLine("                                <div class=\"mt-2 small\">");
                                sb.AppendLine("                                    <div class=\"text-body-secondary fw-bold mb-1\">Exceptions:</div>");
                                sb.AppendLine("                                    <dl class=\"row mb-0 ms-1\">");

                                foreach (DocumentationXmlNamedItem exception in method.Exceptions)
                                {
                                    sb.Append("                                        <dt class=\"col-sm-3\"><code>")
                                        .Append(Html(exception.Name))
                                        .AppendLine("</code></dt>");
                                    sb.Append("                                        <dd class=\"col-sm-9\">")
                                        .Append(exception.Html)
                                        .AppendLine("</dd>");
                                }

                                sb.AppendLine("                                    </dl>");
                                sb.AppendLine("                                </div>");
                            }

                            if (!string.IsNullOrWhiteSpace(method.RemarksHtml))
                            {
                                sb.AppendLine("                                <div class=\"mt-2 small\">");
                                sb.AppendLine("                                    <span class=\"text-body-secondary fw-bold\">Remarks:</span>");
                                sb.Append("                                    <div class=\"ms-3\">").Append(method.RemarksHtml).AppendLine("</div>");
                                sb.AppendLine("                                </div>");
                            }

                            if (!string.IsNullOrWhiteSpace(method.ExampleHtml))
                            {
                                sb.AppendLine("                                <div class=\"mt-2 small\">");
                                sb.AppendLine("                                    <span class=\"text-body-secondary fw-bold\">Example:</span>");
                                sb.Append("                                    <div class=\"ms-3\">").Append(method.ExampleHtml).AppendLine("</div>");
                                sb.AppendLine("                                </div>");
                            }

                            sb.AppendLine("                            </li>");
                        }

                        sb.AppendLine("                        </ul>");
                    }
                }

                sb.AppendLine("                    </div>");
                sb.AppendLine(ReadPartialContent(
                    sharedDocumentationRootDirectory,
                    item.NamespaceName,
                    item.ClassName,
                    "_After_Methods.html"));
                sb.AppendLine("                </section>");
            }

            sb.AppendLine(ReadPartialContent(
                sharedDocumentationRootDirectory,
                item.NamespaceName,
                item.ClassName,
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
            bool isConst = false,
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

            if (isConst) sb.Append(DocumentationVisualHelper.ConstIconHtml);

            if (isReadOnly) sb.Append(DocumentationVisualHelper.ReadOnlyIconHtml);

            if (isAbstract) sb.Append(DocumentationVisualHelper.AbstractIconHtml);

            if (isVirtual) sb.Append(DocumentationVisualHelper.VirtualIconHtml);

            if (isOverride) sb.Append(DocumentationVisualHelper.OverrideIconHtml);

            if (isSealed) sb.Append(DocumentationVisualHelper.SealedIconHtml);

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
            IEnumerable<DocumentationClassPageModel> items,
            string pageOutputDirectory,
            string sharedDocumentationRootDirectory
        )
        {
            Directory.CreateDirectory(sharedDocumentationRootDirectory);

            foreach (DocumentationClassPageModel item in items)
            {
                DocumentationPartialGenerator.EnsurePartials(
                    sharedDocumentationRootDirectory,
                    item.NamespaceName,
                    item.ClassName);
            }
        }

        private static string ResolveLinkGroupName(DocumentationClassPageModel item, DocumentationTypeLinkItem target)
        {
            return string.IsNullOrWhiteSpace(target.GroupName) ? item.GroupName : target.GroupName;
        }

        private static string ResolveLinkPackageId(DocumentationClassPageModel item, DocumentationTypeLinkItem target)
        {
            return string.IsNullOrWhiteSpace(target.PackageId) ? item.PackageId : target.PackageId;
        }

        private static string ResolveLinkVersion(DocumentationClassPageModel item, DocumentationTypeLinkItem target)
        {
            return string.IsNullOrWhiteSpace(target.Version) ? item.Version : target.Version;
        }

        #endregion
    }
}