#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

#endregion

namespace DMBDocumentationBuilder
{
    internal static class DocumentationXmlCommentRenderer
    {
        #region Static methods

        private static string BuildControllerHref(
            string packageId,
            string version,
            string groupName,
            string namespaceName,
            string objectName,
            string? anchor
        )
        {
            string baseHref = $"/Documentation/Show?packageId={System.Net.WebUtility.UrlEncode(packageId)}&version={System.Net.WebUtility.UrlEncode(version)}&groupName={System.Net.WebUtility.UrlEncode(groupName)}&namespaceName={System.Net.WebUtility.UrlEncode(namespaceName)}&objectName={System.Net.WebUtility.UrlEncode(objectName)}";

            if (string.IsNullOrWhiteSpace(anchor)) return baseHref;

            return $"{baseHref}#{System.Net.WebUtility.UrlEncode(anchor)}";
        }

        private static string EscapeRazorString(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        /// <summary>
        ///     Extracts structured documentation metadata from XML comments or project context inputs.
        /// </summary>
        /// <param name="packageId">The packageId value used by the documentation generation operation.</param>
        /// <param name="version">The version value used by the documentation generation operation.</param>
        /// <param name="groupName">The groupName value used by the documentation generation operation.</param>
        /// <param name="symbol">The symbol value used by the documentation generation operation.</param>
        /// <param name="compilation">The compilation value used by the documentation generation operation.</param>
        /// <param name="currentTypeSymbol">The currentTypeSymbol value used by the documentation generation operation.</param>
        /// <returns>The Extract result produced by DocumentationBuilder generation.</returns>
        public static DocumentationXmlModel Extract(
            string packageId,
            string version,
            string groupName,
            ISymbol symbol,
            Compilation compilation,
            INamedTypeSymbol currentTypeSymbol
        )
        {
            DocumentationXmlModel model = new();

            XDocument? document = TryGetDocumentationDocument(symbol);
            if (document?.Root is null) return model;

            XElement root = document.Root;

            model = new DocumentationXmlModel
            {
                SummaryHtml = ExtractSingleSectionHtml(packageId, version, groupName, root, "summary", compilation, currentTypeSymbol),
                RemarksHtml = ExtractSingleSectionHtml(packageId, version, groupName, root, "remarks", compilation, currentTypeSymbol),
                ExampleHtml = ExtractSingleSectionHtml(packageId, version, groupName, root, "example", compilation, currentTypeSymbol),
                ReturnsHtml = ExtractSingleSectionHtml(packageId, version, groupName, root, "returns", compilation, currentTypeSymbol),
                ValueHtml = ExtractSingleSectionHtml(packageId, version, groupName, root, "value", compilation, currentTypeSymbol)
            };

            foreach (DocumentationXmlNamedItem item in ExtractNamedItems(packageId, version, groupName, root, "param", "name", compilation, currentTypeSymbol)) model.Parameters.Add(item);

            foreach (DocumentationXmlNamedItem item in ExtractNamedItems(packageId, version, groupName, root, "typeparam", "name", compilation, currentTypeSymbol)) model.TypeParameters.Add(item);

            foreach (DocumentationXmlNamedItem item in ExtractNamedItems(packageId, version, groupName, root, "exception", "cref", compilation, currentTypeSymbol)) model.Exceptions.Add(item);

            foreach (DocumentationXmlLinkItem item in ExtractSeeAlsoItems(packageId, version, groupName, root, compilation, currentTypeSymbol)) model.SeeAlsos.Add(item);

            return model;
        }

        private static string ExtractInnerHtml(
            string packageId,
            string version,
            string groupName,
            XElement element,
            Compilation compilation,
            INamedTypeSymbol currentTypeSymbol
        )
        {
            return NormalizeHtmlSpacing(RenderNodes(packageId, version, groupName, element.Nodes(), compilation, currentTypeSymbol).Trim());
        }

        private static List<DocumentationXmlNamedItem> ExtractNamedItems(
            string packageId,
            string version,
            string groupName,
            XElement root,
            string elementName,
            string keyAttributeName,
            Compilation compilation,
            INamedTypeSymbol currentTypeSymbol
        )
        {
            List<DocumentationXmlNamedItem> result = [];

            foreach (XElement element in root.Elements(elementName))
            {
                string? key = element.Attribute(keyAttributeName)?.Value;
                if (string.IsNullOrWhiteSpace(key)) continue;

                string html = ExtractInnerHtml(packageId, version, groupName, element, compilation, currentTypeSymbol);
                if (string.IsNullOrWhiteSpace(html)) continue;

                result.Add(new DocumentationXmlNamedItem
                {
                    Name = key,
                    Html = html
                });
            }

            return result;
        }

        private static List<DocumentationXmlLinkItem> ExtractSeeAlsoItems(
            string packageId,
            string version,
            string groupName,
            XElement root,
            Compilation compilation,
            INamedTypeSymbol currentTypeSymbol
        )
        {
            List<DocumentationXmlLinkItem> result = [];

            foreach (XElement element in root.Elements("seealso"))
            {
                DocumentationXmlLinkItem? item = RenderSeeAlsoItem(packageId, version, groupName, element, compilation, currentTypeSymbol);
                if (item is not null) result.Add(item);
            }

            return result;
        }

        private static string ExtractSingleSectionHtml(
            string packageId,
            string version,
            string groupName,
            XElement root,
            string elementName,
            Compilation compilation,
            INamedTypeSymbol currentTypeSymbol
        )
        {
            XElement? element = root.Element(elementName);
            if (element is null) return string.Empty;

            return ExtractInnerHtml(packageId, version, groupName, element, compilation, currentTypeSymbol);
        }

        private static string Html(string? value)
        {
            return System.Net.WebUtility.HtmlEncode(value ?? string.Empty);
        }

        private static string HtmlAttribute(string? value)
        {
            return System.Net.WebUtility.HtmlEncode(value ?? string.Empty);
        }

        private static string NormalizeCrefTypeName(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName)) return string.Empty;

