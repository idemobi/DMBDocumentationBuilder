#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Adds embedded DocumentationViewer static web assets to ASP.NET Core static file options.
    /// </summary>
    /// <remarks>
    ///     This post-configuration composes the host web root provider with the embedded <c>wwwroot</c> provider so
    ///     DocumentationViewer CSS and JavaScript can be served in project-reference and NuGet consumption modes.
    /// </remarks>
    public sealed class DocumentationViewerConfigureOptions : IPostConfigureOptions<StaticFileOptions>
    {
        #region Constants

        private const string BasePath = "wwwroot";

        #endregion

        #region Instance fields and properties

        private IWebHostEnvironment Environment { get; }

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DocumentationViewerConfigureOptions" /> class.
        /// </summary>
        /// <param name="environment">The web host environment that provides the application web root file provider.</param>
        public DocumentationViewerConfigureOptions(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        #endregion

        #region Instance methods

        #region From interface IPostConfigureOptions<StaticFileOptions>

        /// <summary>
        ///     Composes the current static file provider with the embedded DocumentationViewer asset provider.
        /// </summary>
        /// <param name="name">The options name supplied by the ASP.NET Core options pipeline.</param>
        /// <param name="options">The static file options to update.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="name" /> or <paramref name="options" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when neither the options nor the host environment provide a file provider.
        /// </exception>
        public void PostConfigure(string? name, StaticFileOptions options)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));
            options = options ?? throw new ArgumentNullException(nameof(options));

            options.ContentTypeProvider ??= new FileExtensionContentTypeProvider();

            if (options.FileProvider == null && Environment.WebRootFileProvider == null)
            {
                throw new InvalidOperationException("Missing FileProvider.");
            }

            options.FileProvider ??= Environment.WebRootFileProvider;
            ManifestEmbeddedFileProvider filesProvider = new(GetType().Assembly, BasePath);
            options.FileProvider = new CompositeFileProvider(options.FileProvider, filesProvider);
        }

        #endregion

        #endregion
    }
}