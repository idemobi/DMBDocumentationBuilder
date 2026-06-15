#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBBootstrapBuilder;
using DMBComponentBuilder;
using DMBDocumentationBuilderLabs.Controllers;
using DMBDocumentationBuilderLabs.Navigation;
using DMBDocumentationBuilderWebsite;
using DMBDocumentationViewer;
using DMBEffectBuilder;
using DMBPageBuilder;
using DMBServerHelper;
using DMBServerWebHelper;

#endregion

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

ServerHelperConfiguration.LoadCommonConfig(builder);
ServerHelperConfiguration.Config.CookiePrefix = "DDB";
ServerWebHelperConfiguration.LoadCommonConfig(builder);
PageBuilderConfiguration.LoadCommonConfig(builder);
BootstrapBuilderConfiguration.LoadCommonConfig(builder);
ComponentBuilderConfiguration.LoadCommonConfig(builder);
EffectBuilderConfiguration.LoadCommonConfig(builder);
DMBDocumentationViewerConfiguration.LoadCommonConfig(builder);

DocumentationSidebarFactory.Provider = new DMBDocumentationBuilderLabsDocumentationSidebarProvider();

var mvcBuilder = builder.Services.AddControllersWithViews();
mvcBuilder.AddApplicationPart(typeof(DocumentationBuilderController).Assembly);
mvcBuilder.AddApplicationPart(typeof(DocumentationController).Assembly);
mvcBuilder.AddMvcOptions(options => options.Filters.Add(new DMBDocumentationBuilderWebsiteSidebarActionFilter()));

builder.Services.AddTransient<IMenuBarSectionProvider, DMBDocumentationBuilderWebsiteMenuBarSectionProvider>();
builder.Services.AddTransient<IProfileBarSectionProvider, ThemeBarSectionProvider>();
builder.Services.AddTransient<IProfileBarSectionProvider, DebugBarSectionProvider>();

WebApplication app = builder.Build();

app.UseHttpsRedirection();

ServerWebHelperConfiguration.UseApp(app);
DMBDocumentationViewerConfiguration.UseApp(app);

app.MapGet("/", context =>
{
    context.Response.Redirect("/DocumentationHome/Index");
    return Task.CompletedTask;
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=DocumentationHome}/{action=Index}/{id?}");

app.Run();
