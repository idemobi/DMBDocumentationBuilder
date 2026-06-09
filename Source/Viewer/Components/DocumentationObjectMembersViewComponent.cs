#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Renders a compact list of granular members for one generated documentation object.
    /// </summary>
    public sealed class DocumentationObjectMembersViewComponent : ViewComponent
    {
        #region Static methods

        private static DocumentationMemberDisplayFlags ResolveDisplayFlags(
            DocumentationMemberDisplayFlags displayFlags,
            DocumentationMemberDisplayMode displayMode
        )
        {
            DocumentationMemberDisplayFlags referenceFlag = displayFlags & DocumentationMemberDisplayFlags.ReferenceLink;

            return displayMode switch
            {
                DocumentationMemberDisplayMode.SignatureOnly => DocumentationMemberDisplayFlags.Signature | referenceFlag,
                DocumentationMemberDisplayMode.SignatureAndDescription => DocumentationMemberDisplayFlags.Signature |
                                                                          DocumentationMemberDisplayFlags.Summary |
                                                                          referenceFlag,
                _ => displayFlags
            };
        }

        private static IReadOnlyCollection<string>? ResolveMemberKinds(string? memberKind, IReadOnlyCollection<string>? memberKinds)
        {
            if (memberKinds is { Count: > 0 })
            {
                return memberKinds;
            }

            return string.IsNullOrWhiteSpace(memberKind)
                ? null
                : [memberKind];
        }

        #endregion

        #region Instance fields and properties

        private readonly IWebHostEnvironment _environment;

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DocumentationObjectMembersViewComponent" /> class.
        /// </summary>
        /// <param name="environment">Host environment used to resolve the generated documentation database path.</param>
        public DocumentationObjectMembersViewComponent(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        #endregion

        #region Instance methods

        /// <summary>
        ///     Renders a compact member list for the requested documentation object.
        /// </summary>
        /// <param name="objectName">Exact documented object name.</param>
        /// <param name="memberKind">Optional member kind filter, such as <c>Method</c> or <c>Property</c>.</param>
        /// <param name="memberKinds">Optional member kind filters used by fluent rendering.</param>
        /// <param name="packageId">Optional package identifier filter.</param>
        /// <param name="version">Optional package version filter.</param>
        /// <param name="namespaceName">Optional namespace filter.</param>
        /// <param name="objectType">Optional documented object type filter.</param>
        /// <param name="title">Optional title displayed above the member list.</param>
        /// <param name="showReferenceLink">Indicates whether a link to the full documentation page is displayed.</param>
        /// <param name="displayFlags">Flags indicating which member fields should be rendered.</param>
        /// <param name="displayMode">Common display preset applied before the reference link option.</param>
        /// <param name="referenceTitle">Optional text displayed for the reference documentation link.</param>
        /// <returns>The rendered member list view.</returns>
        public IViewComponentResult Invoke(
            string objectName,
            string? memberKind = "Method",
            IReadOnlyCollection<string>? memberKinds = null,
            string? packageId = null,
            string? version = null,
            string? namespaceName = null,
            string? objectType = "Class",
            string? title = "Methods",
            bool showReferenceLink = true,
            DocumentationMemberDisplayFlags displayFlags = DocumentationMemberDisplayFlags.Default,
            DocumentationMemberDisplayMode displayMode = DocumentationMemberDisplayMode.Default,
            string? referenceTitle = "Reference documentation"
        )
        {
            string dbPath = Path.Combine(_environment.ContentRootPath, "Documentation", "data.db");
            DocumentationQueryService queryService = new(dbPath);
            DocumentationQueryResult? referenceDocumentation = queryService.GetDocumentation(
                objectName,
                packageId,
                version,
                namespaceName,
                objectType);
            IReadOnlyList<DocumentationMemberQueryResult> members = queryService.ListObjectMembers(
                objectName,
                ResolveMemberKinds(memberKind, memberKinds),
                packageId,
                version,
                namespaceName,
                objectType);

            DocumentationMemberDisplayFlags effectiveDisplayFlags = ResolveDisplayFlags(displayFlags, displayMode);

            if (!showReferenceLink)
            {
                effectiveDisplayFlags &= ~DocumentationMemberDisplayFlags.ReferenceLink;
            }

            DocumentationObjectMembersViewModel model = new()
            {
                DisplayFlags = effectiveDisplayFlags,
                ObjectName = objectName,
                ReferenceRoutePath = referenceDocumentation?.RoutePath ?? string.Empty,
                ReferenceTitle = referenceTitle ?? string.Empty,
                ShowReferenceLink = effectiveDisplayFlags.HasFlag(DocumentationMemberDisplayFlags.ReferenceLink),
                Title = title ?? string.Empty,
                Members = members
            };

            return View("~/Views/Shared/Components/DocumentationObjectMembers/Default.cshtml", model);
        }

        #endregion
    }
}