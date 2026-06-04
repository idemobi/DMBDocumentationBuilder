#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.IO;
using DMBBootstrapBuilder;
using DMBDocumentationBuilderLabs.Navigation;
using Microsoft.AspNetCore.Mvc.Rendering;

#endregion

namespace DMBDocumentationBuilderWebsite;

/// <summary>
///     Provides the local DMBDocumentationBuilder website navbar module composition.
/// </summary>
internal sealed class DMBDocumentationBuilderWebsiteMenuBarSectionProvider : IMenuBarSectionProvider
{
    #region Instance properties

    /// <inheritdoc />
    public int Order => 100;

    #endregion

    #region Instance methods

    /// <inheritdoc />
    public MenuBarModuleResult Build(TextWriter writer, IHtmlHelper html)
    {
        MenuBarModuleResult result = new();

        result.ActionList.Add(DMBDocumentationBuilderLabsNavigationAgent.CreateMenuGroup());

        return result;
    }

    /// <inheritdoc />
    public bool IsEnabled(IHtmlHelper html)
    {
        return true;
    }

    #endregion
}
