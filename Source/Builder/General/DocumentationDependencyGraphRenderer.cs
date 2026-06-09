#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Text;

#endregion

namespace DMBDocumentationBuilder
{
    internal static class DocumentationDependencyGraphRenderer
    {
        #region Static methods

        /// <summary>
        ///     Renders a compact SVG dependency graph for one documented object page.
        /// </summary>
        /// <param name="dependencyEdges">The dependency edges to render.</param>
        /// <returns>The rendered dependency graph HTML, or an empty string when no edge is available.</returns>
        public static string RenderHtml(IReadOnlyCollection<DocumentationDependencyEdgeItem> dependencyEdges)
        {
            if (dependencyEdges.Count == 0) return string.Empty;

            StringBuilder sb = new();
            const int maxEdgeCount = 32;
            const int sourceX = 24;
            const int targetX = 430;
            const int nodeWidth = 260;
            const int nodeHeight = 42;
            const int rowHeight = 58;
            const int topPadding = 34;

            List<DocumentationDependencyEdgeItem> edges = dependencyEdges
                .OrderBy(x => x.RelationshipKind, StringComparer.Ordinal)
                .ThenBy(x => x.TargetNamespaceName, StringComparer.Ordinal)
                .ThenBy(x => x.TargetName, StringComparer.Ordinal)
                .Take(maxEdgeCount)
                .ToList();

            DocumentationDependencyEdgeItem firstEdge = edges[0];
            int height = Math.Max(180, topPadding + edges.Count * rowHeight + 20);

            sb.AppendLine("                <section id=\"dependency-graph\" class=\"card border-0 shadow-sm mt-3\">");
            sb.AppendLine("                    <div class=\"card-body p-2\">");
            sb.AppendLine("                        <div class=\"d-flex flex-wrap align-items-center justify-content-between gap-2 mb-2\">");
            sb.AppendLine("                            <h2 class=\"mb-0\">Dependency graph</h2>");
            sb.Append("                            <span class=\"badge text-bg-secondary rounded-pill\">")
                .Append(edges.Count)
                .Append(" relation");
            if (edges.Count != 1) sb.Append("s");
            sb.AppendLine("</span>");
            sb.AppendLine("                        </div>");

            if (dependencyEdges.Count > edges.Count)
            {
                sb.Append("                        <p class=\"text-body-secondary small mb-2\">Showing the first ")
                    .Append(edges.Count)
                    .Append(" relations out of ")
                    .Append(dependencyEdges.Count)
                    .AppendLine(".</p>");
            }

            sb.AppendLine("                        <div class=\"overflow-auto border rounded-2 bg-body-tertiary p-2\">");
            sb.Append("                            <svg role=\"img\" aria-label=\"Documented object dependency graph\" viewBox=\"0 0 720 ")
                .Append(height)
                .AppendLine("\" width=\"100%\" style=\"min-width: 680px; max-height: 520px;\">");
            sb.AppendLine("                                <defs>");
            sb.AppendLine("                                    <marker id=\"documentation-object-dependency-arrow\" markerWidth=\"8\" markerHeight=\"8\" refX=\"6\" refY=\"3\" orient=\"auto\" markerUnits=\"strokeWidth\">");
            sb.AppendLine("                                        <path d=\"M0,0 L0,6 L7,3 z\" fill=\"currentColor\"></path>");
            sb.AppendLine("                                    </marker>");
            sb.AppendLine("                                </defs>");
            sb.AppendLine("                                <text x=\"24\" y=\"18\" fill=\"currentColor\" font-size=\"12\" font-weight=\"700\">This object</text>");
            sb.AppendLine("                                <text x=\"430\" y=\"18\" fill=\"currentColor\" font-size=\"12\" font-weight=\"700\">Referenced objects</text>");

            RenderNode(
                sb,
                firstEdge.SourcePackageId,
                firstEdge.SourceVersion,
                firstEdge.SourceGroupName,
                firstEdge.SourceNamespaceName,
                firstEdge.SourceName,
                firstEdge.SourceKindLabel,
                sourceX,
                topPadding,
                nodeWidth,
                nodeHeight);

            for (int index = 0; index < edges.Count; index++)
            {
                DocumentationDependencyEdgeItem edge = edges[index];
                int targetY = topPadding + index * rowHeight;
                int sourceY = topPadding + nodeHeight / 2;
                int targetCenterY = targetY + nodeHeight / 2;
                string color = GetRelationshipColor(edge.RelationshipKind);

                sb.Append("                                <path d=\"M ")
                    .Append(sourceX + nodeWidth)
                    .Append(' ')
                    .Append(sourceY)
                    .Append(" C 350 ")
                    .Append(sourceY)
                    .Append(", 380 ")
                    .Append(targetCenterY)
                    .Append(", ")
                    .Append(targetX)
                    .Append(' ')
                    .Append(targetCenterY)
                    .Append("\" fill=\"none\" stroke=\"")
                    .Append(color)
                    .Append("\" stroke-width=\"1.8\" opacity=\"0.72\" marker-end=\"url(#documentation-object-dependency-arrow)\">")
                    .Append("<title>")
                    .Append(Html(edge.SourceName))
                    .Append(' ')
                    .Append(Html(edge.RelationshipKind.ToLowerInvariant()))
                    .Append(' ')
                    .Append(Html(edge.TargetName))
                    .AppendLine("</title></path>");

                sb.Append("                                <text x=\"342\" y=\"")
                    .Append(targetCenterY - 5)
                    .Append("\" fill=\"")
                    .Append(color)
                    .Append("\" font-size=\"10\" font-weight=\"700\">")
                    .Append(Html(edge.RelationshipKind))
                    .AppendLine("</text>");

                RenderNode(
                    sb,
                    edge.TargetPackageId,
                    edge.TargetVersion,
                    edge.TargetGroupName,
                    edge.TargetNamespaceName,
                    edge.TargetName,
                    edge.TargetKindLabel,
                    targetX,
                    targetY,
                    nodeWidth,
                    nodeHeight);
            }

            sb.AppendLine("                            </svg>");
            sb.AppendLine("                        </div>");
            sb.AppendLine("                    </div>");
            sb.AppendLine("                </section>");

            return sb.ToString();
        }

