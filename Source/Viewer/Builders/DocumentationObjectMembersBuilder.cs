#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Configures and renders a documentation object member list through a fluent Razor API.
    /// </summary>
    public sealed class DocumentationObjectMembersBuilder
    {
        #region Instance fields and properties

        private DocumentationMemberDisplayFlags _displayFlags = DocumentationMemberDisplayFlags.Default;
        private DocumentationMemberDisplayMode _displayMode = DocumentationMemberDisplayMode.Default;

        private readonly IHtmlHelper _htmlHelper;
        private readonly List<DocumentationMemberKind> _memberKinds = [];
        private string? _namespaceName;
        private string? _objectType = "Class";
        private string? _packageId;
        private string? _referenceTitle = "Reference documentation";
        private string? _title = "Methods";
        private string? _version;

        /// <summary>
        ///     Gets the documented object name rendered by this builder.
        /// </summary>
        public string ObjectName { get; }

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DocumentationObjectMembersBuilder" /> class.
        /// </summary>
        /// <param name="htmlHelper">The current Razor HTML helper.</param>
        /// <param name="objectName">Exact documented object name.</param>
        public DocumentationObjectMembersBuilder(IHtmlHelper htmlHelper, string objectName)
        {
            _htmlHelper = htmlHelper;
            ObjectName = objectName;
        }

        #endregion

        #region Instance methods

        /// <summary>
        ///     Sets the display flags used by the rendered member list.
        /// </summary>
        /// <param name="flags">The display flags to apply.</param>
        /// <returns>The configured builder.</returns>
        public DocumentationObjectMembersBuilder Display(DocumentationMemberDisplayFlags flags)
        {
            _displayFlags = flags;
            _displayMode = DocumentationMemberDisplayMode.Default;
            return this;
        }

        /// <summary>
        ///     Sets a common display preset used by the rendered member list.
        /// </summary>
        /// <param name="displayMode">The display preset to apply.</param>
        /// <returns>The configured builder.</returns>
        public DocumentationObjectMembersBuilder Display(DocumentationMemberDisplayMode displayMode)
        {
            _displayMode = displayMode;
            return this;
        }

        /// <summary>
        ///     Renders signatures and optionally includes their summary description.
        /// </summary>
        /// <param name="includeDescription">A value indicating whether the summary description should be rendered.</param>
        /// <returns>The configured builder.</returns>
        public DocumentationObjectMembersBuilder DisplaySignatures(bool includeDescription = false)
        {
            return Display(includeDescription
                ? DocumentationMemberDisplayMode.SignatureAndDescription
                : DocumentationMemberDisplayMode.SignatureOnly);
        }

        /// <summary>
        ///     Filters the reference object as a documented class.
        /// </summary>
        /// <returns>The configured builder.</returns>
        public DocumentationObjectMembersBuilder ForClass()
        {
            _objectType = "Class";
            return this;
        }

        /// <summary>
        ///     Filters the reference object by its documented object type.
        /// </summary>
        /// <param name="objectType">The object type stored in the documentation database.</param>
        /// <returns>The configured builder.</returns>
        public DocumentationObjectMembersBuilder ForObjectType(string? objectType)
        {
            _objectType = objectType;
            return this;
        }

        /// <summary>
        ///     Filters the reference object by namespace.
        /// </summary>
        /// <param name="namespaceName">The namespace stored in the documentation database.</param>
        /// <returns>The configured builder.</returns>
        public DocumentationObjectMembersBuilder InNamespace(string? namespaceName)
        {
            _namespaceName = namespaceName;
            return this;
        }

        /// <summary>
        ///     Filters the reference object by package identifier.
        /// </summary>
        /// <param name="packageId">The package identifier stored in the documentation database.</param>
        /// <returns>The configured builder.</returns>
        public DocumentationObjectMembersBuilder InPackage(string? packageId)
        {
            _packageId = packageId;
            return this;
        }

        /// <summary>
        ///     Filters the reference object by package version.
        /// </summary>
        /// <param name="version">The package version stored in the documentation database.</param>
        /// <returns>The configured builder.</returns>
        public DocumentationObjectMembersBuilder InVersion(string? version)
        {
            _version = version;
            return this;
        }

        /// <summary>
        ///     Sets the reference link label.
        /// </summary>
        /// <param name="referenceTitle">The label displayed for the reference documentation link.</param>
        /// <returns>The configured builder.</returns>
        public DocumentationObjectMembersBuilder ReferenceTitle(string? referenceTitle)
        {
            _referenceTitle = referenceTitle;
            return this;
        }

        /// <summary>
        ///     Renders the configured documentation member list.
        /// </summary>
        /// <returns>The rendered member list content.</returns>
        public Task<IHtmlContent> RenderAsync()
        {
            IViewComponentHelper componentHelper = _htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IViewComponentHelper>();

            if (componentHelper is IViewContextAware contextAware)
            {
                contextAware.Contextualize(_htmlHelper.ViewContext);
            }

            string[] memberKinds = _memberKinds
                .Select(memberKind => memberKind.ToString())
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            return componentHelper.InvokeAsync("DocumentationObjectMembers", new
            {
                objectName = ObjectName,
                memberKinds,
                packageId = _packageId,
                version = _version,
                namespaceName = _namespaceName,
                objectType = _objectType,
                title = _title,
                displayFlags = _displayFlags,
                displayMode = _displayMode,
                referenceTitle = _referenceTitle
            });
        }

        /// <summary>
        ///     Adds constructors to the rendered member list.
        /// </summary>
        /// <returns>The configured builder.</returns>
        public DocumentationObjectMembersBuilder ShowConstructors()
        {
            return ShowMemberKind(DocumentationMemberKind.Constructor);
        }

        /// <summary>
        ///     Adds extension methods to the rendered member list.
        /// </summary>
        /// <returns>The configured builder.</returns>
        public DocumentationObjectMembersBuilder ShowExtensionMethods()
        {
            return ShowMemberKind(DocumentationMemberKind.ExtensionMethod);
        }

        /// <summary>
        ///     Adds fields to the rendered member list.
        /// </summary>
        /// <returns>The configured builder.</returns>
        public DocumentationObjectMembersBuilder ShowFields()
        {
            return ShowMemberKind(DocumentationMemberKind.Field);
        }

        /// <summary>
        ///     Adds one member kind to the rendered member list.
        /// </summary>
        /// <param name="memberKind">The member kind to render.</param>
        /// <returns>The configured builder.</returns>
        public DocumentationObjectMembersBuilder ShowMemberKind(DocumentationMemberKind memberKind)
        {
            _memberKinds.Add(memberKind);
            return this;
        }

        /// <summary>
        ///     Adds every supported member kind to the rendered member list.
        /// </summary>
        /// <returns>The configured builder.</returns>
        public DocumentationObjectMembersBuilder ShowMembers()
        {
            return ShowConstructors()
                .ShowFields()
                .ShowProperties()
                .ShowMethods()
                .ShowExtensionMethods();
        }

        /// <summary>
        ///     Adds methods to the rendered member list.
        /// </summary>
        /// <returns>The configured builder.</returns>
        public DocumentationObjectMembersBuilder ShowMethods()
        {
            return ShowMemberKind(DocumentationMemberKind.Method);
        }

        /// <summary>
        ///     Adds properties to the rendered member list.
        /// </summary>
        /// <returns>The configured builder.</returns>
        public DocumentationObjectMembersBuilder ShowProperties()
        {
            return ShowMemberKind(DocumentationMemberKind.Property);
        }

        /// <summary>
        ///     Sets the title displayed above the rendered member list.
        /// </summary>
        /// <param name="title">The title displayed above the member list.</param>
        /// <returns>The configured builder.</returns>
        public DocumentationObjectMembersBuilder Title(string? title)
        {
            _title = title;
            return this;
        }

        /// <summary>
        ///     Hides the reference documentation link.
        /// </summary>
        /// <returns>The configured builder.</returns>
        public DocumentationObjectMembersBuilder WithoutReferenceLink()
        {
            _displayFlags &= ~DocumentationMemberDisplayFlags.ReferenceLink;
            return this;
        }

        #endregion
    }
}