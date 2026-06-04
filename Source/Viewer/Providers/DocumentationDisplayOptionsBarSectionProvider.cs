#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.IO;
using DMBBootstrapBuilder;
using DMBPageBuilder;
using Microsoft.AspNetCore.Mvc.Rendering;

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Provides documentation display options inside the BootstrapBuilder profile navbar.
    /// </summary>
    public sealed class DocumentationDisplayOptionsBarSectionProvider : IProfileBarSectionProvider
    {
        #region Static methods

        private static ToggleActionItem CreateAccessToggle(string title, string access, string iconBootstrap)
        {
            return new ToggleActionItem
                {
                    Title = title,
                    Icon = IconStruct.Bootstrap(iconBootstrap),
                    SwitchValue = true,
                    SwitchJavaScript = $"DocumentationDisplayOptions.setAccess('{access}', this.checked);"
                }
                .SetDataAttribut("doc-display-group", "access")
                .SetDataAttribut("doc-display-toggle", access);
        }

        private static ToggleActionItem CreateKindToggle(string title, string kind, string iconBootstrap)
        {
            return new ToggleActionItem
                {
                    Title = title,
                    Icon = IconStruct.Bootstrap(iconBootstrap),
                    SwitchValue = true,
                    SwitchJavaScript = $"DocumentationDisplayOptions.setKind('{kind}', this.checked);"
                }
                .SetDataAttribut("doc-display-group", "kind")
                .SetDataAttribut("doc-display-toggle", kind);
        }

        #endregion

        #region Instance fields and properties

        #region From interface IProfileBarSectionProvider

        /// <summary>
        ///     Gets the navbar order used to place display options next to the customization menu.
        /// </summary>
        public int Order => 2;

        #endregion

        #endregion

        #region Instance methods

        #region From interface IProfileBarSectionProvider

        /// <summary>
        ///     Builds the display option menu for generated documentation pages.
        /// </summary>
        /// <param name="writer">The writer that receives the rendered HTML output.</param>
        /// <param name="html">The Razor HTML helper used to access view context and services.</param>
        /// <returns>The configured <see cref="ProfilBarModuleResult" /> value.</returns>
        public ProfilBarModuleResult Build(TextWriter writer, IHtmlHelper html)
        {
            PageInformation page = PageRegistry.GetOrCreatePageInformation(html.ViewContext.HttpContext);
            page.SetStylesheet("/css/documentation/DocumentationDisplayOptions.css");
            page.SetScriptFile("/js/documentation/DocumentationDisplayOptions.js", PageScriptLocation.EndOfBody, PageScriptLoadingMode.Defer);

            ProfilBarModuleResult result = new();
            GroupActionItem group = new GroupActionItem("Display", IconStruct.Bootstrap("bi-sliders"))
                .SetAttribut("data-doc-display-root", true)
                .SetAttribut("aria-disabled", "true");

            GroupActionItem members = new GroupActionItem("Members", IconStruct.Bootstrap("bi-list-ul"));
            group.AddItem(members);
            members.AddItem(CreateKindToggle("Constructors", "constructor", "bi-braces"));
            members.AddItem(CreateKindToggle("Fields", "field", "bi-input-cursor"));
            members.AddItem(CreateKindToggle("Properties", "property", "bi-card-list"));
            members.AddItem(CreateKindToggle("Methods", "method", "bi-code-slash"));
            members.AddItem(CreateKindToggle("Events", "event", "bi-broadcast"));
            members.AddItem(CreateKindToggle("Extensions", "extension-method", "bi-plugin"));

            group.AddItem(new DividerActionItem());

            GroupActionItem access = new GroupActionItem("Access", IconStruct.Bootstrap("bi-shield-lock"));
            group.AddItem(access);
            access.AddItem(CreateAccessToggle("Public", "public", "bi-unlock"));
            access.AddItem(CreateAccessToggle("Protected", "protected", "bi-shield-check"));
            access.AddItem(CreateAccessToggle("Internal", "internal", "bi-diagram-3"));
            access.AddItem(CreateAccessToggle("Private", "private", "bi-lock"));

            group.AddItem(new DividerActionItem());

            group.AddItem(new JavaScriptActionItem("DocumentationDisplayOptions.setPreset('public-api'); return false;")
                .SetTitle("Public API")
                .SetIcon(IconStruct.Bootstrap("bi-box-arrow-up-right"))
                .SetDataAttribut("doc-display-action", true));
            group.AddItem(new JavaScriptActionItem("DocumentationDisplayOptions.setPreset('all'); return false;")
                .SetTitle("All members")
                .SetIcon(IconStruct.Bootstrap("bi-ui-checks-grid"))
                .SetDataAttribut("doc-display-action", true));
            group.AddItem(new JavaScriptActionItem("return false;")
                .SetTitle("Visible members")
                .SetIcon(IconStruct.Bootstrap("bi-eye"))
                .SetBadge("0/0", VariantStyle.Secondary)
                .SetDataAttribut("doc-display-action", true)
                .SetDataAttribut("doc-display-counter", true));

            result.ActionList.Add(group);
            return result;
        }

        /// <summary>
        ///     Determines whether display options should be shown for the current request.
        /// </summary>
        /// <param name="html">The Razor HTML helper used to inspect route data.</param>
        /// <returns>True when the current route belongs to <see cref="Controllers.DocumentationController" />; otherwise, false.</returns>
        public bool IsEnabled(IHtmlHelper html)
        {
            string? controller = html.ViewContext.RouteData.Values["controller"]?.ToString();
            return string.Equals(controller, "Documentation", StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #endregion
    }
}