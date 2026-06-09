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
            const int horizontalPadding = 24;
            const int columnGap = 150;
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

            bool hasIncomingEdges = incomingEdges.Count > 0;
            bool hasOutgoingEdges = outgoingEdges.Count > 0;
            int dependentsX = horizontalPadding;
            int currentX = hasIncomingEdges
                ? dependentsX + nodeWidth + columnGap
                : horizontalPadding;
            int dependenciesX = currentX + nodeWidth + columnGap;
            int lastColumnX = hasOutgoingEdges ? dependenciesX : currentX;
            int svgWidth = lastColumnX + nodeWidth + horizontalPadding;
            int rowCount = Math.Max(1, Math.Max(incomingEdges.Count, outgoingEdges.Count));
            int height = Math.Max(220, topPadding + rowCount * rowHeight + 28);
            int centerY = topPadding + Math.Max(0, rowCount - 1) * rowHeight / 2;

            StringBuilder sb = new();
            sb.AppendLine("                <section id=\"dependency-graph\" class=\"card border-0 shadow-sm mt-3\" data-doc-display-element=\"dependency-graph\">");
            sb.AppendLine("                    <div class=\"card-body p-2\">");
            sb.AppendLine("                        <div class=\"d-flex flex-wrap align-items-center justify-content-between gap-2 mb-2\">");
            sb.AppendLine("                            <h2 class=\"mb-0\">Dependency graph</h2>");
            sb.AppendLine("                            <div class=\"d-inline-flex align-items-center gap-2\">");
            sb.AppendLine("                                <div class=\"btn-group btn-group-sm\" role=\"group\" aria-label=\"Dependency graph zoom controls\">");
            sb.AppendLine("                                    <button type=\"button\" class=\"btn btn-outline-secondary\" title=\"Zoom out\" aria-label=\"Zoom out\" data-documentation-dependency-zoom=\"out\"><i class=\"bi bi-zoom-out\"></i></button>");
            sb.AppendLine("                                    <button type=\"button\" class=\"btn btn-outline-secondary\" title=\"Reset zoom\" aria-label=\"Reset zoom\" data-documentation-dependency-zoom=\"reset\"><i class=\"bi bi-aspect-ratio\"></i></button>");
            sb.AppendLine("                                    <button type=\"button\" class=\"btn btn-outline-secondary\" title=\"Zoom in\" aria-label=\"Zoom in\" data-documentation-dependency-zoom=\"in\"><i class=\"bi bi-zoom-in\"></i></button>");
            sb.AppendLine("                                </div>");
            sb.AppendLine("                                <span class=\"badge text-bg-secondary rounded-pill\" data-documentation-dependency-zoom-value=\"true\">100%</span>");
            sb.Append("                                <span class=\"badge text-bg-secondary rounded-pill\">")
                .Append(incomingEdges.Count)
                .Append(" ← · ")
                .Append(outgoingEdges.Count)
                .AppendLine(" →</span>");
            sb.AppendLine("                            </div>");
            sb.AppendLine("                        </div>");

            if (incomingEdges.Count + outgoingEdges.Count < dependencyEdges.Count)
            {
                sb.Append("                        <p class=\"text-body-secondary small mb-2\">Showing up to ")
                    .Append(maxEdgeCountPerSide)
                    .AppendLine(" relations per side.</p>");
            }

            sb.AppendLine("                        <div class=\"overflow-auto border rounded-2 bg-body-tertiary p-2\" data-documentation-dependency-graph=\"true\" style=\"max-height: 560px;\">");
            sb.Append("                            <div data-documentation-dependency-canvas=\"true\" data-documentation-dependency-base-width=\"")
                .Append(svgWidth)
                .Append("\" data-documentation-dependency-base-height=\"")
                .Append(height)
                .Append("\" style=\"width: ")
                .Append(svgWidth)
                .Append("px; height: ")
                .Append(height)
                .AppendLine("px;\">");
            sb.Append("                            <svg role=\"img\" aria-label=\"Two-way documented object dependency graph\" viewBox=\"0 0 ")
                .Append(svgWidth)
                .Append(' ')
                .Append(height)
                .Append("\" width=\"")
                .Append(svgWidth)
                .Append("\" height=\"")
                .Append(height)
                .AppendLine("\" data-documentation-dependency-svg=\"true\" style=\"display: block; overflow: visible;\">");
            sb.AppendLine("                                <defs>");
            AppendRelationshipMarkers(sb);
            sb.AppendLine("                                </defs>");

            if (hasIncomingEdges)
            {
                RenderColumnHeading(sb, dependentsX, nodeWidth, "Dependents");
            }

            RenderColumnHeading(sb, currentX, nodeWidth, "This object");

            if (hasOutgoingEdges)
            {
                RenderColumnHeading(sb, dependenciesX, nodeWidth, "Dependencies");
            }

            RenderNode(
                sb,
                packageId,
                version,
                groupName,
                namespaceName,
                objectName,
                kindLabel,
                currentX,
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
                    dependentsX,
                    sourceY,
                    nodeWidth,
                    nodeHeight,
                    false);

                RenderPath(
                    sb,
                    dependentsX + nodeWidth,
                    sourceCenterY,
                    currentX,
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
                    currentX + nodeWidth,
                    currentCenterY,
                    dependenciesX,
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
                    dependenciesX,
                    targetY,
                    nodeWidth,
                    nodeHeight,
                    false);
            }

            sb.AppendLine("                            </svg>");
            sb.AppendLine("                            </div>");
            sb.AppendLine("                        </div>");
            AppendZoomScript(sb);
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

        private static void AppendZoomScript(StringBuilder sb)
        {
            sb.AppendLine("                        <script>");
            sb.AppendLine("                            (function () {");
            sb.AppendLine("                                document.querySelectorAll('[data-documentation-dependency-graph=\"true\"]').forEach(function (graph) {");
            sb.AppendLine("                                    if (graph.dataset.documentationDependencyZoomReady === 'true') return;");
            sb.AppendLine("                                    graph.dataset.documentationDependencyZoomReady = 'true';");
            sb.AppendLine("                                    var section = graph.closest('section');");
            sb.AppendLine("                                    if (!section) return;");
            sb.AppendLine("                                    var svg = graph.querySelector('[data-documentation-dependency-svg=\"true\"]');");
            sb.AppendLine("                                    var canvas = graph.querySelector('[data-documentation-dependency-canvas=\"true\"]');");
            sb.AppendLine("                                    if (!svg || !canvas) return;");
            sb.AppendLine("                                    var valueLabel = section.querySelector('[data-documentation-dependency-zoom-value=\"true\"]');");
            sb.AppendLine("                                    var baseWidth = Number(canvas.dataset.documentationDependencyBaseWidth) || 1098;");
            sb.AppendLine("                                    var baseHeight = Number(canvas.dataset.documentationDependencyBaseHeight) || 240;");
            sb.AppendLine("                                    var baseViewBox = svg.getAttribute('viewBox') || '0 0 ' + baseWidth + ' ' + baseHeight;");
            sb.AppendLine("                                    var zoom = 1;");
            sb.AppendLine("                                    function applyZoom() {");
            sb.AppendLine("                                        var centerX = graph.scrollLeft + graph.clientWidth / 2;");
            sb.AppendLine("                                        var centerY = graph.scrollTop + graph.clientHeight / 2;");
            sb.AppendLine("                                        var ratioX = centerX / Math.max(1, graph.scrollWidth);");
            sb.AppendLine("                                        var ratioY = centerY / Math.max(1, graph.scrollHeight);");
            sb.AppendLine("                                        var width = Math.round(baseWidth * zoom);");
            sb.AppendLine("                                        var height = Math.round(baseHeight * zoom);");
            sb.AppendLine("                                        canvas.style.width = width + 'px';");
            sb.AppendLine("                                        canvas.style.height = height + 'px';");
            sb.AppendLine("                                        svg.style.width = width + 'px';");
            sb.AppendLine("                                        svg.style.height = height + 'px';");
            sb.AppendLine("                                        svg.setAttribute('viewBox', baseViewBox);");
            sb.AppendLine("                                        if (valueLabel) valueLabel.textContent = Math.round(zoom * 100) + '%';");
            sb.AppendLine("                                        window.requestAnimationFrame(function () {");
            sb.AppendLine("                                            graph.scrollLeft = ratioX * graph.scrollWidth - graph.clientWidth / 2;");
            sb.AppendLine("                                            graph.scrollTop = ratioY * graph.scrollHeight - graph.clientHeight / 2;");
            sb.AppendLine("                                        });");
            sb.AppendLine("                                    }");
            sb.AppendLine("                                    section.querySelectorAll('[data-documentation-dependency-zoom]').forEach(function (button) {");
            sb.AppendLine("                                        button.addEventListener('click', function () {");
            sb.AppendLine("                                            var action = button.getAttribute('data-documentation-dependency-zoom');");
            sb.AppendLine("                                            if (action === 'in') zoom = Math.min(2.4, zoom + 0.2);");
            sb.AppendLine("                                            if (action === 'out') zoom = Math.max(0.6, zoom - 0.2);");
            sb.AppendLine("                                            if (action === 'reset') zoom = 1;");
            sb.AppendLine("                                            applyZoom();");
            sb.AppendLine("                                        });");
            sb.AppendLine("                                    });");
            sb.AppendLine("                                    applyZoom();");
            sb.AppendLine("                                });");
            sb.AppendLine("                            })();");
            sb.AppendLine("                        </script>");
        }

        private static void AppendRelationshipMarkers(StringBuilder sb)
        {
            AppendRelationshipMarker(sb, GetRelationshipMarkerId("Extends", false), GetRelationshipColor("Extends"), false);
            AppendRelationshipMarker(sb, GetRelationshipMarkerId("Extends", true), GetRelationshipColor("Extends"), true);
            AppendRelationshipMarker(sb, GetRelationshipMarkerId("Inherits", false), GetRelationshipColor("Inherits"), false);
            AppendRelationshipMarker(sb, GetRelationshipMarkerId("Inherits", true), GetRelationshipColor("Inherits"), true);
            AppendRelationshipMarker(sb, GetRelationshipMarkerId("Implements", false), GetRelationshipColor("Implements"), false);
            AppendRelationshipMarker(sb, GetRelationshipMarkerId("Implements", true), GetRelationshipColor("Implements"), true);
            AppendRelationshipMarker(sb, GetRelationshipMarkerId("Uses", false), GetRelationshipColor("Uses"), false);
            AppendRelationshipMarker(sb, GetRelationshipMarkerId("Uses", true), GetRelationshipColor("Uses"), true);
        }

        private static void AppendRelationshipMarker(StringBuilder sb, string markerId, string color, bool useCircleMarker)
        {
            sb.Append("                                    <marker id=\"")
                .Append(markerId)
                .Append(useCircleMarker
                    ? "\" markerWidth=\"8\" markerHeight=\"8\" refX=\"4\" refY=\"4\" orient=\"auto\" markerUnits=\"strokeWidth\">"
                    : "\" markerWidth=\"8\" markerHeight=\"8\" refX=\"6\" refY=\"3\" orient=\"auto\" markerUnits=\"strokeWidth\">")
                .AppendLine();

            if (useCircleMarker)
            {
                sb.Append("                                        <circle cx=\"4\" cy=\"4\" r=\"3\" fill=\"")
                    .Append(color)
                    .AppendLine("\" fill-opacity=\"0.72\"></circle>");
            }
            else
            {
                sb.Append("                                        <path d=\"M0,0 L0,6 L7,3 z\" fill=\"")
                    .Append(color)
                    .AppendLine("\" fill-opacity=\"0.72\"></path>");
            }

            sb.AppendLine("                                    </marker>");
        }

        private static string GetRelationshipMarkerId(string relationshipKind, bool useCircleMarker)
        {
            string suffix = relationshipKind switch
            {
                "Extends" => "extends",
                "Inherits" => "inherits",
                "Implements" => "implements",
                _ => "uses"
            };

            return "documentation-object-dependency-" + (useCircleMarker ? "circle-" : "arrow-") + suffix;
        }

        private static void RenderColumnHeading(StringBuilder sb, int x, int nodeWidth, string heading)
        {
            sb.Append("                                <text x=\"")
                .Append(x + nodeWidth / 2)
                .Append("\" y=\"22\" fill=\"currentColor\" font-size=\"12\" font-weight=\"700\" text-anchor=\"middle\">")
                .Append(Html(heading))
                .AppendLine("</text>");
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
            string markerId = GetRelationshipMarkerId(relationshipKind, useCircleMarker);

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
                .Append("\" stroke-width=\"1.8\" stroke-opacity=\"0.72\" marker-end=\"url(#")
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