            string withoutGenericArguments = StripGenericArguments(typeName);

            string[] parts = withoutGenericArguments.Split('.');
            for (int i = 0; i < parts.Length; i++)
            {
                int genericTickIndex = parts[i].IndexOf('`');
                if (genericTickIndex >= 0) parts[i] = parts[i][..genericTickIndex];
            }

            return string.Join(".", parts);
        }

        private static string NormalizeHtmlSpacing(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;

            string normalized = html
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("\t", " ");

            while (normalized.Contains("  ", StringComparison.Ordinal)) normalized = normalized.Replace("  ", " ", StringComparison.Ordinal);

            normalized = normalized.Replace(" </code>", "</code>", StringComparison.Ordinal);
            normalized = normalized.Replace("<code> ", "<code>", StringComparison.Ordinal);
            normalized = normalized.Replace(" </a>", "</a>", StringComparison.Ordinal);

            return normalized.Trim();
        }

        private static CrefTarget? ParseCref(string cref, Compilation compilation, INamedTypeSymbol currentTypeSymbol)
        {
            if (string.IsNullOrWhiteSpace(cref)) return null;

            if (cref.Length >= 3 && cref[1] == ':')
            {
                string kind = cref[..2];
                string target = cref[2..];

                return kind switch
                {
                    "T:" => ParseTypeTarget(target),
                    "F:" => ParseMemberTarget(target, currentTypeSymbol),
                    "P:" => ParseMemberTarget(target, currentTypeSymbol),
                    "M:" => ParseMethodTarget(target, currentTypeSymbol),
                    "E:" => ParseMemberTarget(target, currentTypeSymbol),
                    _ => null
                };
            }

            if (string.Equals(cref, "null", StringComparison.Ordinal) ||
                string.Equals(cref, "true", StringComparison.Ordinal) ||
                string.Equals(cref, "false", StringComparison.Ordinal))
            {
                return new CrefTarget
                {
                    NamespaceName = string.Empty,
                    ObjectName = string.Empty,
                    Anchor = null,
                    Label = cref,
                    IsIntraPage = false,
                    IsKeyword = true
                };
            }

            return null;
        }

