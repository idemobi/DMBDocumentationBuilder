#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Text;
using DMBBootstrapBuilder;
using DMBPageBuilder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Renders generated documentation root, group, namespace, and object pages from the documentation database.
    /// </summary>
    public class DocumentationController : RawBootstrapController
    {
        #region Instance fields and properties

        #region Fields

        private readonly IWebHostEnvironment _environment;

        #endregion

        #endregion

        #region Instance constructors and destructors

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="DocumentationController" /> class.
        /// </summary>
        /// <param name="environment">Host environment used to resolve the generated documentation database path.</param>
        public DocumentationController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        #endregion

        #endregion

        #region Instance methods

        private string ResolveAIRenderDatabasePath(string databasePathFromRegistry)
        {
            if (string.IsNullOrWhiteSpace(databasePathFromRegistry))
            {
                return string.Empty;
            }

            if (Path.IsPathRooted(databasePathFromRegistry))
            {
                return databasePathFromRegistry;
            }

            string mainDatabaseDirectory = Path.GetDirectoryName(GetSqliteDatabasePath())
                                           ?? throw new InvalidOperationException("Unable to resolve main database directory.");

            return Path.GetFullPath(Path.Combine(mainDatabaseDirectory, databasePathFromRegistry));
        }

        #endregion

        #region Nested types

        private sealed class DocumentationAIRenderSource
        {
            #region Instance fields and properties

            /// <summary>
            ///     Gets the configured AI-render SQLite database path.
            /// </summary>
            public string DatabasePath { get; init; } = string.Empty;

            /// <summary>
            ///     Gets a value indicating whether this AI-render source is enabled.
            /// </summary>
            public bool IsEnabled { get; init; }

            /// <summary>
            ///     Gets the model identifier for this AI-render source.
            /// </summary>
            public string Model { get; init; } = string.Empty;

            /// <summary>
            ///     Gets the provider identifier for this AI-render source.
            /// </summary>
            public string Provider { get; init; } = string.Empty;

            #endregion
        }

        private sealed class DocumentationSidebarDatabaseItem
        {
            #region Instance fields and properties

            /// <summary>
            ///     Gets the MVC action name associated with the sidebar item.
            /// </summary>
            public string ActionName { get; init; } = string.Empty;

            /// <summary>
            ///     Gets the MVC controller name associated with the sidebar item.
            /// </summary>
            public string ControllerName { get; init; } = string.Empty;

            /// <summary>
            ///     Gets the documentation group name displayed by the sidebar item.
            /// </summary>
            public string GroupName { get; init; } = string.Empty;

            /// <summary>
            ///     Gets the icon CSS class or identifier associated with the sidebar item.
            /// </summary>
            public string Icon { get; init; } = string.Empty;

            /// <summary>
            ///     Gets the stable sidebar item key.
            /// </summary>
            public string ItemKey { get; init; } = string.Empty;

            /// <summary>
            ///     Gets the kind of documentation item represented by this sidebar entry.
            /// </summary>
            public string ItemKind { get; init; } = string.Empty;

            /// <summary>
            ///     Gets the namespace name displayed by the sidebar item.
            /// </summary>
            public string NamespaceName { get; init; } = string.Empty;

            /// <summary>
            ///     Gets the package identifier displayed by the sidebar item.
            /// </summary>
            public string PackageId { get; init; } = string.Empty;

            /// <summary>
            ///     Gets the parent sidebar item key.
            /// </summary>
            public string ParentItemKey { get; init; } = string.Empty;

            /// <summary>
            ///     Gets the route group name used to build links.
            /// </summary>
            public string RouteGroupName { get; init; } = string.Empty;

            /// <summary>
            ///     Gets the route namespace name used to build links.
            /// </summary>
            public string RouteNamespaceName { get; init; } = string.Empty;

            /// <summary>
            ///     Gets the route object name used to build links.
            /// </summary>
            public string RouteObjectName { get; init; } = string.Empty;

            /// <summary>
            ///     Gets the route package identifier used to build links.
            /// </summary>
            public string RoutePackageId { get; init; } = string.Empty;

            /// <summary>
            ///     Gets the route version used to build links.
            /// </summary>
            public string RouteVersion { get; init; } = string.Empty;

            /// <summary>
            ///     Gets the sidebar grouping kind.
            /// </summary>
            public string SidebarKind { get; init; } = string.Empty;

            /// <summary>
            ///     Gets the sidebar sort order.
            /// </summary>
            public int SortOrder { get; init; }

            /// <summary>
            ///     Gets the title displayed by the sidebar item.
            /// </summary>
            public string Title { get; init; } = string.Empty;

            /// <summary>
            ///     Gets the package version displayed by the sidebar item.
            /// </summary>
            public string Version { get; init; } = string.Empty;

            #endregion
        }

        #endregion

        #region Private methods

        private string GetSqliteDatabasePath()
        {
            return Path.Combine(_environment.ContentRootPath, "Documentation", "data.db");
        }

        private static string MergeKeywords(string? technical, string? text)
        {
            return $"{technical ?? string.Empty} {text ?? string.Empty}".Trim();
        }

        private SideBarSectionComponent CreateDatabaseRootSidebar()
        {
            return CreateDatabaseSidebar("Root", "Documentation", string.Empty, string.Empty, string.Empty, string.Empty);
        }

        private SideBarSectionComponent CreateDatabaseGroupSidebar(string groupName, string version)
        {
            return CreateDatabaseSidebar("Group", groupName, groupName, string.Empty, version, string.Empty);
        }

        private SideBarSectionComponent CreateDatabaseNamespaceSidebar(
            string groupName,
            string namespaceName,
            string? packageId,
            string? version
        )
        {
            return CreateDatabaseSidebar(
                "Namespace",
                namespaceName,
                groupName,
                packageId ?? string.Empty,
                version ?? string.Empty,
                namespaceName);
        }

        private SideBarSectionComponent CreateDatabaseProjectSidebar(
            string groupName,
            string packageId,
            string version
        )
        {
            return CreateDatabaseSidebar(
                "Project",
                packageId,
                groupName,
                packageId,
                version,
                string.Empty);
        }

        private SideBarSectionComponent CreateMarkdownContentSidebar(
            string groupName,
            string packageId,
            string version,
            string? sidebarNamespaceName
        )
        {
            string resolvedSidebarNamespaceName = string.IsNullOrWhiteSpace(sidebarNamespaceName)
                ? ResolveDefaultSidebarNamespaceName(groupName, packageId, version)
                : sidebarNamespaceName;

            return string.IsNullOrWhiteSpace(resolvedSidebarNamespaceName)
                ? CreateDatabaseProjectSidebar(groupName, packageId, version)
                : CreateDatabaseNamespaceSidebar(groupName, resolvedSidebarNamespaceName, packageId, version);
        }

        private SideBarSectionComponent CreateOpenApiSidebar(
            string groupName,
            string packageId,
            string version,
            string? sidebarNamespaceName
        )
        {
            return string.IsNullOrWhiteSpace(sidebarNamespaceName)
                ? CreateDatabaseProjectSidebar(groupName, packageId, version)
                : CreateDatabaseNamespaceSidebar(groupName, sidebarNamespaceName, packageId, version);
        }

        private SideBarSectionComponent CreateDatabaseSidebar(
            string sidebarKind,
            string sectionTitle,
            string groupName,
            string packageId,
            string version,
            string namespaceName
        )
        {
            List<DocumentationSidebarDatabaseItem> items = GetDatabaseSidebarItems(
                sidebarKind,
                groupName,
                packageId,
                version,
                namespaceName);

            SideBarSectionComponent sidebar = new(sectionTitle);

            foreach (IActionItem item in BuildDatabaseSidebarItems(items, string.Empty))
            {
                sidebar.Add(item);
            }

            return sidebar;
        }

        private string ResolveDefaultSidebarNamespaceName(string groupName, string packageId, string version)
        {
            using var connection = new SqliteConnection($"Data Source={GetSqliteDatabasePath()}");
            connection.Open();

            if (!TableExists(connection, "DocumentationSidebarItems"))
            {
                return string.Empty;
            }

            const string sql = """
                               SELECT NamespaceName
                               FROM DocumentationSidebarItems
                               WHERE SidebarKind = 'Namespace'
                                 AND GroupName = @GroupName
                                 AND PackageId = @PackageId
                                 AND Version = @Version
                                 AND NamespaceName <> ''
                               GROUP BY NamespaceName
                               ORDER BY NamespaceName
                               LIMIT 1
                               """;

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@GroupName", groupName);
            command.Parameters.AddWithValue("@PackageId", packageId);
            command.Parameters.AddWithValue("@Version", version);

            return command.ExecuteScalar()?.ToString() ?? string.Empty;
        }

        private IReadOnlyList<IActionItem> BuildDatabaseSidebarItems(
            IReadOnlyCollection<DocumentationSidebarDatabaseItem> items,
            string parentItemKey
        )
        {
            List<IActionItem> result = [];

            foreach (DocumentationSidebarDatabaseItem item in items
                         .Where(item => string.Equals(item.ParentItemKey, parentItemKey, StringComparison.Ordinal))
                         .OrderBy(item => item.SortOrder)
                         .ThenBy(item => item.Title, StringComparer.Ordinal))
            {
                IActionItem actionItem = CreateDatabaseSidebarActionItem(item);

                if (actionItem is IActionContainerItem container)
                {
                    foreach (IActionItem child in BuildDatabaseSidebarItems(items, item.ItemKey))
                    {
                        container.Items.Add(child);
                    }
                }

                result.Add(actionItem);
            }

            return result;
        }

        private static IActionItem CreateDatabaseSidebarActionItem(DocumentationSidebarDatabaseItem item)
        {
            if (string.Equals(item.ItemKind, "Group", StringComparison.OrdinalIgnoreCase))
            {
                return ActionItemFactory.Group(item.Title, CreateIcon(item.Icon));
            }

            AspRouteActionItem actionItem = ActionItemFactory
                .AspRoute(item.ControllerName, item.ActionName, string.Empty)
                .SetTitle(item.Title);

            if (!string.IsNullOrWhiteSpace(item.Icon))
            {
                actionItem.SetIcon(CreateIcon(item.Icon));
            }

            AddRouteValue(actionItem, "groupName", item.RouteGroupName);
            AddRouteValue(actionItem, "packageId", item.RoutePackageId);
            AddRouteValue(actionItem, "version", item.RouteVersion);
            AddRouteValue(actionItem, "namespaceName", item.RouteNamespaceName);
            AddRouteValue(actionItem, "objectName", item.RouteObjectName);
            AddRouteValue(actionItem, "sidebarNamespaceName", GetSidebarNamespaceRouteValue(item));

            return actionItem;
        }

        private static string GetSidebarNamespaceRouteValue(DocumentationSidebarDatabaseItem item)
        {
            if (!string.Equals(item.SidebarKind, "Namespace", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            bool isDocumentationContentRoute =
                string.Equals(item.ControllerName, "DocumentationContent", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(item.ActionName, "ShowContent", StringComparison.OrdinalIgnoreCase);

            bool isOpenApiRoute =
                string.Equals(item.ControllerName, "Documentation", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(item.ActionName, "ShowOpenApi", StringComparison.OrdinalIgnoreCase);

            return isDocumentationContentRoute || isOpenApiRoute
                ? item.NamespaceName
                : string.Empty;
        }

        private static void AddRouteValue(AspRouteActionItem actionItem, string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                actionItem.AddRouteValue(key, value);
            }
        }

        private static IconStruct CreateIcon(string icon)
        {
            return string.IsNullOrWhiteSpace(icon)
                ? IconStruct.Empty
                : IconStruct.Bootstrap(icon);
        }

        private List<DocumentationSidebarDatabaseItem> GetDatabaseSidebarItems(
            string sidebarKind,
            string groupName,
            string packageId,
            string version,
            string namespaceName
        )
        {
            using var connection = new SqliteConnection($"Data Source={GetSqliteDatabasePath()}");
            connection.Open();

            if (!TableExists(connection, "DocumentationSidebarItems"))
            {
                return [];
            }

            const string sql = """
                               SELECT SidebarKind, GroupName, PackageId, Version, NamespaceName, ItemKey, ParentItemKey, ItemKind,
                                      Title, Icon, ControllerName, ActionName, RouteGroupName, RoutePackageId, RouteVersion,
                                      RouteNamespaceName, RouteObjectName, SortOrder
                               FROM DocumentationSidebarItems
                               WHERE SidebarKind = @SidebarKind
                                 AND GroupName = @GroupName
                                 AND PackageId = @PackageId
                                 AND Version = @Version
                                 AND NamespaceName = @NamespaceName
                               ORDER BY ParentItemKey, SortOrder, Title
                               """;

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@SidebarKind", sidebarKind);
            command.Parameters.AddWithValue("@GroupName", groupName);
            command.Parameters.AddWithValue("@PackageId", packageId);
            command.Parameters.AddWithValue("@Version", version);
            command.Parameters.AddWithValue("@NamespaceName", namespaceName);

            List<DocumentationSidebarDatabaseItem> items = ReadDatabaseSidebarItems(command);

            if (string.Equals(sidebarKind, "Root", StringComparison.OrdinalIgnoreCase))
            {
                items = ApplyRootSidebarGroupRouteVersions(items);
            }

            if (!string.IsNullOrWhiteSpace(version))
            {
                items = ApplyGroupSidebarRouteVersion(items, version);
            }

            if (items.Count > 0 ||
                !string.Equals(sidebarKind, "Group", StringComparison.OrdinalIgnoreCase) ||
                string.IsNullOrWhiteSpace(version))
            {
                return items;
            }

            command.Parameters["@Version"].Value = string.Empty;
            return FilterDatabaseGroupSidebarItemsByVersion(
                ReadDatabaseSidebarItems(command),
                version);
        }

        private static List<DocumentationSidebarDatabaseItem> ReadDatabaseSidebarItems(SqliteCommand command)
        {
            using var reader = command.ExecuteReader();
            List<DocumentationSidebarDatabaseItem> items = [];

            while (reader.Read())
            {
                items.Add(new DocumentationSidebarDatabaseItem
                {
                    SidebarKind = reader["SidebarKind"]?.ToString() ?? string.Empty,
                    GroupName = reader["GroupName"]?.ToString() ?? string.Empty,
                    PackageId = reader["PackageId"]?.ToString() ?? string.Empty,
                    Version = reader["Version"]?.ToString() ?? string.Empty,
                    NamespaceName = reader["NamespaceName"]?.ToString() ?? string.Empty,
                    ItemKey = reader["ItemKey"]?.ToString() ?? string.Empty,
                    ParentItemKey = reader["ParentItemKey"]?.ToString() ?? string.Empty,
                    ItemKind = reader["ItemKind"]?.ToString() ?? string.Empty,
                    Title = reader["Title"]?.ToString() ?? string.Empty,
                    Icon = reader["Icon"]?.ToString() ?? string.Empty,
                    ControllerName = reader["ControllerName"]?.ToString() ?? string.Empty,
                    ActionName = reader["ActionName"]?.ToString() ?? string.Empty,
                    RouteGroupName = reader["RouteGroupName"]?.ToString() ?? string.Empty,
                    RoutePackageId = reader["RoutePackageId"]?.ToString() ?? string.Empty,
                    RouteVersion = reader["RouteVersion"]?.ToString() ?? string.Empty,
                    RouteNamespaceName = reader["RouteNamespaceName"]?.ToString() ?? string.Empty,
                    RouteObjectName = reader["RouteObjectName"]?.ToString() ?? string.Empty,
                    SortOrder = Convert.ToInt32(reader["SortOrder"])
                });
            }

            return items;
        }

        private static List<DocumentationSidebarDatabaseItem> FilterDatabaseGroupSidebarItemsByVersion(
            IReadOnlyCollection<DocumentationSidebarDatabaseItem> items,
            string version
        )
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                return items.ToList();
            }

            Dictionary<string, DocumentationSidebarDatabaseItem> itemsByKey = items
                .Where(item => !string.IsNullOrWhiteSpace(item.ItemKey))
                .ToDictionary(item => item.ItemKey, StringComparer.Ordinal);

            HashSet<string> includedItemKeys = new(StringComparer.Ordinal);

            foreach (DocumentationSidebarDatabaseItem item in items)
            {
                bool isSelectedVersionRoute = string.Equals(item.RouteVersion, version, StringComparison.OrdinalIgnoreCase);
                bool isGroupHomeRoute = string.Equals(item.ActionName, nameof(ShowGroup), StringComparison.OrdinalIgnoreCase) &&
                                        string.IsNullOrWhiteSpace(item.RoutePackageId) &&
                                        string.IsNullOrWhiteSpace(item.RouteVersion);

                if (!isSelectedVersionRoute && !isGroupHomeRoute)
                {
                    continue;
                }

                IncludeItemAndParents(item, itemsByKey, includedItemKeys);
            }

            return items
                .Where(item => includedItemKeys.Contains(item.ItemKey))
                .Select(item => ApplyGroupSidebarRouteVersion(item, version))
                .ToList();
        }

        private static void IncludeItemAndParents(
            DocumentationSidebarDatabaseItem item,
            IReadOnlyDictionary<string, DocumentationSidebarDatabaseItem> itemsByKey,
            ISet<string> includedItemKeys
        )
        {
            DocumentationSidebarDatabaseItem currentItem = item;

            while (!string.IsNullOrWhiteSpace(currentItem.ItemKey) &&
                   includedItemKeys.Add(currentItem.ItemKey) &&
                   !string.IsNullOrWhiteSpace(currentItem.ParentItemKey) &&
                   itemsByKey.TryGetValue(currentItem.ParentItemKey, out DocumentationSidebarDatabaseItem? parentItem))
            {
                currentItem = parentItem;
            }
        }

        private static DocumentationSidebarDatabaseItem ApplyGroupSidebarRouteVersion(
            DocumentationSidebarDatabaseItem item,
            string version
        )
        {
            if (!string.Equals(item.ActionName, nameof(ShowGroup), StringComparison.OrdinalIgnoreCase) ||
                !string.IsNullOrWhiteSpace(item.RouteVersion))
            {
                return item;
            }

            return new DocumentationSidebarDatabaseItem
            {
                ActionName = item.ActionName,
                ControllerName = item.ControllerName,
                GroupName = item.GroupName,
                Icon = item.Icon,
                ItemKey = item.ItemKey,
                ItemKind = item.ItemKind,
                NamespaceName = item.NamespaceName,
                PackageId = item.PackageId,
                ParentItemKey = item.ParentItemKey,
                RouteGroupName = item.RouteGroupName,
                RouteNamespaceName = item.RouteNamespaceName,
                RouteObjectName = item.RouteObjectName,
                RoutePackageId = item.RoutePackageId,
                RouteVersion = version,
                SidebarKind = item.SidebarKind,
                SortOrder = item.SortOrder,
                Title = item.Title,
                Version = item.Version
            };
        }

        private static List<DocumentationSidebarDatabaseItem> ApplyGroupSidebarRouteVersion(
            IEnumerable<DocumentationSidebarDatabaseItem> items,
            string version
        )
        {
            return items
                .Select(item => ApplyGroupSidebarRouteVersion(item, version))
                .ToList();
        }

        private List<DocumentationSidebarDatabaseItem> ApplyRootSidebarGroupRouteVersions(
            IEnumerable<DocumentationSidebarDatabaseItem> items
        )
        {
            return items
                .Select(item => string.Equals(item.ActionName, nameof(ShowGroup), StringComparison.OrdinalIgnoreCase) &&
                                string.IsNullOrWhiteSpace(item.RouteVersion)
                    ? ApplyGroupSidebarRouteVersion(
                        item,
                        ResolveLatestGroupSidebarVersion(item.RouteGroupName, null) ?? string.Empty)
                    : item)
                .ToList();
        }

        private string? ResolveLatestGroupSidebarVersion(string groupName, string? version)
        {
            if (!string.IsNullOrWhiteSpace(version))
            {
                return version;
            }

            using var connection = new SqliteConnection($"Data Source={GetSqliteDatabasePath()}");
            connection.Open();

            if (!TableExists(connection, "DocumentationSidebarItems"))
            {
                return version;
            }

            const string sql = """
                               SELECT Version
                               FROM DocumentationSidebarItems
                               WHERE SidebarKind = 'Group'
                                 AND GroupName = @GroupName
                                 AND Version IS NOT NULL
                                 AND Version <> ''
                               UNION
                               SELECT RouteVersion AS Version
                               FROM DocumentationSidebarItems
                               WHERE SidebarKind = 'Group'
                                 AND GroupName = @GroupName
                                 AND RouteVersion IS NOT NULL
                                 AND RouteVersion <> ''
                               """;

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@GroupName", groupName);

            using var reader = command.ExecuteReader();
            List<string> versions = [];

            while (reader.Read())
            {
                string candidateVersion = reader["Version"]?.ToString() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(candidateVersion))
                {
                    versions.Add(candidateVersion);
                }
            }

            return versions
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(candidateVersion => candidateVersion, DocumentationVersionComparer.Instance)
                .FirstOrDefault();
        }

        private static string NormalizeDocumentationVersionLinks(string html, string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                return html;
            }

            return System.Text.RegularExpressions.Regex.Replace(
                html,
                "(?<url>/Documentation/(?:ShowGroup|ShowNamespace)\\?[^\"'<>\\s]*)",
                match =>
                {
                    string url = match.Groups["url"].Value;

                    if (url.Contains("version=", StringComparison.OrdinalIgnoreCase))
                    {
                        return url;
                    }

                    return $"{url}&version={System.Net.WebUtility.UrlEncode(version)}";
                });
        }

        private static string Html(string? value)
        {
            return System.Net.WebUtility.HtmlEncode(value ?? string.Empty);
        }

        private static string BuildDocumentationGroupFallbackHtml(
            string groupName,
            string version,
            IReadOnlyCollection<DocumentationSidebarDatabaseItem> sidebarItems
        )
        {
            List<DocumentationSidebarDatabaseItem> namespaceItems = sidebarItems
                .Where(item => string.Equals(item.ActionName, nameof(ShowNamespace), StringComparison.OrdinalIgnoreCase))
                .Where(item => !string.IsNullOrWhiteSpace(item.RoutePackageId))
                .Where(item => !string.IsNullOrWhiteSpace(item.RouteVersion))
                .Where(item => !string.IsNullOrWhiteSpace(item.RouteNamespaceName))
                .GroupBy(item => $"{item.RoutePackageId}\u001F{item.RouteVersion}\u001F{item.RouteNamespaceName}", StringComparer.Ordinal)
                .Select(group => group.First())
                .OrderBy(item => item.RoutePackageId, StringComparer.Ordinal)
                .ThenBy(item => item.RouteNamespaceName, StringComparer.Ordinal)
                .ToList();

            StringBuilder sb = new();

            sb.AppendLine("<div class=\"ex-container py-3 ex-py-lg-5\">");
            sb.AppendLine("    <div class=\"row g-3\">");
            sb.AppendLine("        <div class=\"col-12\">");
            sb.AppendLine("            <div class=\"mb-4\">");
            sb.AppendLine("                <div class=\"d-flex flex-wrap align-items-center gap-2 mb-3\">");
            sb.AppendLine("                    <span class=\"badge text-bg-primary rounded-pill px-2 py-1\">Group</span>");
            sb.Append("                    <span class=\"badge text-bg-secondary rounded-pill px-2 py-1\">")
                .Append(Html(version))
                .AppendLine("</span>");
            sb.AppendLine("                </div>");
            sb.Append("                <h1 class=\"display-6 fw-bold mb-3\">")
                .Append(Html(groupName))
                .AppendLine("</h1>");
            sb.AppendLine("                <section id=\"namespaces\" class=\"card border-0 shadow-sm\">");
            sb.AppendLine("                    <div class=\"card-body p-2\">");
            sb.AppendLine("                        <h2>Namespaces</h2>");

            if (namespaceItems.Count == 0)
            {
                sb.AppendLine("                        <p class=\"text-body-secondary mb-0\">No namespaces available for this documentation version.</p>");
            }
            else
            {
                sb.AppendLine("                        <ul class=\"list-group list-group-flush\">");

                foreach (DocumentationSidebarDatabaseItem item in namespaceItems)
                {
                    sb.AppendLine("                            <li class=\"list-group-item px-0\">");
                    sb.Append("                                <a href=\"/Documentation/ShowNamespace?groupName=")
                        .Append(System.Net.WebUtility.UrlEncode(groupName))
                        .Append("&packageId=")
                        .Append(System.Net.WebUtility.UrlEncode(item.RoutePackageId))
                        .Append("&version=")
                        .Append(System.Net.WebUtility.UrlEncode(item.RouteVersion))
                        .Append("&namespaceName=")
                        .Append(System.Net.WebUtility.UrlEncode(item.RouteNamespaceName))
                        .AppendLine("\">");
                    sb.Append("                                    <code>")
                        .Append(Html(item.RouteNamespaceName))
                        .AppendLine("</code>");
                    sb.AppendLine("                                </a>");
                    sb.AppendLine("                            </li>");
                }

                sb.AppendLine("                        </ul>");
            }

            sb.AppendLine("                    </div>");
            sb.AppendLine("                </section>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");

            return sb.ToString();
        }

        private string? ResolveLatestDocumentationVersion(
            string? packageId,
            string? version,
            string objectName,
            string? namespaceName = null,
            string? objectType = null
        )
        {
            if (!string.IsNullOrWhiteSpace(version) || string.IsNullOrWhiteSpace(packageId))
            {
                return version;
            }

            using var connection = new SqliteConnection($"Data Source={GetSqliteDatabasePath()}");
            connection.Open();

            string query = """
                           SELECT DISTINCT Version
                           FROM DocumentationObjects
                           WHERE ObjectName = @ObjectName
                             AND PackageId = @PackageId
                             AND Version IS NOT NULL
                             AND Version <> ''
                           """;

            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                query += " AND NamespaceName = @NamespaceName";
            }

            if (!string.IsNullOrWhiteSpace(objectType))
            {
                query += " AND ObjectType = @ObjectType";
            }

            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@ObjectName", objectName);
            command.Parameters.AddWithValue("@PackageId", packageId);

            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                command.Parameters.AddWithValue("@NamespaceName", namespaceName);
            }

            if (!string.IsNullOrWhiteSpace(objectType))
            {
                command.Parameters.AddWithValue("@ObjectType", objectType);
            }

            using var reader = command.ExecuteReader();
            List<string> versions = [];

            while (reader.Read())
            {
                string candidateVersion = reader["Version"]?.ToString() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(candidateVersion))
                {
                    versions.Add(candidateVersion);
                }
            }

            return versions
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(candidateVersion => candidateVersion, DocumentationVersionComparer.Instance)
                .FirstOrDefault();
        }

        private (string? PackageId, string? Version) ResolveLatestDocumentationReference(
            string? packageId,
            string? version,
            string objectName,
            string? namespaceName = null,
            string? objectType = null
        )
        {
            if (!string.IsNullOrWhiteSpace(packageId))
            {
                return (packageId, ResolveLatestDocumentationVersion(packageId, version, objectName, namespaceName, objectType));
            }

            using var connection = new SqliteConnection($"Data Source={GetSqliteDatabasePath()}");
            connection.Open();

            string query = """
                           SELECT DISTINCT PackageId, Version
                           FROM DocumentationObjects
                           WHERE ObjectName = @ObjectName
                             AND PackageId IS NOT NULL
                             AND PackageId <> ''
                             AND Version IS NOT NULL
                             AND Version <> ''
                           """;

            if (!string.IsNullOrWhiteSpace(version))
            {
                query += " AND Version = @Version";
            }

            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                query += " AND NamespaceName = @NamespaceName";
            }

            if (!string.IsNullOrWhiteSpace(objectType))
            {
                query += " AND ObjectType = @ObjectType";
            }

            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@ObjectName", objectName);

            if (!string.IsNullOrWhiteSpace(version))
            {
                command.Parameters.AddWithValue("@Version", version);
            }

            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                command.Parameters.AddWithValue("@NamespaceName", namespaceName);
            }

            if (!string.IsNullOrWhiteSpace(objectType))
            {
                command.Parameters.AddWithValue("@ObjectType", objectType);
            }

            using var reader = command.ExecuteReader();
            List<(string PackageId, string Version)> candidates = [];

            while (reader.Read())
            {
                string candidatePackageId = reader["PackageId"]?.ToString() ?? string.Empty;
                string candidateVersion = reader["Version"]?.ToString() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(candidatePackageId) &&
                    !string.IsNullOrWhiteSpace(candidateVersion))
                {
                    candidates.Add((candidatePackageId, candidateVersion));
                }
            }

            (string PackageId, string Version) selectedCandidate = candidates
                .Distinct()
                .OrderByDescending(candidate => candidate.Version, DocumentationVersionComparer.Instance)
                .ThenByDescending(candidate => IsPreferredDocumentationPackage(candidate.PackageId, objectName, namespaceName))
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(selectedCandidate.PackageId) ||
                string.IsNullOrWhiteSpace(selectedCandidate.Version))
            {
                return (packageId, version);
            }

            return selectedCandidate;
        }

        private static bool IsPreferredDocumentationPackage(string packageId, string objectName, string? namespaceName)
        {
            return string.Equals(packageId, objectName, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(packageId, namespaceName, StringComparison.OrdinalIgnoreCase);
        }

        private sealed class DocumentationVersionComparer : IComparer<string>
        {
            #region Static fields and properties

            /// <summary>
            ///     Gets the shared comparer instance used to order documentation versions.
            /// </summary>
            public static readonly DocumentationVersionComparer Instance = new();

            #endregion

            #region Static methods

            private static int[] ParseVersionParts(string? value)
            {
                int[] parts = new int[4];

                if (string.IsNullOrWhiteSpace(value))
                {
                    return parts;
                }

                string normalized = value.Trim();
                int metadataIndex = normalized.IndexOf('+');

                if (metadataIndex >= 0)
                {
                    normalized = normalized[..metadataIndex];
                }

                int prereleaseIndex = normalized.IndexOf('-');

                if (prereleaseIndex >= 0)
                {
                    normalized = normalized[..prereleaseIndex];
                }

                string[] tokens = normalized.Split('.', StringSplitOptions.RemoveEmptyEntries);

                for (int index = 0; index < tokens.Length && index < parts.Length; index++)
                {
                    if (int.TryParse(tokens[index], out int parsedPart))
                    {
                        parts[index] = parsedPart;
                    }
                }

                return parts;
            }

            #endregion

            #region Instance methods

            #region From interface IComparer<string>

            /// <summary>
            ///     Compares two documentation version strings by normalized numeric version parts.
            /// </summary>
            /// <param name="x">The first version string.</param>
            /// <param name="y">The second version string.</param>
            /// <returns>A value less than, equal to, or greater than zero depending on version ordering.</returns>
            public int Compare(string? x, string? y)
            {
                if (string.Equals(x, y, StringComparison.OrdinalIgnoreCase))
                {
                    return 0;
                }

                int[] xParts = ParseVersionParts(x);
                int[] yParts = ParseVersionParts(y);

                for (int index = 0; index < xParts.Length; index++)
                {
                    int partComparison = xParts[index].CompareTo(yParts[index]);

                    if (partComparison != 0)
                    {
                        return partComparison;
                    }
                }

                return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
            }

            #endregion

            #endregion
        }

        private (
            long Id,
            string? Html,
            string? TechnicalKeywords,
            string? Keywords,
            string? BuilderVersion,
            string PackageId,
            string Version,
            string NamespaceName,
            string ObjectName,
            string ObjectType
            ) GetHtmlContent(
                string objectName,
                string? packageId = null,
                string? version = null,
                string? namespaceName = null,
                string? objectType = null
            )
        {
            using var connection = new SqliteConnection($"Data Source={GetSqliteDatabasePath()}");
            connection.Open();

            string query = """
                           SELECT Id, HtmlContent, TechnicalKeywords, Keywords, Builder, PackageId, Version, NamespaceName, ObjectName, ObjectType
                           FROM DocumentationObjects
                           WHERE ObjectName = @ObjectName
                           """;

            if (objectType == "Group")
            {
                query += " AND ObjectType = @ObjectType";

                if (version != null)
                {
                    query += " AND (Version = @Version OR Version = '')";
                }
            }
            else
            {
                if (packageId != null)
                {
                    query += " AND PackageId = @PackageId";
                }

                if (version != null)
                {
                    query += " AND Version = @Version";
                }

                if (namespaceName != null)
                {
                    query += " AND (NamespaceName = @NamespaceName OR ObjectName = @NamespaceName)";
                }

                if (objectType != null)
                {
                    query += " AND ObjectType = @ObjectType";
                }
            }

            using var command = new SqliteCommand(query, connection);

            command.Parameters.AddWithValue("@ObjectName", objectName);

            if (objectType != null)
            {
                command.Parameters.AddWithValue("@ObjectType", objectType);
            }

            if (objectType != "Group")
            {
                if (packageId != null)
                {
                    command.Parameters.AddWithValue("@PackageId", packageId);
                }

                if (version != null)
                {
                    command.Parameters.AddWithValue("@Version", version);
                }

                if (namespaceName != null)
                {
                    command.Parameters.AddWithValue("@NamespaceName", namespaceName);
                }
            }
            else if (version != null)
            {
                command.Parameters.AddWithValue("@Version", version);
            }

            using var reader = command.ExecuteReader();
            List<(
                long Id,
                string? Html,
                string? TechnicalKeywords,
                string? Keywords,
                string? BuilderVersion,
                string PackageId,
                string Version,
                string NamespaceName,
                string ObjectName,
                string ObjectType
                )> candidates = [];

            while (reader.Read())
            {
                candidates.Add((
                    reader.GetInt64(0),
                    reader["HtmlContent"]?.ToString(),
                    reader["TechnicalKeywords"]?.ToString(),
                    reader["Keywords"]?.ToString(),
                    reader["Builder"]?.ToString(),
                    reader["PackageId"]?.ToString() ?? string.Empty,
                    reader["Version"]?.ToString() ?? string.Empty,
                    reader["NamespaceName"]?.ToString() ?? string.Empty,
                    reader["ObjectName"]?.ToString() ?? string.Empty,
                    reader["ObjectType"]?.ToString() ?? string.Empty
                ));
            }

            var selectedCandidate = candidates
                .OrderByDescending(candidate => candidate.Version, DocumentationVersionComparer.Instance)
                .ThenByDescending(candidate => IsPreferredDocumentationPackage(candidate.PackageId, objectName, namespaceName))
                .FirstOrDefault();

            if (candidates.Count > 0)
            {
                return
                (
                    selectedCandidate.Id,
                    selectedCandidate.Html,
                    selectedCandidate.TechnicalKeywords,
                    selectedCandidate.Keywords,
                    selectedCandidate.BuilderVersion,
                    selectedCandidate.PackageId,
                    selectedCandidate.Version,
                    selectedCandidate.NamespaceName,
                    selectedCandidate.ObjectName,
                    selectedCandidate.ObjectType
                );
            }

            return (0, null, null, null, null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        private IReadOnlyList<DocumentationVersionNavigationItem> GetVersionNavigationItems(
            string groupName,
            string packageId,
            string currentVersion,
            string namespaceName,
            string objectName,
            string objectType
        )
        {
            if (string.IsNullOrWhiteSpace(packageId) ||
                string.IsNullOrWhiteSpace(currentVersion) ||
                string.IsNullOrWhiteSpace(objectName) ||
                string.Equals(objectType, "Group", StringComparison.OrdinalIgnoreCase))
            {
                return [];
            }

            using var connection = new SqliteConnection($"Data Source={GetSqliteDatabasePath()}");
            connection.Open();

            string query = """
                           SELECT DISTINCT Version
                           FROM DocumentationObjects
                           WHERE PackageId = @PackageId
                             AND ObjectName = @ObjectName
                             AND ObjectType = @ObjectType
                             AND Version IS NOT NULL
                             AND Version <> ''
                           """;

            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                query += " AND NamespaceName = @NamespaceName";
            }

            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@PackageId", packageId);
            command.Parameters.AddWithValue("@ObjectName", objectName);
            command.Parameters.AddWithValue("@ObjectType", objectType);

            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                command.Parameters.AddWithValue("@NamespaceName", namespaceName);
            }

            using var reader = command.ExecuteReader();
            List<string> versions = [];

            while (reader.Read())
            {
                string version = reader["Version"]?.ToString() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(version))
                {
                    versions.Add(version);
                }
            }

            return versions
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(version => version, DocumentationVersionComparer.Instance)
                .Select(version => new DocumentationVersionNavigationItem
                {
                    Version = version,
                    IsCurrent = string.Equals(version, currentVersion, StringComparison.OrdinalIgnoreCase),
                    Url = BuildVersionNavigationUrl(groupName, packageId, version, namespaceName, objectName, objectType)
                })
                .Where(item => !string.IsNullOrWhiteSpace(item.Url))
                .ToArray();
        }

        private string BuildVersionNavigationUrl(
            string groupName,
            string packageId,
            string version,
            string namespaceName,
            string objectName,
            string objectType
        )
        {
            if (string.Equals(objectType, "Namespace", StringComparison.OrdinalIgnoreCase))
            {
                return Url.Action(
                    nameof(ShowNamespace),
                    "Documentation",
                    new
                    {
                        groupName,
                        packageId,
                        version,
                        namespaceName = objectName
                    }) ?? string.Empty;
            }

            if (IsMarkdownContentObjectType(objectType))
            {
                return Url.Action(
                    nameof(ShowContent),
                    "DocumentationContent",
                    new
                    {
                        groupName,
                        packageId,
                        version,
                        namespaceName,
                        objectName
                    }) ?? string.Empty;
            }

            return Url.Action(
                nameof(Show),
                "Documentation",
                new
                {
                    groupName,
                    packageId,
                    version,
                    namespaceName,
                    objectName
                }) ?? string.Empty;
        }

        private static bool IsMarkdownContentObjectType(string objectType)
        {
            return string.Equals(objectType, "Tutorial", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(objectType, "ReleaseNote", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(objectType, "MarkdownContent", StringComparison.OrdinalIgnoreCase);
        }

        private List<DocumentationAIRenderSource> GetAIRenderSources()
        {
            using var connection = new SqliteConnection($"Data Source={GetSqliteDatabasePath()}");
            connection.Open();

            if (!TableExists(connection, "DocumentationAIRenderSources"))
            {
                return [];
            }

            const string sql = """
                               SELECT Provider, Model, DatabasePath, IsEnabled
                               FROM DocumentationAIRenderSources
                               WHERE IsEnabled = 1
                               ORDER BY Provider, Model
                               """;

            using var command = new SqliteCommand(sql, connection);
            using var reader = command.ExecuteReader();

            List<DocumentationAIRenderSource> results = [];

            while (reader.Read())
            {
                results.Add(new DocumentationAIRenderSource
                {
                    Provider = reader["Provider"]?.ToString() ?? string.Empty,
                    Model = reader["Model"]?.ToString() ?? string.Empty,
                    DatabasePath = reader["DatabasePath"]?.ToString() ?? string.Empty,
                    IsEnabled = Convert.ToInt32(reader["IsEnabled"]) == 1
                });
            }

            return results;
        }

        private static bool TableExists(SqliteConnection connection, string tableName)
        {
            const string sql = """
                               SELECT 1
                               FROM sqlite_master
                               WHERE type = 'table' AND name = @TableName
                               LIMIT 1
                               """;

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@TableName", tableName);

            object? result = command.ExecuteScalar();
            return result != null && result != DBNull.Value;
        }

        private static bool ColumnExists(SqliteConnection connection, string tableName, string columnName)
        {
            using var command = new SqliteCommand($"PRAGMA table_info({tableName});", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                if (string.Equals(reader["name"]?.ToString(), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static DocumentationAIResultViewModel? GetAIResultFromDatabase(
            string databasePath,
            string provider,
            string model,
            long documentationObjectId,
            string packageId,
            string version,
            string namespaceName,
            string objectName,
            string objectType
        )
        {
            if (!System.IO.File.Exists(databasePath))
            {
                return null;
            }

            using var connection = new SqliteConnection($"Data Source={databasePath}");
            connection.Open();

            if (!TableExists(connection, "DocumentationAIResult"))
            {
                return null;
            }

            bool hasObjectIdentityColumns =
                ColumnExists(connection, "DocumentationAIResult", "PackageId") &&
                ColumnExists(connection, "DocumentationAIResult", "Version") &&
                ColumnExists(connection, "DocumentationAIResult", "NamespaceName") &&
                ColumnExists(connection, "DocumentationAIResult", "ObjectName") &&
                ColumnExists(connection, "DocumentationAIResult", "ObjectType");

            string sql = """
                         SELECT AISummary, AISummaryShort, AIKeywords, AIModel
                         FROM DocumentationAIResult
                         WHERE DocumentationObjectId = @Id
                         """;

            if (hasObjectIdentityColumns)
            {
                sql += """

                       AND PackageId = @PackageId
                       AND Version = @Version
                       AND NamespaceName = @NamespaceName
                       AND ObjectName = @ObjectName
                       AND ObjectType = @ObjectType
                       """;
            }

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", documentationObjectId);

            if (hasObjectIdentityColumns)
            {
                command.Parameters.AddWithValue("@PackageId", packageId);
                command.Parameters.AddWithValue("@Version", version);
                command.Parameters.AddWithValue("@NamespaceName", namespaceName);
                command.Parameters.AddWithValue("@ObjectName", objectName);
                command.Parameters.AddWithValue("@ObjectType", objectType);
            }

            using var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            DocumentationAIResultViewModel result = new()
            {
                Provider = provider,
                Model = model,
                Summary = reader["AISummary"]?.ToString() ?? string.Empty,
                ShortSummary = reader["AISummaryShort"]?.ToString() ?? string.Empty,
                Keywords = reader["AIKeywords"]?.ToString() ?? string.Empty,
                AIModel = reader["AIModel"]?.ToString() ?? string.Empty
            };

            return result.HasContent ? result : null;
        }

        private DocumentationRenderPageViewModel BuildRenderPageModel(
            long documentationObjectId,
            string html,
            string builderVersion,
            string groupName,
            string packageId,
            string version,
            string namespaceName,
            string objectName,
            string objectType
        )
        {
            List<DocumentationAIResultViewModel> aiResults = [];

            foreach (DocumentationAIRenderSource source in GetAIRenderSources())
            {
                string absolutePath = ResolveAIRenderDatabasePath(source.DatabasePath);

                DocumentationAIResultViewModel? aiResult = GetAIResultFromDatabase(
                    absolutePath,
                    source.Provider,
                    source.Model,
                    documentationObjectId,
                    packageId,
                    version,
                    namespaceName,
                    objectName,
                    objectType);

                if (aiResult != null)
                {
                    aiResults.Add(aiResult);
                }
            }

            return new DocumentationRenderPageViewModel
            {
                HtmlContent = html,
                BuilderVersion = builderVersion,
                AIResults = aiResults,
                PackageId = packageId,
                Version = version,
                ObjectName = objectName,
                ObjectType = objectType,
                Versions = GetVersionNavigationItems(
                    groupName,
                    packageId,
                    version,
                    namespaceName,
                    objectName,
                    objectType)
            };
        }

        #endregion

        #region Actions

        /// <summary>
        ///     Renders the documentation root page.
        /// </summary>
        /// <returns>The documentation index view.</returns>
        public IActionResult Index()
        {
            AddBreadcrumb(
                new UrlActionItem().WithUrl("/").SetTitle("Home").SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_house)),
                new AspRouteActionItem("Documentation", "Index").SetTitle("Documentation").SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_book))
            );

            SetSidebar(new SideBarComponent().AddSection(CreateDatabaseRootSidebar()));

            SetDescription("Documentation rendered by DMBDocumentationViewer");
            SetTitle("Documentation");
            SetKeywords("documentation");

            return View("~/Views/Documentation/Index.cshtml");
        }

        /// <summary>
        ///     Renders a generated documentation object page.
        /// </summary>
        /// <param name="groupName">Documentation group name from the route.</param>
        /// <param name="namespaceName">Namespace name that contains the documented object.</param>
        /// <param name="objectName">Documented object name.</param>
        /// <param name="packageId">Optional package identifier filter.</param>
        /// <param name="version">Optional package version filter. When omitted, the latest matching version is selected.</param>
        /// <returns>A rendered documentation page, a redirect for incomplete route values, or a not-found result.</returns>
        public IActionResult Show(string groupName, string namespaceName, string objectName, string? packageId = null, string? version = null)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(namespaceName))
            {
                return RedirectToAction(nameof(ShowGroup), new { groupName, version });
            }

            if (string.IsNullOrWhiteSpace(objectName))
            {
                return RedirectToAction(nameof(ShowNamespace), new { groupName, namespaceName, packageId, version });
            }

            (packageId, version) = ResolveLatestDocumentationReference(packageId, version, objectName, namespaceName);

            SetSidebar(new SideBarComponent().AddSection(
                CreateDatabaseNamespaceSidebar(groupName, namespaceName, packageId, version)));

            AddBreadcrumb(
                new UrlActionItem().WithUrl("/").SetTitle("Home").SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_house)),
                new AspRouteActionItem("Documentation", "Index").SetTitle("Documentation").SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_book)),
                new AspRouteActionItem("Documentation", "ShowGroup").AddRouteValue(nameof(groupName), groupName).AddRouteValue(nameof(version), version ?? string.Empty).SetTitle(groupName).SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_collection)),
                new AspRouteActionItem("Documentation", "ShowNamespace").AddRouteValue(nameof(groupName), groupName).AddRouteValue(nameof(packageId), packageId ?? string.Empty).AddRouteValue(nameof(version), version ?? string.Empty).AddRouteValue(nameof(namespaceName), namespaceName).SetTitle(namespaceName).SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_book_fill)),
                new AspRouteActionItem("Documentation", "Show").AddRouteValue(nameof(groupName), groupName).AddRouteValue(nameof(packageId), packageId ?? string.Empty).AddRouteValue(nameof(version), version ?? string.Empty).AddRouteValue(nameof(namespaceName), namespaceName).AddRouteValue(nameof(objectName), objectName).SetTitle(objectName).SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_box))
            );

            var (
                    id,
                    html,
                    technicalKeywords,
                    keywords,
                    builderVersion,
                    selectedPackageId,
                    selectedVersion,
                    selectedNamespaceName,
                    selectedObjectName,
                    selectedObjectType) =
                GetHtmlContent(objectName, packageId, version, namespaceName);

            if (html == null)
            {
                return NotFound();
            }

            SetDescription(groupName);
            SetTitle($"{namespaceName} {objectName}");
            SetKeywords(technicalKeywords ?? string.Empty);

            DocumentationRenderPageViewModel model = BuildRenderPageModel(
                id,
                html,
                builderVersion ?? string.Empty,
                groupName,
                selectedPackageId,
                selectedVersion,
                selectedNamespaceName,
                selectedObjectName,
                selectedObjectType);

            return View("~/Views/Documentation/RenderFromDatabase.cshtml", model);
        }

        /// <summary>
        ///     Renders a generated documentation group page.
        /// </summary>
        /// <param name="groupName">Documentation group name from the route.</param>
        /// <param name="version">Optional package version filter. When omitted, the latest group sidebar version is selected.</param>
        /// <returns>A rendered group page, a redirect to the index for missing input, or a not-found result.</returns>
        public IActionResult ShowGroup(string groupName, string? version = null)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return RedirectToAction(nameof(Index));
            }

            string resolvedVersion = ResolveLatestGroupSidebarVersion(groupName, version) ?? string.Empty;

            SetSidebar(new SideBarComponent().AddSection(
                CreateDatabaseGroupSidebar(groupName, resolvedVersion)));

            AddBreadcrumb(
                new UrlActionItem().WithUrl("/").SetTitle("Home").SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_house)),
                new AspRouteActionItem("Documentation", "Index").SetTitle("Documentation").SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_book)),
                new AspRouteActionItem("Documentation", "ShowGroup").AddRouteValue(nameof(groupName), groupName).AddRouteValue(nameof(version), resolvedVersion).SetTitle(groupName).SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_collection))
            );

            var (
                    id,
                    html,
                    technicalKeywords,
                    keywords,
                    builderVersion,
                    selectedPackageId,
                    selectedVersion,
                    selectedNamespaceName,
                    selectedObjectName,
                    selectedObjectType) =
                GetHtmlContent(groupName, version: resolvedVersion, objectType: "Group");

            if (html == null)
            {
                List<DocumentationSidebarDatabaseItem> sidebarItems = GetDatabaseSidebarItems(
                    "Group",
                    groupName,
                    string.Empty,
                    resolvedVersion,
                    string.Empty);

                if (sidebarItems.Count == 0)
                {
                    return NotFound();
                }

                html = BuildDocumentationGroupFallbackHtml(groupName, resolvedVersion, sidebarItems);
                selectedPackageId = string.Empty;
                selectedVersion = resolvedVersion;
                selectedNamespaceName = string.Empty;
                selectedObjectName = groupName;
                selectedObjectType = "Group";
            }

            string selectedGroupVersion = string.IsNullOrWhiteSpace(selectedVersion)
                ? resolvedVersion
                : selectedVersion;

            SetDescription(groupName);
            SetTitle(groupName);
            SetKeywords(technicalKeywords ?? string.Empty);

            DocumentationRenderPageViewModel model = BuildRenderPageModel(
                id,
                NormalizeDocumentationVersionLinks(html, selectedGroupVersion),
                builderVersion ?? string.Empty,
                groupName,
                selectedPackageId,
                selectedGroupVersion,
                selectedNamespaceName,
                selectedObjectName,
                selectedObjectType);

            return View("~/Views/Documentation/RenderFromDatabase.cshtml", model);
        }

        /// <summary>
        ///     Renders the generic MCP connection and tool contract page for the selected documentation scope.
        /// </summary>
        /// <param name="groupName">Documentation group name from the route.</param>
        /// <param name="packageId">Optional package identifier for contextual examples.</param>
        /// <param name="version">Optional package version for contextual examples.</param>
        /// <param name="namespaceName">Optional namespace name for contextual examples and sidebar preservation.</param>
        /// <returns>A rendered MCP help page, or a redirect to the index when no group is selected.</returns>
        public IActionResult ShowMcp(string groupName, string? packageId = null, string? version = null, string? namespaceName = null)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return RedirectToAction(nameof(Index));
            }

            string resolvedVersion = string.IsNullOrWhiteSpace(version)
                ? ResolveLatestGroupSidebarVersion(groupName, version) ?? string.Empty
                : version;

            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                SetSidebar(new SideBarComponent().AddSection(
                    CreateDatabaseNamespaceSidebar(groupName, namespaceName, packageId, resolvedVersion)));
            }
            else if (!string.IsNullOrWhiteSpace(packageId))
            {
                SetSidebar(new SideBarComponent().AddSection(
                    CreateDatabaseProjectSidebar(groupName, packageId, resolvedVersion)));
            }
            else
            {
                SetSidebar(new SideBarComponent().AddSection(
                    CreateDatabaseGroupSidebar(groupName, resolvedVersion)));
            }

            string endpointPath = DMBDocumentationViewerConfiguration.Config.McpEndpoint;
            if (!endpointPath.StartsWith("/", StringComparison.Ordinal))
            {
                endpointPath = "/" + endpointPath;
            }

            string endpointUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}{endpointPath}";

            AddBreadcrumb(
                new UrlActionItem().WithUrl("/").SetTitle("Home").SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_house)),
                new AspRouteActionItem("Documentation", "Index").SetTitle("Documentation").SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_book)),
                new AspRouteActionItem("Documentation", "ShowGroup").AddRouteValue(nameof(groupName), groupName).AddRouteValue(nameof(version), resolvedVersion).SetTitle(groupName).SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_collection)),
                new AspRouteActionItem("Documentation", "ShowMcp").AddRouteValue(nameof(groupName), groupName).AddRouteValue(nameof(packageId), packageId ?? string.Empty).AddRouteValue(nameof(version), resolvedVersion).AddRouteValue(nameof(namespaceName), namespaceName ?? string.Empty).SetTitle("MCP").SetIcon(IconStruct.Bootstrap("bi-plug"))
            );

            SetDescription("Connect an AI assistant to DocumentationViewer through MCP.");
            SetTitle("MCP");
            SetKeywords("MCP, Model Context Protocol, AI assistant, documentation, source code");

            DocumentationMcpPageViewModel model = DocumentationMcpPageBuilder.BuildViewModel(
                endpointUrl,
                groupName,
                packageId,
                resolvedVersion,
                namespaceName);

            return View("~/Views/Documentation/Mcp.cshtml", model);
        }

        /// <summary>
        ///     Renders an imported OpenAPI document overview or operation detail page.
        /// </summary>
        /// <param name="groupName">Documentation group name from the route.</param>
        /// <param name="namespaceName">OpenAPI document name stored in the namespace route slot.</param>
        /// <param name="objectName">Optional OpenAPI operation identifier stored in the object route slot.</param>
        /// <param name="packageId">Package identifier that owns the OpenAPI document.</param>
        /// <param name="version">Package version that owns the OpenAPI document.</param>
        /// <param name="sidebarNamespaceName">Optional namespace sidebar context to preserve while browsing REST API pages.</param>
        /// <returns>A rendered OpenAPI page, a redirect for incomplete route values, or a not-found result.</returns>
        public IActionResult ShowOpenApi(
            string groupName,
            string namespaceName,
            string? objectName = null,
            string? packageId = null,
            string? version = null,
            string? sidebarNamespaceName = null
        )
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(namespaceName) ||
                string.IsNullOrWhiteSpace(packageId) ||
                string.IsNullOrWhiteSpace(version))
            {
                return RedirectToAction(nameof(ShowGroup), new { groupName, version });
            }

            SetSidebar(new SideBarComponent().AddSection(
                CreateOpenApiSidebar(groupName, packageId, version, sidebarNamespaceName)));

            OpenApiQueryService openApiQueryService = new(GetSqliteDatabasePath());
            DocumentationOpenApiDocumentQueryResult? document = openApiQueryService.GetDocument(packageId, version, namespaceName);

            if (document is null)
            {
                return NotFound();
            }

            IReadOnlyList<DocumentationOpenApiOperationQueryResult> operations =
                openApiQueryService.ListOperations(packageId, version, namespaceName);

            DocumentationOpenApiOperationQueryResult? selectedOperation = string.IsNullOrWhiteSpace(objectName)
                ? null
                : openApiQueryService.GetOperation(packageId, version, namespaceName, objectName);

            if (!string.IsNullOrWhiteSpace(objectName) && selectedOperation is null)
            {
                return NotFound();
            }

            AddBreadcrumb(
                new UrlActionItem().WithUrl("/").SetTitle("Home").SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_house)),
                new AspRouteActionItem("Documentation", "Index").SetTitle("Documentation").SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_book)),
                new AspRouteActionItem("Documentation", "ShowGroup").AddRouteValue(nameof(groupName), groupName).AddRouteValue(nameof(version), version ?? string.Empty).SetTitle(groupName).SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_collection)),
                new AspRouteActionItem("Documentation", "ShowOpenApi").AddRouteValue(nameof(groupName), groupName).AddRouteValue(nameof(packageId), packageId ?? string.Empty).AddRouteValue(nameof(version), version ?? string.Empty).AddRouteValue(nameof(namespaceName), namespaceName).AddRouteValue(nameof(sidebarNamespaceName), sidebarNamespaceName ?? string.Empty).SetTitle("REST API").SetIcon(IconStruct.Bootstrap("bi-hdd-network"))
            );

            SetDescription(document.Description);
            SetTitle(selectedOperation is null ? document.Title : $"{selectedOperation.HttpMethod} {selectedOperation.Path}");
            SetKeywords($"OpenAPI REST API {document.Title} {document.DocumentName}");

            DocumentationOpenApiPageViewModel model = new()
            {
                GroupName = groupName,
                Document = document,
                Operations = operations,
                SidebarNamespaceName = sidebarNamespaceName ?? string.Empty,
                SelectedOperationDisplay = OpenApiOperationDisplayBuilder.Build(document.JsonContent, selectedOperation),
                SelectedOperation = selectedOperation
            };

            return View("~/Views/Documentation/OpenApi.cshtml", model);
        }

        /// <summary>
        ///     Renders a generated documentation namespace page.
        /// </summary>
        /// <param name="groupName">Documentation group name from the route.</param>
        /// <param name="namespaceName">Namespace name to render.</param>
        /// <param name="packageId">Optional package identifier filter.</param>
        /// <param name="version">Optional package version filter. When omitted, the latest matching version is selected.</param>
        /// <returns>A rendered namespace page, a redirect for incomplete route values, or a not-found result.</returns>
        public IActionResult ShowNamespace(string groupName, string namespaceName, string? packageId = null, string? version = null)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(namespaceName))
            {
                return RedirectToAction(nameof(ShowGroup), new { groupName, version });
            }

            (packageId, version) = ResolveLatestDocumentationReference(packageId, version, namespaceName, namespaceName, "Namespace");

            SetSidebar(new SideBarComponent().AddSection(
                CreateDatabaseNamespaceSidebar(groupName, namespaceName, packageId, version)));

            AddBreadcrumb(
                new UrlActionItem().WithUrl("/").SetTitle("Home").SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_house)),
                new AspRouteActionItem("Documentation", "Index").SetTitle("Documentation").SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_book)),
                new AspRouteActionItem("Documentation", "ShowGroup").AddRouteValue(nameof(groupName), groupName).AddRouteValue(nameof(version), version ?? string.Empty).SetTitle(groupName).SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_collection)),
                new AspRouteActionItem("Documentation", "ShowNamespace").AddRouteValue(nameof(groupName), groupName).AddRouteValue(nameof(packageId), packageId ?? string.Empty).AddRouteValue(nameof(version), version ?? string.Empty).AddRouteValue(nameof(namespaceName), namespaceName).SetTitle(namespaceName).SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_book_fill))
            );

            var (
                    id,
                    html,
                    technicalKeywords,
                    keywords,
                    builderVersion,
                    selectedPackageId,
                    selectedVersion,
                    selectedNamespaceName,
                    selectedObjectName,
                    selectedObjectType) =
                GetHtmlContent(namespaceName, packageId, version, objectType: "Namespace");

            if (html == null)
            {
                return NotFound();
            }

            SetDescription(namespaceName);
            SetTitle(namespaceName);
            SetKeywords(technicalKeywords ?? string.Empty);

            DocumentationRenderPageViewModel model = BuildRenderPageModel(
                id,
                html,
                builderVersion ?? string.Empty,
                groupName,
                selectedPackageId,
                selectedVersion,
                selectedNamespaceName,
                selectedObjectName,
                selectedObjectType);

            return View("~/Views/Documentation/RenderFromDatabase.cshtml", model);
        }

        /// <summary>
        ///     Renders a generated Markdown content page.
        /// </summary>
        /// <param name="groupName">Documentation group name from the route.</param>
        /// <param name="namespaceName">Markdown content section name.</param>
        /// <param name="objectName">Markdown content slug.</param>
        /// <param name="packageId">Optional package identifier filter.</param>
        /// <param name="version">Optional package version filter. When omitted, the latest matching version is selected.</param>
        /// <param name="sidebarNamespaceName">
        ///     Optional namespace sidebar scope to preserve when a Markdown page is opened from a
        ///     namespace page.
        /// </param>
        /// <returns>A rendered Markdown content page, a redirect for incomplete route values, or a not-found result.</returns>
        public IActionResult ShowContent(
            string groupName,
            string namespaceName,
            string objectName,
            string? packageId = null,
            string? version = null,
            string? sidebarNamespaceName = null
        )
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(namespaceName) || string.IsNullOrWhiteSpace(objectName))
            {
                return RedirectToAction(nameof(ShowGroup), new { groupName, version });
            }

            (packageId, version) = ResolveLatestDocumentationReference(packageId, version, objectName, namespaceName);

            var (
                    id,
                    html,
                    technicalKeywords,
                    keywords,
                    builderVersion,
                    selectedPackageId,
                    selectedVersion,
                    selectedNamespaceName,
                    selectedObjectName,
                    selectedObjectType) =
                GetHtmlContent(objectName, packageId, version, namespaceName);

            if (html == null || !IsMarkdownContentObjectType(selectedObjectType))
            {
                return NotFound();
            }

            string selectedSidebarNamespaceName = string.IsNullOrWhiteSpace(sidebarNamespaceName)
                ? ResolveDefaultSidebarNamespaceName(groupName, selectedPackageId, selectedVersion)
                : sidebarNamespaceName;

            SetSidebar(new SideBarComponent().AddSection(
                CreateMarkdownContentSidebar(groupName, selectedPackageId, selectedVersion, selectedSidebarNamespaceName)));

            AddBreadcrumb(
                new UrlActionItem().WithUrl("/").SetTitle("Home").SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_house)),
                new AspRouteActionItem("Documentation", "Index").SetTitle("Documentation").SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_book)),
                new AspRouteActionItem("Documentation", "ShowGroup").AddRouteValue(nameof(groupName), groupName).AddRouteValue(nameof(version), selectedVersion).SetTitle(groupName).SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_collection)),
                new AspRouteActionItem("DocumentationContent", "ShowContent").AddRouteValue(nameof(groupName), groupName).AddRouteValue(nameof(packageId), selectedPackageId).AddRouteValue(nameof(version), selectedVersion).AddRouteValue(nameof(namespaceName), selectedNamespaceName).AddRouteValue(nameof(objectName), selectedObjectName).AddRouteValue(nameof(sidebarNamespaceName), selectedSidebarNamespaceName).SetTitle(selectedNamespaceName).SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_journal_text))
            );

            SetDescription(MergeKeywords(technicalKeywords, keywords));
            SetTitle($"{namespaceName} {objectName}");
            SetKeywords(technicalKeywords ?? string.Empty);

            DocumentationRenderPageViewModel model = BuildRenderPageModel(
                id,
                html,
                builderVersion ?? string.Empty,
                groupName,
                selectedPackageId,
                selectedVersion,
                selectedNamespaceName,
                selectedObjectName,
                selectedObjectType);

            return View("~/Views/Documentation/RenderFromDatabase.cshtml", model);
        }

        #endregion
    }
}