        private static string Attribute(string? value)
        {
            return System.Net.WebUtility.HtmlEncode(value ?? string.Empty);
        }

        private static string BuildObjectRoutePath(
            string packageId,
            string version,
            string groupName,
            string namespaceName,
            string objectName
        )
        {
            StringBuilder sb = new();
            sb.Append("/Documentation/Show?packageId=")
                .Append(System.Net.WebUtility.UrlEncode(packageId))
                .Append("&version=")
                .Append(System.Net.WebUtility.UrlEncode(version))
                .Append("&groupName=")
                .Append(System.Net.WebUtility.UrlEncode(groupName))
                .Append("&namespaceName=")
                .Append(System.Net.WebUtility.UrlEncode(namespaceName))
                .Append("&objectName=")
                .Append(System.Net.WebUtility.UrlEncode(objectName));

            return sb.ToString();
        }

        private static string GetRelationshipColor(string relationshipKind)
        {
            return relationshipKind switch
            {
                "Extends" => "#0dcaf0",
                "Inherits" => "#0d6efd",
                "Implements" => "#198754",
                _ => "#6c757d"
            };
        }

        private static string Html(string? value)
        {
            return System.Net.WebUtility.HtmlEncode(value ?? string.Empty);
        }

        private static void RenderNode(
            StringBuilder sb,
            string packageId,
            string version,
            string groupName,
            string namespaceName,
            string objectName,
            string kindLabel,
            int x,
            int y,
            int width,
            int height
        )
        {
            string routePath = BuildObjectRoutePath(packageId, version, groupName, namespaceName, objectName);

            sb.Append("                                <a href=\"")
                .Append(Attribute(routePath))
                .AppendLine("\">");
            sb.Append("                                    <rect x=\"")
                .Append(x)
                .Append("\" y=\"")
                .Append(y)
                .Append("\" width=\"")
                .Append(width)
                .Append("\" height=\"")
                .Append(height)
                .AppendLine("\" rx=\"6\" fill=\"var(--bs-body-bg)\" stroke=\"var(--bs-border-color)\" stroke-width=\"1\"></rect>");
            sb.Append("                                    <text x=\"")
                .Append(x + 12)
                .Append("\" y=\"")
                .Append(y + 18)
                .Append("\" fill=\"currentColor\" font-size=\"13\" font-weight=\"700\">")
                .Append(Html(Shorten(objectName, 30)))
                .AppendLine("</text>");
            sb.Append("                                    <text x=\"")
                .Append(x + 12)
                .Append("\" y=\"")
                .Append(y + 34)
                .Append("\" fill=\"currentColor\" opacity=\"0.68\" font-size=\"11\">")
                .Append(Html(Shorten(kindLabel + " - " + namespaceName, 38)))
                .AppendLine("</text>");
            sb.AppendLine("                                </a>");
        }

        private static string Shorten(string? value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            string text = value.Trim();
            if (text.Length <= maxLength) return text;

            return text[..Math.Max(0, maxLength - 3)] + "...";
        }

        #endregion
    }
}