        private static CrefTarget? ParseMemberTarget(string fullMemberName, INamedTypeSymbol currentTypeSymbol)
        {
            int lastDot = fullMemberName.LastIndexOf('.');
            if (lastDot <= 0 || lastDot >= fullMemberName.Length - 1) return null;

            string memberName = fullMemberName[(lastDot + 1)..];
            string typeFullName = fullMemberName[..lastDot];
            string cleanedTypeFullName = NormalizeCrefTypeName(typeFullName);

            int typeLastDot = cleanedTypeFullName.LastIndexOf('.');
            if (typeLastDot <= 0 || typeLastDot >= cleanedTypeFullName.Length - 1) return null;

            string namespaceName = cleanedTypeFullName[..typeLastDot];
            string objectName = cleanedTypeFullName[(typeLastDot + 1)..];

            bool isCurrentPage =
                currentTypeSymbol.ContainingNamespace?.ToDisplayString() == namespaceName &&
                currentTypeSymbol.Name == objectName;

            return new CrefTarget
            {
                NamespaceName = namespaceName,
                ObjectName = objectName,
                Anchor = memberName,
                Label = memberName,
                IsIntraPage = isCurrentPage,
                IsKeyword = false
            };
        }

        private static CrefTarget? ParseMethodTarget(string fullMethodName, INamedTypeSymbol currentTypeSymbol)
        {
            int parameterIndex = fullMethodName.IndexOf('(');
            string withoutParameters = parameterIndex >= 0
                ? fullMethodName[..parameterIndex]
                : fullMethodName;

            return ParseMemberTarget(withoutParameters, currentTypeSymbol);
        }

        private static CrefTarget? ParseTypeTarget(string fullTypeName)
        {
            string cleanedTypeName = NormalizeCrefTypeName(fullTypeName);

            int lastDot = cleanedTypeName.LastIndexOf('.');
            if (lastDot <= 0 || lastDot >= cleanedTypeName.Length - 1) return null;

            string namespaceName = cleanedTypeName[..lastDot];
            string objectName = cleanedTypeName[(lastDot + 1)..];

            return new CrefTarget
            {
                NamespaceName = namespaceName,
                ObjectName = objectName,
                Anchor = null,
                Label = objectName,
                IsIntraPage = false,
                IsKeyword = false
            };
        }

        private static string RenderCodeBlock(XElement element)
        {
            string code = element.Value
                .Replace("\r\n", "\n", StringComparison.Ordinal);
            //.Trim('\n', '\r');

            return $"<pre class=\"border rounded-3\"><code class=\"language-csharp\">{Html(code)}</code></pre>";
        }

        private static string RenderElement(
            string packageId,
            string version,
            string groupName,
            XElement element,
            Compilation compilation,
            INamedTypeSymbol currentTypeSymbol
        )
        {
            switch (element.Name.LocalName)
            {
                case "see":
                    return RenderSeeElement(packageId, version, groupName, element, compilation, currentTypeSymbol);

                case "paramref":
                {
                    string? name = element.Attribute("name")?.Value;
                    return string.IsNullOrWhiteSpace(name)
                        ? string.Empty
                        : $"<code>{Html(name)}</code>";
                }

                case "typeparamref":
                {
                    string? name = element.Attribute("name")?.Value;
                    return string.IsNullOrWhiteSpace(name)
                        ? string.Empty
                        : $"<code>{Html(name)}</code>";
                }

                case "c":
                    return $"<code>{Html(element.Value)}</code>";

                case "code":
                    return RenderCodeBlock(element);

                case "para":
                {
                    string inner = RenderNodes(packageId, version, groupName, element.Nodes(), compilation, currentTypeSymbol).Trim();
                    return string.IsNullOrWhiteSpace(inner) ? string.Empty : $"<p>{inner}</p>";
                }

                case "list":
                    return RenderListElement(packageId, version, groupName, element, compilation, currentTypeSymbol);

                case "item":
                    return RenderListItemElement(packageId, version, groupName, element, compilation, currentTypeSymbol);

                case "term":
                case "description":
                    return RenderNodes(packageId, version, groupName, element.Nodes(), compilation, currentTypeSymbol);

                case "br":
                    return "<br />";

                default:
                    return RenderNodes(packageId, version, groupName, element.Nodes(), compilation, currentTypeSymbol);
            }
        }

