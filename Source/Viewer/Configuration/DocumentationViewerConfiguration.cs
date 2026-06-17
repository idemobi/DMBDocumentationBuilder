#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBBootstrapBuilder;
using DMBComponentBuilder;
using DMBPageBuilder;
using DMBServerWebHelper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Configures the DocumentationViewer MVC and MCP integration for a host web application.
    /// </summary>
    [Serializable]
    public class DocumentationViewerConfiguration : WebGenericConfiguration<DocumentationViewerConfiguration>, IServerWebConfig
    {
        #region Static constructors and destructors

        static DocumentationViewerConfiguration()
        {
        }

        #endregion

        #region Static methods

        private static bool IsComponentBuilderStaticFileOptionsRegistration(ServiceDescriptor descriptor)
        {
            return descriptor.ServiceType == typeof(IPostConfigureOptions<StaticFileOptions>) &&
                   descriptor.ImplementationType == typeof(ComponentBuilderConfigureOptions);
        }

        /// <summary>
        ///     Maps the DocumentationViewer MCP endpoint on the host application.
        /// </summary>
        /// <param name="app">The configured web application.</param>
        public static void UseApp(WebApplication app)
        {
            app.MapMcp(Config.McpEndpoint);
        }

        #endregion

        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the route used to expose the MCP server endpoint.
        /// </summary>
        /// <value>The endpoint path. The default value is `/mcp`.</value>
        public string McpEndpoint { get; set; } = "/mcp";

        #endregion

        #region Instance methods

        #region From interface IServerWebConfig

        /// <summary>
        ///     Registers the stateless MCP server and DocumentationViewer tool assembly after base configuration is available.
        /// </summary>
        /// <param name="appBuilder">The host application builder used to register services.</param>
        /// <param name="configBuilder">The configuration builder supplied by the host pipeline.</param>
        /// <param name="configRoot">The resolved configuration root supplied by the host pipeline.</param>
        public override void AfterConfiguration(IHostApplicationBuilder appBuilder, IConfigurationBuilder configBuilder, IConfigurationRoot configRoot)
        {
            appBuilder.Services
                .AddMcpServer()
                .WithHttpTransport(options => { options.Stateless = true; })
                .WithToolsFromAssembly(typeof(DocumentationMcpTools).Assembly);
            appBuilder.Services.ConfigureOptions<DocumentationViewerConfigureOptions>();
            if (!appBuilder.Services.Any(IsComponentBuilderStaticFileOptionsRegistration))
            {
                appBuilder.Services.ConfigureOptions<ComponentBuilderConfigureOptions>();
            }

            appBuilder.Services.RegisterGlobalStylesheetAsset(
                "dmb-component-builder-code-block-css",
                "/css/components/CodeBlock.css",
                10);
            appBuilder.Services.RegisterGlobalScriptAsset(
                "dmb-component-builder-code-block-js",
                "/js/components/CodeBlock.js",
                PageScriptLocation.EndOfBody,
                PageScriptLoadingMode.Defer,
                10);
            appBuilder.Services.AddTransient<IProfileBarSectionProvider, DocumentationDisplayOptionsBarSectionProvider>();
        }

        /// <summary>
        ///     Indicates whether this package contributes an API description surface.
        /// </summary>
        /// <returns><see langword="false" /> because the viewer does not expose Swagger-style API descriptions.</returns>
        public override bool ApiDescription()
        {
            return false;
        }

        /// <summary>
        ///     Runs before host configuration is completed.
        /// </summary>
        /// <param name="appBuilder">The host application builder supplied by the configuration pipeline.</param>
        /// <param name="configBuilder">The configuration builder supplied by the configuration pipeline.</param>
        /// <param name="configRoot">The resolved configuration root supplied by the configuration pipeline.</param>
        /// <remarks>
        ///     The viewer has no pre-configuration work by default.
        /// </remarks>
        public override void BeforeConfiguration(IHostApplicationBuilder appBuilder, IConfigurationBuilder configBuilder, IConfigurationRoot configRoot)
        {
        }

        /// <summary>
        ///     Indicates whether the package requires a configuration file or application settings section.
        /// </summary>
        /// <returns><see langword="false" /> because the default viewer configuration can run without a required settings file.</returns>
        public override bool NeedsConfigFileOrAppSettings()
        {
            return false;
        }

        /// <summary>
        ///     Populates randomized configuration values for development scenarios.
        /// </summary>
        /// <remarks>
        ///     The viewer currently has no randomized configuration values.
        /// </remarks>
        public override void RandomFake()
        {
        }

        #endregion

        #endregion
    }
}