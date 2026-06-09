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
        ///     Renders a compact two-way SVG dependency graph for one documented object page.
        /// </summary>
        /// <param name="dependencyEdges">The incoming and outgoing dependency edges to render.</param>
        /// <param name="packageId">The package identifier that owns the current object.</param>
        /// <param name="version">The package version that owns the current object.</param>
        /// <param name="groupName">The documentation group that owns the current object.</param>
        /// <param name="namespaceName">The namespace that owns the current object.</param>
        /// <param name="objectName">The current object name.</param>
        /// <param name="kindLabel">The current object kind label.</param>
        /// <returns>The rendered dependency graph HTML, or an empty string when no edge is available.</returns>
        public static string RenderHtml(
            IReadOnlyCollection<DocumentationDependencyEdgeItem> dependencyEdges,
            string packageId,
            string version,
            string groupName,
            string namespaceName,
            string objectName,
            string kindLabel
        )
        {
            if (dependencyEdges.Count == 0) return string.Empty;

            const int maxEdgeCountPerSide = 18;
            const int leftX = 24;
            const int centerX = 424;
            const int rightX = 824;
            const int nodeWidth = 250;
            const int nodeHeight = 42;
            const int rowHeight = 58;
            const int topPadding = 42;

            List<DocumentationDependencyEdgeItem> incomingEdges = dependencyEdges
                .Where(edge =>
                    string.Equals(edge.TargetNamespaceName, namespaceName, StringComparison.Ordinal) &&
                    string.Equals(edge.TargetName, objectName, StringComparison.Ordinal))
                .OrderBy(x => x.SourceNamespaceName, StringComparer.Ordinal)
                .ThenBy(x => x.SourceName, StringComparer.Ordinal)
                .ThenBy(x => x.RelationshipKind, StringComparer.Ordinal)
                .Take(maxEdgeCountPerSide)
                .ToList();

            List<DocumentationDependencyEdgeItem> outgoingEdges = dependencyEdges
                .Where(edge =>
                    string.Equals(edge.SourceNamespaceName, namespaceName, StringComparison.Ordinal) &&
                    string.Equals(edge.SourceName, objectName, StringComparison.Ordinal))
                .OrderBy(x => x.RelationshipKind, StringComparer.Ordinal)
                .ThenBy(x => x.TargetNamespaceName, StringComparer.Ordinal)
                .ThenBy(x => x.TargetName, StringComparer.Ordinal)
                .Take(maxEdgeCountPerSide)
                .ToList();

            if (incomingEdges.Count == 0 && outgoingEdges.Count == 0) return string.Empty;

            int rowCount = Math.Max(1, Math.Max(incomingEdges.Count, outgoingEdges.Count));
            int height = Math.Max(220, topPadding + rowCount * rowHeight + 28);
            int centerY = topPadding + Math.Max(0, rowCount - 1) * rowHeight / 2;

            StringBuilder sb = new();
            sb.AppendLine("                <section id=\"dependency-graph\" class=\"card border-0 shadow-sm mt-3\">");
            sb.AppendLine("                    <div class=\"card-body p-2\">");
            sb.AppendLine("                        <div class=\"d-flex flex-wrap align-items-center justify-content-between gap-2 mb-2\">");
            sb.AppendLine("                            <h2 class=\"mb-0\">Dependency graph</h2>");
            sb.Append("                            <span class=\"badge text-bg-secondary rounded-pill\">")
                .Append(incomingEdges.Count)
                .Append(" ← · ")
                .Append(outgoingEdges.Count)
                .AppendLine(" →</span>");
            sb.AppendLine("                        </div>");

            if (incomingEdges.Count + outgoingEdges.Count < dependencyEdges.Count)
            {
                sb.Append("                        <p class=\"text-body-secondary small mb-2\">Showing up to ")
                    .Append(maxEdgeCountPerSide)
                    .AppendLine(" relations per side.</p>");
            }

            sb.AppendLine("                        <div class=\"overflow-auto border rounded-2 bg-body-tertiary p-2\">");
            sb.Append("                            <svg role=\"img\" aria-label=\"Two-way documented object dependency graph\" viewBox=\"0 0 1098 ")
                .Append(height)
                .AppendLine("\" width=\"100%\" style=\"min-width: 1040px; max-height: 560px;\">");
            sb.AppendLine("                                <defs>");
            sb.AppendLine("                                    <marker id=\"documentation-object-dependency-arrow\" markerWidth=\"8\" markerHeight=\"8\" refX=\"6\" refY=\"3\" orient=\"auto\" markerUnits=\"strokeWidth\">");
            sb.AppendLine("                                        <path d=\"M0,0 L0,6 L7,3 z\" fill=\"context-stroke\"></path>");
            sb.AppendLine("                                    </marker>");
            sb.AppendLine("                                    <marker id=\"documentation-object-dependency-circle\" markerWidth=\"8\" markerHeight=\"8\" refX=\"4\" refY=\"4\" orient=\"auto\" markerUnits=\"strokeWidth\">");
            sb.AppendLine("                                        <circle cx=\"4\" cy=\"4\" r=\"3\" fill=\"context-stroke\"></circle>");
            sb.AppendLine("                                    </marker>");
            sb.AppendLine("                                </defs>");
            sb.AppendLine("                                <text x=\"24\" y=\"22\" fill=\"currentColor\" font-size=\"12\" font-weight=\"700\">Dependents</text>");
            sb.AppendLine("                                <text x=\"424\" y=\"22\" fill=\"currentColor\" font-size=\"12\" font-weight=\"700\">This object</text>");
            sb.AppendLine("                                <text x=\"824\" y=\"22\" fill=\"currentColor\" font-size=\"12\" font-weight=\"700\">Dependencies</text>");

            RenderNode(
                sb,
                packageId,
                version,
                groupName,
                namespaceName,
                objectName,
                kindLabel,
                centerX,
                centerY,
                nodeWidth,
                nodeHeight,
                true);

            for (int index = 0; index < incomingEdges.Count; index++)
            {
                DocumentationDependencyEdgeItem edge = incomingEdges[index];
                int sourceY = topPadding + index * rowHeight;
                int sourceCenterY = sourceY + nodeHeight / 2;
                int currentCenterY = centerY + nodeHeight / 2;
                string color = GetRelationshipColor(edge.RelationshipKind);

                RenderNode(
                    sb,
                    edge.SourcePackageId,
                    edge.SourceVersion,
                    edge.SourceGroupName,
                    edge.SourceNamespaceName,
                    edge.SourceName,
                    edge.SourceKindLabel,
                    leftX,
                    sourceY,
                    nodeWidth,
                    nodeHeight,
                    false);

                RenderPath(
                    sb,
                    leftX + nodeWidth,
                    sourceCenterY,
                    centerX,
                    currentCenterY,
                    edge.RelationshipKind,
                    edge.SourceName,
                    edge.TargetName,
                    color,
                    string.Equals(kindLabel, "Interface", StringComparison.Ordinal));
            }

            for (int index = 0; index < outgoingEdges.Count; index++)
            {
                DocumentationDependencyEdgeItem edge = outgoingEdges[index];
                int targetY = topPadding + index * rowHeight;
                int currentCenterY = centerY + nodeHeight / 2;
                int targetCenterY = targetY + nodeHeight / 2;
                string color = GetRelationshipColor(edge.RelationshipKind);

                RenderPath(
                    sb,
                    centerX + nodeWidth,
                    currentCenterY,
                    rightX,
                    targetCenterY,
                    edge.RelationshipKind,
                    edge.SourceName,
                    edge.TargetName,
                    color,
                    string.Equals(edge.TargetKindLabel, "Interface", StringComparison.Ordinal));

                RenderNode(
                    sb,
                    edge.TargetPackageId,
                    edge.TargetVersion,
                    edge.TargetGroupName,
                    edge.TargetNamespaceName,
                    edge.TargetName,
                    edge.TargetKindLabel,
                    rightX,
                    targetY,
                    nodeWidth,
                    nodeHeight,
                    false);
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
            int height,
            bool isCurrent
        )
        {
            string routePath = BuildObjectRoutePath(packageId, version, groupName, namespaceName, objectName);
            string stroke = isCurrent ? "var(--bs-primary)" : "var(--bs-border-color)";
            string strokeWidth = isCurrent ? "2" : "1";

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
                .Append("\" rx=\"6\" fill=\"var(--bs-body-bg)\" stroke=\"")
                .Append(stroke)
                .Append("\" stroke-width=\"")
                .Append(strokeWidth)
                .AppendLine("\"></rect>");
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

        private static void RenderPath(
            StringBuilder sb,
            int startX,
            int startY,
            int endX,
            int endY,
            string relationshipKind,
            string sourceName,
            string targetName,
            string color,
            bool useCircleMarker
        )
        {
            int curveStartX = startX + Math.Sign(endX - startX) * 140;
            int curveEndX = endX - Math.Sign(endX - startX) * 140;
            string markerId = useCircleMarker
                ? "documentation-object-dependency-circle"
                : "documentation-object-dependency-arrow";

            sb.Append("                                <path d=\"M ")
                .Append(startX)
                .Append(' ')
                .Append(startY)
                .Append(" C ")
                .Append(curveStartX)
                .Append(' ')
                .Append(startY)
                .Append(", ")
                .Append(curveEndX)
                .Append(' ')
                .Append(endY)
                .Append(", ")
                .Append(endX)
                .Append(' ')
                .Append(endY)
                .Append("\" fill=\"none\" stroke=\"")
                .Append(color)
                .Append("\" stroke-width=\"1.8\" opacity=\"0.72\" marker-end=\"url(#")
                .Append(markerId)
                .Append(")\">")
                .Append("<title>")
                .Append(Html(sourceName))
                .Append(' ')
                .Append(Html(relationshipKind.ToLowerInvariant()))
                .Append(' ')
                .Append(Html(targetName))
                .AppendLine("</title></path>");
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