        /// <summary>
        ///     Renders the XML documentation example section as safe HTML.
        /// </summary>
        /// <param name="packageId">The packageId value used by the documentation generation operation.</param>
        /// <param name="version">The version value used by the documentation generation operation.</param>
        /// <param name="groupName">The groupName value used by the documentation generation operation.</param>
        /// <param name="symbol">The symbol value used by the documentation generation operation.</param>
        /// <param name="compilation">The compilation value used by the documentation generation operation.</param>
        /// <param name="currentTypeSymbol">The currentTypeSymbol value used by the documentation generation operation.</param>
        /// <returns>The RenderExampleHtml result produced by DocumentationBuilder generation.</returns>
        public static string? RenderExampleHtml(
            string packageId,
            string version,
            string groupName,
            ISymbol symbol,
            Compilation compilation,
            INamedTypeSymbol currentTypeSymbol
        )
        {
            string value = Extract(packageId, version, groupName, symbol, compilation, currentTypeSymbol).ExampleHtml;
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        /// <summary>
        ///     Renders XML documentation exception entries as named documentation items.
        /// </summary>
        /// <param name="packageId">The packageId value used by the documentation generation operation.</param>
        /// <param name="version">The version value used by the documentation generation operation.</param>
        /// <param name="groupName">The groupName value used by the documentation generation operation.</param>
        /// <param name="symbol">The symbol value used by the documentation generation operation.</param>
        /// <param name="compilation">The compilation value used by the documentation generation operation.</param>
        /// <param name="currentTypeSymbol">The currentTypeSymbol value used by the documentation generation operation.</param>
        /// <returns>The RenderExceptionItems result produced by DocumentationBuilder generation.</returns>
        public static List<DocumentationXmlNamedItem> RenderExceptionItems(
            string packageId,
            string version,
            string groupName,
            ISymbol symbol,
            Compilation compilation,
            INamedTypeSymbol currentTypeSymbol
        )
        {
            return Extract(packageId, version, groupName, symbol, compilation, currentTypeSymbol).Exceptions;
        }

        private static string RenderListElement(
            string packageId,
            string version,
            string groupName,
            XElement element,
            Compilation compilation,
            INamedTypeSymbol currentTypeSymbol
        )
        {
            string listType = element.Attribute("type")?.Value ?? "bullet";

            string tagName = listType switch
            {
                "number" => "ol",
                "table" => "dl",
                _ => "ul"
            };

            StringBuilder sb = new();

            sb.Append('<').Append(tagName).Append('>');

            foreach (XElement itemElement in element.Elements("item")) sb.Append(RenderListItemElement(packageId, version, groupName, itemElement, compilation, currentTypeSymbol, listType));

            sb.Append("</").Append(tagName).Append('>');

            return sb.ToString();
        }

        private static string RenderListItemElement(
            string packageId,
            string version,
            string groupName,
            XElement element,
            Compilation compilation,
            INamedTypeSymbol currentTypeSymbol
        )
        {
            return RenderListItemElement(packageId, version, groupName, element, compilation, currentTypeSymbol, "bullet");
        }

        private static string RenderListItemElement(
            string packageId,
            string version,
            string groupName,
            XElement element,
            Compilation compilation,
            INamedTypeSymbol currentTypeSymbol,
            string listType
        )
        {
            if (string.Equals(listType, "table", StringComparison.OrdinalIgnoreCase))
            {
                string term = element.Element("term") is XElement termElement
                    ? ExtractInnerHtml(packageId, version, groupName, termElement, compilation, currentTypeSymbol)
                    : string.Empty;

                string description = element.Element("description") is XElement descriptionElement_A
                    ? ExtractInnerHtml(packageId, version, groupName, descriptionElement_A, compilation, currentTypeSymbol)
                    : string.Empty;

                return $"<dt>{term}</dt><dd>{description}</dd>";
            }

            XElement? descriptionElement = element.Element("description");
            string inner = descriptionElement is not null
                ? ExtractInnerHtml(packageId, version, groupName, descriptionElement, compilation, currentTypeSymbol)
                : RenderNodes(packageId, version, groupName, element.Nodes(), compilation, currentTypeSymbol).Trim();

            return $"<li>{inner}</li>";
        }

        private static string RenderNodes(
            string packageId,
            string version,
            string groupName,
            IEnumerable<XNode> nodes,
            Compilation compilation,
            INamedTypeSymbol currentTypeSymbol
        )
        {
            StringBuilder sb = new();

            foreach (XNode node in nodes)
            {
                switch (node)
                {
                    case XText textNode:
                        sb.Append(Html(textNode.Value));
                    break;

                    case XElement element:
                        sb.Append(RenderElement(packageId, version, groupName, element, compilation, currentTypeSymbol));
                    break;
                }
            }

            return NormalizeHtmlSpacing(sb.ToString());
        }

        /// <summary>
        ///     Renders XML documentation parameter entries as named documentation items.
        /// </summary>
        /// <param name="packageId">The packageId value used by the documentation generation operation.</param>
        /// <param name="version">The version value used by the documentation generation operation.</param>
        /// <param name="groupName">The groupName value used by the documentation generation operation.</param>
        /// <param name="symbol">The symbol value used by the documentation generation operation.</param>
        /// <param name="compilation">The compilation value used by the documentation generation operation.</param>
        /// <param name="currentTypeSymbol">The currentTypeSymbol value used by the documentation generation operation.</param>
        /// <returns>The RenderParameterItems result produced by DocumentationBuilder generation.</returns>
        public static List<DocumentationXmlNamedItem> RenderParameterItems(
            string packageId,
            string version,
            string groupName,
            ISymbol symbol,
            Compilation compilation,
            INamedTypeSymbol currentTypeSymbol
        )
        {
            return Extract(packageId, version, groupName, symbol, compilation, currentTypeSymbol).Parameters;
        }

        /// <summary>
        ///     Renders the XML documentation remarks section as safe HTML.
        /// </summary>
        /// <param name="packageId">The packageId value used by the documentation generation operation.</param>
        /// <param name="version">The version value used by the documentation generation operation.</param>
        /// <param name="groupName">The groupName value used by the documentation generation operation.</param>
        /// <param name="symbol">The symbol value used by the documentation generation operation.</param>
        /// <param name="compilation">The compilation value used by the documentation generation operation.</param>
        /// <param name="currentTypeSymbol">The currentTypeSymbol value used by the documentation generation operation.</param>
        /// <returns>The RenderRemarksHtml result produced by DocumentationBuilder generation.</returns>
        public static string? RenderRemarksHtml(
            string packageId,
            string version,
            string groupName,
            ISymbol symbol,
            Compilation compilation,
            INamedTypeSymbol currentTypeSymbol
        )
        {
            string value = Extract(packageId, version, groupName, symbol, compilation, currentTypeSymbol).RemarksHtml;
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        /// <summary>
        ///     Renders the XML documentation returns section as safe HTML.
        /// </summary>
        /// <param name="packageId">The packageId value used by the documentation generation operation.</param>
        /// <param name="version">The version value used by the documentation generation operation.</param>
        /// <param name="groupName">The groupName value used by the documentation generation operation.</param>
        /// <param name="symbol">The symbol value used by the documentation generation operation.</param>
        /// <param name="compilation">The compilation value used by the documentation generation operation.</param>
        /// <param name="currentTypeSymbol">The currentTypeSymbol value used by the documentation generation operation.</param>
        /// <returns>The RenderReturnsHtml result produced by DocumentationBuilder generation.</returns>
        public static string? RenderReturnsHtml(
            string packageId,
            string version,
            string groupName,
            ISymbol symbol,
            Compilation compilation,
            INamedTypeSymbol currentTypeSymbol
        )
        {
            string value = Extract(packageId, version, groupName, symbol, compilation, currentTypeSymbol).ReturnsHtml;
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        private static DocumentationXmlLinkItem? RenderSeeAlsoItem(
            string packageId,
            string version,
            string groupName,
            XElement element,
            Compilation compilation,
            INamedTypeSymbol currentTypeSymbol
        )
        {
            string? langword = element.Attribute("langword")?.Value;
            if (!string.IsNullOrWhiteSpace(langword))
            {
                return new DocumentationXmlLinkItem
                {
                    Label = langword,
                    IsKeyword = true,
                    Href = string.Empty,
                    NamespaceName = string.Empty,
                    ObjectName = string.Empty,
                    Anchor = string.Empty,
                    IsIntraPage = false
                };
            }

            string? href = element.Attribute("href")?.Value;
            if (!string.IsNullOrWhiteSpace(href))
            {
                return new DocumentationXmlLinkItem
                {
                    Label = string.IsNullOrWhiteSpace(element.Value) ? href : element.Value.Trim(),
                    Href = href,
                    NamespaceName = string.Empty,
                    ObjectName = string.Empty,
                    Anchor = string.Empty,
                    IsIntraPage = false,
                    IsKeyword = false
                };
            }

            string? cref = element.Attribute("cref")?.Value;
            if (string.IsNullOrWhiteSpace(cref)) return null;

            CrefTarget? target = ParseCref(cref, compilation, currentTypeSymbol);
            if (target is null)
                return new DocumentationXmlLinkItem
                {
                    Label = cref,
                    Href = string.Empty,
                    NamespaceName = string.Empty,
                    ObjectName = string.Empty,
                    Anchor = string.Empty,
                    IsIntraPage = false,
                    IsKeyword = false
                };

            if (target.IsKeyword)
            {
                return new DocumentationXmlLinkItem
                {
                    Label = target.Label,
                    Href = string.Empty,
                    NamespaceName = string.Empty,
                    ObjectName = string.Empty,
                    Anchor = string.Empty,
                    IsIntraPage = false,
                    IsKeyword = true
                };
            }

            string hrefValue = target.IsIntraPage && !string.IsNullOrWhiteSpace(target.Anchor)
                ? "#" + target.Anchor
                : BuildControllerHref(packageId, version, groupName, target.NamespaceName, target.ObjectName, target.Anchor);

            return new DocumentationXmlLinkItem
            {
                Label = target.Label,
                Href = hrefValue,
                NamespaceName = target.NamespaceName,
                ObjectName = target.ObjectName,
                Anchor = target.Anchor ?? string.Empty,
                IsIntraPage = target.IsIntraPage,
                IsKeyword = false
            };
        }

        /// <summary>
        ///     Renders XML documentation see-also entries as generated documentation links.
        /// </summary>
        /// <param name="packageId">The packageId value used by the documentation generation operation.</param>
        /// <param name="version">The version value used by the documentation generation operation.</param>
        /// <param name="groupName">The groupName value used by the documentation generation operation.</param>
        /// <param name="symbol">The symbol value used by the documentation generation operation.</param>
        /// <param name="compilation">The compilation value used by the documentation generation operation.</param>
        /// <param name="currentTypeSymbol">The currentTypeSymbol value used by the documentation generation operation.</param>
        /// <returns>The RenderSeeAlsoItems result produced by DocumentationBuilder generation.</returns>
        public static List<DocumentationXmlLinkItem> RenderSeeAlsoItems(
            string packageId,
            string version,
            string groupName,
            ISymbol symbol,
            Compilation compilation,
            INamedTypeSymbol currentTypeSymbol
        )
        {
            return Extract(packageId, version, groupName, symbol, compilation, currentTypeSymbol).SeeAlsos;
        }

        private static string RenderSeeElement(
            string packageId,
            string version,
            string groupName,
            XElement element,
            Compilation compilation,
            INamedTypeSymbol currentTypeSymbol
        )
        {
            DocumentationXmlLinkItem? item = RenderSeeAlsoItem(packageId, version, groupName, element, compilation, currentTypeSymbol);
            if (item is null) return string.Empty;

            if (item.IsKeyword) return $"<code>{Html(item.Label)}</code>";

            if (string.IsNullOrWhiteSpace(item.Href)) return $"<code>{Html(item.Label)}</code>";

            return $"<a href=\"{item.Href}\"><code>{Html(item.Label)}</code></a>";
        }

        /// <summary>
        ///     Renders the XML documentation summary section as safe HTML.
        /// </summary>
        /// <param name="packageId">The packageId value used by the documentation generation operation.</param>
        /// <param name="version">The version value used by the documentation generation operation.</param>
        /// <param name="groupName">The groupName value used by the documentation generation operation.</param>
        /// <param name="symbol">The symbol value used by the documentation generation operation.</param>
        /// <param name="compilation">The compilation value used by the documentation generation operation.</param>
        /// <param name="currentTypeSymbol">The currentTypeSymbol value used by the documentation generation operation.</param>
        /// <returns>The RenderSummaryHtml result produced by DocumentationBuilder generation.</returns>
        public static string? RenderSummaryHtml(
            string packageId,
            string version,
            string groupName,
            ISymbol symbol,
            Compilation compilation,
            INamedTypeSymbol currentTypeSymbol
        )
        {
            string value = Extract(packageId, version, groupName, symbol, compilation, currentTypeSymbol).SummaryHtml;
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        /// <summary>
        ///     Renders XML documentation type parameter entries as named documentation items.
        /// </summary>
        /// <param name="packageId">The packageId value used by the documentation generation operation.</param>
        /// <param name="version">The version value used by the documentation generation operation.</param>
        /// <param name="groupName">The groupName value used by the documentation generation operation.</param>
        /// <param name="symbol">The symbol value used by the documentation generation operation.</param>
        /// <param name="compilation">The compilation value used by the documentation generation operation.</param>
        /// <param name="currentTypeSymbol">The currentTypeSymbol value used by the documentation generation operation.</param>
        /// <returns>The RenderTypeParameterItems result produced by DocumentationBuilder generation.</returns>
        public static List<DocumentationXmlNamedItem> RenderTypeParameterItems(
            string packageId,
            string version,
            string groupName,
            ISymbol symbol,
            Compilation compilation,
            INamedTypeSymbol currentTypeSymbol
        )
        {
            return Extract(packageId, version, groupName, symbol, compilation, currentTypeSymbol).TypeParameters;
        }

        /// <summary>
        ///     Renders the XML documentation value section as safe HTML.
        /// </summary>
        /// <param name="packageId">The packageId value used by the documentation generation operation.</param>
        /// <param name="version">The version value used by the documentation generation operation.</param>
        /// <param name="groupName">The groupName value used by the documentation generation operation.</param>
        /// <param name="symbol">The symbol value used by the documentation generation operation.</param>
        /// <param name="compilation">The compilation value used by the documentation generation operation.</param>
        /// <param name="currentTypeSymbol">The currentTypeSymbol value used by the documentation generation operation.</param>
        /// <returns>The RenderValueHtml result produced by DocumentationBuilder generation.</returns>
        public static string? RenderValueHtml(
            string packageId,
            string version,
            string groupName,
            ISymbol symbol,
            Compilation compilation,
            INamedTypeSymbol currentTypeSymbol
        )
        {
            string value = Extract(packageId, version, groupName, symbol, compilation, currentTypeSymbol).ValueHtml;
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        private static string StripGenericArguments(string value)
        {
            StringBuilder sb = new();
            int depth = 0;

            foreach (char current in value)
            {
                if (current == '{')
                {
                    depth++;
                    continue;
                }

                if (current == '}')
                {
                    if (depth > 0) depth--;
                    continue;
                }

                if (depth == 0) sb.Append(current);
            }

            return sb.ToString();
        }

        private static XDocument? TryGetDocumentationDocument(ISymbol symbol)
        {
            string? xml = symbol.GetDocumentationCommentXml(
                expandIncludes: true,
                cancellationToken: default);

            if (string.IsNullOrWhiteSpace(xml)) return null;

            try
            {
                return XDocument.Parse(xml);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Nested type: CrefTarget

        private sealed class CrefTarget
        {
            #region Instance fields and properties

            /// <summary>
            ///     Gets or sets the Anchor value used by generated documentation.
            /// </summary>
            public string? Anchor { get; init; }

            /// <summary>
            ///     Gets or sets the IsIntraPage value used by generated documentation.
            /// </summary>
            public required bool IsIntraPage { get; init; }

            /// <summary>
            ///     Gets or sets the IsKeyword value used by generated documentation.
            /// </summary>
            public required bool IsKeyword { get; init; }

            /// <summary>
            ///     Gets or sets the Label value used by generated documentation.
            /// </summary>
            public required string Label { get; init; }

            /// <summary>
            ///     Gets or sets the NamespaceName value used by generated documentation.
            /// </summary>
            public required string NamespaceName { get; init; }

            /// <summary>
            ///     Gets or sets the ObjectName value used by generated documentation.
            /// </summary>
            public required string ObjectName { get; init; }

            #endregion
        }

        #endregion
    }
}