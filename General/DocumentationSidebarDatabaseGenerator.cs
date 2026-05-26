#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationSidebarDatabaseGenerator.cs create at 2026/05/18 22:05:00
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using System.Text.Json;

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Persists versioned sidebar data used by DocumentationViewer.
    /// </summary>
    public static class DocumentationSidebarDatabaseGenerator
    {
        #region Static methods

        /// <summary>
        /// Persists sidebar data for the package versions included in the current generation.
        /// </summary>
        /// <param name="index">The documentation index used to build sidebar entries.</param>
        /// <param name="groups">The documentation group descriptors that own optional Markdown sidebar content.</param>
        /// <param name="sqliteDatabasePath">The SQLite database path that receives the sidebar entries.</param>
        public static void GenerateSidebarData(
            DocumentationIndex index,
            IEnumerable<DocumentationGroupDescriptor> groups,
            string sqliteDatabasePath
        )
        {
            if (index is null) throw new ArgumentNullException(nameof(index));
            if (groups is null) throw new ArgumentNullException(nameof(groups));

            DocumentationDatabaseManager.ReplaceGeneratedSidebarItems(
                sqliteDatabasePath,
                BuildSidebarItems(index, groups));
        }

        private static void AddGroupSidebarItems(
            DocumentationGroupItem group,
            IReadOnlyDictionary<string, DocumentationProjectDescriptor> projectsByKey,
            List<DocumentationSidebarItem> items
        )
        {
            List<IGrouping<string, DocumentationProjectItem>> versionGroups = group.Projects
                .GroupBy(project => project.Version, StringComparer.Ordinal)
                .OrderByDescending(projectGroup => projectGroup.Key, DocumentationVersionComparer.Instance)
                .ToList();

            foreach (IGrouping<string, DocumentationProjectItem> versionGroup in versionGroups)
            {
                string sidebarVersion = versionGroup.Key;

                items.Add(CreateRouteItem(
                    "Group",
                    group.GroupName,
                    string.Empty,
                    sidebarVersion,
                    string.Empty,
                    "group-home",
                    string.Empty,
                    group.GroupName,
                    "bi-collection",
                    "ShowGroup",
                    group.GroupName,
                    string.Empty,
                    sidebarVersion,
                    string.Empty,
                    string.Empty,
                    0));

                List<DocumentationProjectItem> projects = versionGroup
                    .OrderBy(project => project.ProjectName, StringComparer.Ordinal)
                    .ToList();

                for (int projectIndex = 0; projectIndex < projects.Count; projectIndex++)
                {
                    DocumentationProjectItem project = projects[projectIndex];
                    string projectKey = CreateProjectItemKey(project);

                    items.Add(CreateGroupItem(
                        "Group",
                        group.GroupName,
                        string.Empty,
                        sidebarVersion,
                        string.Empty,
                        projectKey,
                        string.Empty,
                        project.ProjectName,
                        "bi-folder",
                        projectIndex + 1));

                    AddProjectNamespaceRoutes(
                        "Group",
                        group.GroupName,
                        string.Empty,
                        sidebarVersion,
                        string.Empty,
                        projectKey,
                        group.GroupName,
                        project,
                        items);

                    if (projectsByKey.TryGetValue(CreateProjectLookupKey(group.GroupName, project.PackageId, project.Version), out DocumentationProjectDescriptor? descriptor))
                    {
                        AddMarkdownContentGroups(
                            "Group",
                            group.GroupName,
                            string.Empty,
                            sidebarVersion,
                            string.Empty,
                            projectKey,
                            group.GroupName,
                            descriptor,
                            items);

                        AddOpenApiContentGroups(
                            "Group",
                            group.GroupName,
                            string.Empty,
                            sidebarVersion,
                            string.Empty,
                            projectKey,
                            string.Empty,
                            group.GroupName,
                            descriptor,
                            items);
                    }
                }

                AddMcpSidebarItems(
                    "Group",
                    group.GroupName,
                    string.Empty,
                    sidebarVersion,
                    string.Empty,
                    group.GroupName,
                    string.Empty,
                    sidebarVersion,
                    string.Empty,
                    items);
            }
        }

        private static void AddNamespaceSidebarItems(
            string groupName,
            DocumentationProjectItem project,
            DocumentationNamespaceItem ns,
            IReadOnlyDictionary<string, DocumentationProjectDescriptor> projectsByKey,
            List<DocumentationSidebarItem> items
        )
        {
            items.Add(CreateRouteItem(
                "Namespace",
                groupName,
                project.PackageId,
                project.Version,
                ns.NamespaceName,
                "back-to-group",
                string.Empty,
                groupName,
                "bi-arrow-left-circle",
                "ShowGroup",
                groupName,
                string.Empty,
                project.Version,
                string.Empty,
                string.Empty,
                0));

            string projectKey = CreateProjectItemKey(project);

            items.Add(CreateGroupItem(
                "Namespace",
                groupName,
                project.PackageId,
                project.Version,
                ns.NamespaceName,
                projectKey,
                string.Empty,
                project.ProjectName,
                "bi-folder",
                1));

            AddProjectNamespaceRoutes(
                "Namespace",
                groupName,
                project.PackageId,
                project.Version,
                ns.NamespaceName,
                projectKey,
                groupName,
                new DocumentationProjectItem
                {
                    ProjectName = project.ProjectName,
                    PackageId = project.PackageId,
                    Version = project.Version,
                    ProjectFilePath = string.Empty,
                    Namespaces = new List<DocumentationNamespaceItem> { ns }
                },
                items);

            int sortOrder = 2;
            AddObjectGroup(groupName, project, ns, "Classes", "bi-building", ns.Classes, sortOrder++, items);
            AddObjectGroup(groupName, project, ns, "Interfaces", "bi-diagram-3", ns.Interfaces, sortOrder++, items);
            AddObjectGroup(groupName, project, ns, "Records", "bi-card-text", ns.Records, sortOrder++, items);
            AddObjectGroup(groupName, project, ns, "Structs", "bi-box", ns.Structs, sortOrder++, items);
            AddObjectGroup(groupName, project, ns, "Enums", "bi-list-ul", ns.Enums, sortOrder, items);

            if (projectsByKey.TryGetValue(CreateProjectLookupKey(groupName, project.PackageId, project.Version), out DocumentationProjectDescriptor? descriptor))
            {
                AddMarkdownContentGroups(
                    "Namespace",
                    groupName,
                    project.PackageId,
                    project.Version,
                    ns.NamespaceName,
                    projectKey,
                    groupName,
                    descriptor,
                    items);

                AddOpenApiContentGroups(
                    "Namespace",
                    groupName,
                    project.PackageId,
                    project.Version,
                    ns.NamespaceName,
                    projectKey,
                    string.Empty,
                    groupName,
                    descriptor,
                    items);
            }

            AddMcpSidebarItems(
                "Namespace",
                groupName,
                project.PackageId,
                project.Version,
                ns.NamespaceName,
                groupName,
                project.PackageId,
                project.Version,
                ns.NamespaceName,
                items);
        }

        private static void AddProjectSidebarItems(
            string groupName,
            DocumentationProjectItem project,
            IReadOnlyDictionary<string, DocumentationProjectDescriptor> projectsByKey,
            List<DocumentationSidebarItem> items
        )
        {
            items.Add(CreateRouteItem(
                "Project",
                groupName,
                project.PackageId,
                project.Version,
                string.Empty,
                "back-to-group",
                string.Empty,
                groupName,
                "bi-arrow-left-circle",
                "ShowGroup",
                groupName,
                string.Empty,
                project.Version,
                string.Empty,
                string.Empty,
                0));

            string projectKey = CreateProjectItemKey(project);

            items.Add(CreateGroupItem(
                "Project",
                groupName,
                project.PackageId,
                project.Version,
                string.Empty,
                projectKey,
                string.Empty,
                project.ProjectName,
                "bi-folder",
                1));

            AddProjectNamespaceRoutes(
                "Project",
                groupName,
                project.PackageId,
                project.Version,
                string.Empty,
                projectKey,
                groupName,
                project,
                items);

            if (projectsByKey.TryGetValue(CreateProjectLookupKey(groupName, project.PackageId, project.Version), out DocumentationProjectDescriptor? descriptor))
            {
                AddMarkdownContentGroups(
                    "Project",
                    groupName,
                    project.PackageId,
                    project.Version,
                    string.Empty,
                    projectKey,
                    groupName,
                    descriptor,
                    items);

                AddOpenApiContentGroups(
                    "Project",
                    groupName,
                    project.PackageId,
                    project.Version,
                    string.Empty,
                    projectKey,
                    string.Empty,
                    groupName,
                    descriptor,
                    items);
            }

            AddMcpSidebarItems(
                "Project",
                groupName,
                project.PackageId,
                project.Version,
                string.Empty,
                groupName,
                project.PackageId,
                project.Version,
                string.Empty,
                items);
        }

        private static void AddMcpSidebarItems(
            string sidebarKind,
            string sidebarGroupName,
            string sidebarPackageId,
            string sidebarVersion,
            string sidebarNamespaceName,
            string routeGroupName,
            string routePackageId,
            string routeVersion,
            string routeNamespaceName,
            List<DocumentationSidebarItem> items
        )
        {
            const string mcpKey = "mcp";

            items.Add(CreateGroupItem(
                sidebarKind,
                sidebarGroupName,
                sidebarPackageId,
                sidebarVersion,
                sidebarNamespaceName,
                mcpKey,
                string.Empty,
                "MCP",
                "bi-plug",
                900));

            items.Add(CreateRouteItem(
                sidebarKind,
                sidebarGroupName,
                sidebarPackageId,
                sidebarVersion,
                sidebarNamespaceName,
                "mcp:connect-ai-assistant",
                mcpKey,
                "Connect an AI assistant",
                string.Empty,
                "ShowMcp",
                routeGroupName,
                routePackageId,
                routeVersion,
                routeNamespaceName,
                string.Empty,
                0));
        }

        private static void AddOpenApiContentGroups(
            string sidebarKind,
            string sidebarGroupName,
            string sidebarPackageId,
            string sidebarVersion,
            string sidebarNamespaceName,
            string sectionKeyPrefix,
            string sectionParentItemKey,
            string routeGroupName,
            DocumentationProjectDescriptor project,
            List<DocumentationSidebarItem> items
        )
        {
            IReadOnlyList<DocumentationOpenApiDocumentItem> documents = DocumentationOpenApiExtractor.Extract(routeGroupName, project);

            if (documents.Count == 0)
            {
                return;
            }

            string sectionTitle = documents.FirstOrDefault()?.SectionTitle ?? "REST API";
            string sectionIcon = documents.FirstOrDefault()?.Icon ?? "bi-hdd-network";
            string sectionKey = $"{sectionKeyPrefix}:openapi";

            items.Add(CreateGroupItem(
                sidebarKind,
                sidebarGroupName,
                sidebarPackageId,
                sidebarVersion,
                sidebarNamespaceName,
                sectionKey,
                sectionParentItemKey,
                sectionTitle,
                sectionIcon,
                800));

            for (int documentIndex = 0; documentIndex < documents.Count; documentIndex++)
            {
                DocumentationOpenApiDocumentItem document = documents[documentIndex];
                bool useDocumentGroup = documents.Count > 1;
                string documentKey = useDocumentGroup
                    ? $"{sectionKey}:document:{document.DocumentName}"
                    : sectionKey;

                if (useDocumentGroup)
                {
                    items.Add(CreateGroupItem(
                        sidebarKind,
                        sidebarGroupName,
                        sidebarPackageId,
                        sidebarVersion,
                        sidebarNamespaceName,
                        documentKey,
                        sectionKey,
                        document.Title,
                        "bi-diagram-3",
                        documentIndex));
                }

                items.Add(CreateRouteItem(
                    sidebarKind,
                    sidebarGroupName,
                    sidebarPackageId,
                    sidebarVersion,
                    sidebarNamespaceName,
                    $"{documentKey}:overview",
                    documentKey,
                    "Overview",
                    string.Empty,
                    "ShowOpenApi",
                    routeGroupName,
                    project.PackageId,
                    project.Version,
                    document.DocumentName,
                    string.Empty,
                    useDocumentGroup ? 0 : documentIndex));

                AddOpenApiOperationRoutes(
                    sidebarKind,
                    sidebarGroupName,
                    sidebarPackageId,
                    sidebarVersion,
                    sidebarNamespaceName,
                    documentKey,
                    routeGroupName,
                    project,
                    document,
                    items);
            }
        }

        private static void AddOpenApiOperationRoutes(
            string sidebarKind,
            string sidebarGroupName,
            string sidebarPackageId,
            string sidebarVersion,
            string sidebarNamespaceName,
            string parentItemKey,
            string routeGroupName,
            DocumentationProjectDescriptor project,
            DocumentationOpenApiDocumentItem document,
            List<DocumentationSidebarItem> items
        )
        {
            int tagIndex = 1;

            foreach (IGrouping<string, DocumentationOpenApiOperationItem> tagGroup in document.Operations
                         .GroupBy(GetPrimaryOperationTag, StringComparer.Ordinal)
                         .OrderBy(group => group.Key, StringComparer.Ordinal))
            {
                string tagKey = $"{parentItemKey}:tag:{tagGroup.Key}";

                items.Add(CreateGroupItem(
                    sidebarKind,
                    sidebarGroupName,
                    sidebarPackageId,
                    sidebarVersion,
                    sidebarNamespaceName,
                    tagKey,
                    parentItemKey,
                    tagGroup.Key,
                    "bi-tag",
                    tagIndex++));

                int operationIndex = 0;

                foreach (DocumentationOpenApiOperationItem operation in tagGroup
                             .OrderBy(operation => operation.Path, StringComparer.Ordinal)
                             .ThenBy(operation => operation.HttpMethod, StringComparer.Ordinal))
                {
                    items.Add(CreateRouteItem(
                        sidebarKind,
                        sidebarGroupName,
                        sidebarPackageId,
                        sidebarVersion,
                        sidebarNamespaceName,
                        $"{tagKey}:operation:{operation.OperationId}",
                        tagKey,
                        $"{operation.HttpMethod} {operation.Path}",
                        string.Empty,
                        "ShowOpenApi",
                        routeGroupName,
                        project.PackageId,
                        project.Version,
                        document.DocumentName,
                        operation.OperationId,
                        operationIndex++));
                }
            }
        }

        private static string GetPrimaryOperationTag(DocumentationOpenApiOperationItem operation)
        {
            try
            {
                using JsonDocument document = JsonDocument.Parse(operation.TagsJson);

                if (document.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement tag in document.RootElement.EnumerateArray())
                    {
                        if (tag.ValueKind == JsonValueKind.String)
                        {
                            string? value = tag.GetString();

                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                return value;
                            }
                        }
                    }
                }
            }
            catch (JsonException)
            {
                return "Operations";
            }

            return "Operations";
        }

        private static void AddMarkdownContentGroups(
            string sidebarKind,
            string sidebarGroupName,
            string sidebarPackageId,
            string sidebarVersion,
            string sidebarNamespaceName,
            string parentItemKey,
            string routeGroupName,
            DocumentationProjectDescriptor project,
            List<DocumentationSidebarItem> items
        )
        {
            IReadOnlyList<DocumentationMarkdownContentItem> markdownItems = DocumentationMarkdownContentScanner.Scan(project);

            int sectionIndex = 0;

            foreach (IGrouping<string, DocumentationMarkdownContentItem> section in markdownItems
                         .GroupBy(item => item.SectionTitle, StringComparer.Ordinal)
                         .OrderBy(group => group.Key, StringComparer.Ordinal))
            {
                DocumentationMarkdownContentItem first = section.First();
                string sectionKey = $"{parentItemKey}:markdown:{first.ObjectType}:{first.SectionTitle}";

                items.Add(CreateGroupItem(
                    sidebarKind,
                    sidebarGroupName,
                    sidebarPackageId,
                    sidebarVersion,
                    sidebarNamespaceName,
                    sectionKey,
                    parentItemKey,
                    first.SectionTitle,
                    first.Icon,
                    100 + sectionIndex++));

                foreach (IGrouping<string, DocumentationMarkdownContentItem> folder in section
                             .GroupBy(item => item.FolderTitle, StringComparer.Ordinal)
                             .OrderBy(group => group.Key, StringComparer.Ordinal))
                {
                    if (string.IsNullOrWhiteSpace(folder.Key))
                    {
                        AddMarkdownRoutes(
                            sidebarKind,
                            sidebarGroupName,
                            sidebarPackageId,
                            sidebarVersion,
                            sidebarNamespaceName,
                            sectionKey,
                            routeGroupName,
                            project,
                            folder,
                            items);
                    }
                    else
                    {
                        string folderKey = $"{sectionKey}:folder:{folder.Key}";

                        items.Add(CreateGroupItem(
                            sidebarKind,
                            sidebarGroupName,
                            sidebarPackageId,
                            sidebarVersion,
                            sidebarNamespaceName,
                            folderKey,
                            sectionKey,
                            folder.Key,
                            "bi-folder",
                            0));

                        AddMarkdownRoutes(
                            sidebarKind,
                            sidebarGroupName,
                            sidebarPackageId,
                            sidebarVersion,
                            sidebarNamespaceName,
                            folderKey,
                            routeGroupName,
                            project,
                            folder,
                            items);
                    }
                }
            }
        }

        private static void AddMarkdownRoutes(
            string sidebarKind,
            string sidebarGroupName,
            string sidebarPackageId,
            string sidebarVersion,
            string sidebarNamespaceName,
            string parentItemKey,
            string routeGroupName,
            DocumentationProjectDescriptor project,
            IEnumerable<DocumentationMarkdownContentItem> markdownItems,
            List<DocumentationSidebarItem> items
        )
        {
            DocumentationMarkdownContentItem[] orderedItems = markdownItems
                .OrderBy(item => item.Title, StringComparer.Ordinal)
                .ToArray();

            for (int index = 0; index < orderedItems.Length; index++)
            {
                DocumentationMarkdownContentItem item = orderedItems[index];

                items.Add(CreateRouteItem(
                    sidebarKind,
                    sidebarGroupName,
                    sidebarPackageId,
                    sidebarVersion,
                    sidebarNamespaceName,
                    $"{parentItemKey}:page:{item.Slug}",
                    parentItemKey,
                    item.Title,
                    string.Empty,
                    "ShowContent",
                    "DocumentationContent",
                    routeGroupName,
                    project.PackageId,
                    project.Version,
                    item.SectionTitle,
                    item.Slug,
                    index));
            }
        }

        private static void AddObjectGroup(
            string groupName,
            DocumentationProjectItem project,
            DocumentationNamespaceItem ns,
            string title,
            string icon,
            IEnumerable<string> objectNames,
            int groupSortOrder,
            List<DocumentationSidebarItem> items
        )
        {
            string[] names = objectNames
                .OrderBy(x => x, StringComparer.Ordinal)
                .ToArray();

            if (names.Length == 0)
            {
                return;
            }

            string objectGroupKey = $"objects:{title}";

            items.Add(CreateGroupItem(
                "Namespace",
                groupName,
                project.PackageId,
                project.Version,
                ns.NamespaceName,
                objectGroupKey,
                string.Empty,
                title,
                icon,
                groupSortOrder));

            for (int objectIndex = 0; objectIndex < names.Length; objectIndex++)
            {
                string objectName = names[objectIndex];

                items.Add(CreateRouteItem(
                    "Namespace",
                    groupName,
                    project.PackageId,
                    project.Version,
                    ns.NamespaceName,
                    $"{objectGroupKey}:{objectName}",
                    objectGroupKey,
                    objectName,
                    string.Empty,
                    "Show",
                    groupName,
                    project.PackageId,
                    project.Version,
                    ns.NamespaceName,
                    objectName,
                    objectIndex));
            }
        }

        private static void AddProjectNamespaceRoutes(
            string sidebarKind,
            string sidebarGroupName,
            string sidebarPackageId,
            string sidebarVersion,
            string sidebarNamespaceName,
            string parentItemKey,
            string routeGroupName,
            DocumentationProjectItem project,
            List<DocumentationSidebarItem> items
        )
        {
            List<DocumentationNamespaceItem> namespaces = project.Namespaces
                .OrderBy(x => x.NamespaceName, StringComparer.Ordinal)
                .ToList();

            for (int namespaceIndex = 0; namespaceIndex < namespaces.Count; namespaceIndex++)
            {
                DocumentationNamespaceItem ns = namespaces[namespaceIndex];

                items.Add(CreateRouteItem(
                    sidebarKind,
                    sidebarGroupName,
                    sidebarPackageId,
                    sidebarVersion,
                    sidebarNamespaceName,
                    $"{parentItemKey}:namespace:{ns.NamespaceName}",
                    parentItemKey,
                    ns.NamespaceName,
                    string.Empty,
                    "ShowNamespace",
                    routeGroupName,
                    project.PackageId,
                    project.Version,
                    ns.NamespaceName,
                    string.Empty,
                    namespaceIndex));
            }
        }

        private static void AddRootSidebarItems(DocumentationIndex index, List<DocumentationSidebarItem> items)
        {
            const string groupsKey = "groups";

            items.Add(CreateGroupItem("Root", string.Empty, string.Empty, string.Empty, string.Empty, groupsKey, string.Empty, "Groups", "bi-collection", 0));

            List<DocumentationGroupItem> groups = index.Groups
                .GroupBy(x => x.GroupName, StringComparer.Ordinal)
                .Select(x => x.First())
                .OrderBy(x => x.GroupName, StringComparer.Ordinal)
                .ToList();

            for (int indexOfGroup = 0; indexOfGroup < groups.Count; indexOfGroup++)
            {
                DocumentationGroupItem group = groups[indexOfGroup];

                items.Add(CreateRouteItem(
                    "Root",
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    $"group:{group.GroupName}",
                    groupsKey,
                    group.GroupName,
                    string.Empty,
                    "ShowGroup",
                    group.GroupName,
                    string.Empty,
                    ResolveLatestGroupVersion(group),
                    string.Empty,
                    string.Empty,
                    indexOfGroup));
            }
        }

        private static IEnumerable<DocumentationSidebarItem> BuildSidebarItems(
            DocumentationIndex index,
            IEnumerable<DocumentationGroupDescriptor> groups
        )
        {
            List<DocumentationSidebarItem> items = [];
            IReadOnlyDictionary<string, DocumentationProjectDescriptor> projectsByKey = BuildProjectLookup(groups);

            AddRootSidebarItems(index, items);

            foreach (DocumentationGroupItem group in index.Groups
                         .GroupBy(group => group.GroupName, StringComparer.Ordinal)
                         .Select(group => group.First()))
            {
                AddGroupSidebarItems(group, projectsByKey, items);

                foreach (DocumentationProjectItem project in group.Projects)
                {
                    AddProjectSidebarItems(group.GroupName, project, projectsByKey, items);

                    foreach (DocumentationNamespaceItem ns in project.Namespaces)
                    {
                        AddNamespaceSidebarItems(group.GroupName, project, ns, projectsByKey, items);
                    }
                }
            }

            return items;
        }

        private static DocumentationSidebarItem CreateGroupItem(
            string sidebarKind,
            string groupName,
            string packageId,
            string version,
            string namespaceName,
            string itemKey,
            string parentItemKey,
            string title,
            string icon,
            int sortOrder
        )
        {
            return new DocumentationSidebarItem
            {
                SidebarKind = sidebarKind,
                GroupName = groupName,
                PackageId = packageId,
                Version = version,
                NamespaceName = namespaceName,
                ItemKey = itemKey,
                ParentItemKey = parentItemKey,
                ItemKind = "Group",
                Title = title,
                Icon = icon,
                SortOrder = sortOrder
            };
        }

        private static string CreateProjectItemKey(DocumentationProjectItem project)
        {
            return $"project:{project.PackageId}:{project.Version}:{project.ProjectName}";
        }

        private static string ResolveLatestGroupVersion(DocumentationGroupItem group)
        {
            return group.Projects
                .Select(project => project.Version)
                .Where(version => !string.IsNullOrWhiteSpace(version))
                .OrderByDescending(version => version, DocumentationVersionComparer.Instance)
                .FirstOrDefault() ?? string.Empty;
        }

        private static IReadOnlyDictionary<string, DocumentationProjectDescriptor> BuildProjectLookup(IEnumerable<DocumentationGroupDescriptor> groups)
        {
            Dictionary<string, DocumentationProjectDescriptor> result = new(StringComparer.Ordinal);

            foreach (DocumentationGroupDescriptor group in groups)
            {
                foreach (DocumentationProjectDescriptor project in group.Projects)
                {
                    result[CreateProjectLookupKey(group.GroupName, project.PackageId, project.Version)] = project;
                }
            }

            return result;
        }

        private static string CreateProjectLookupKey(string groupName, string packageId, string version)
        {
            return $"{groupName}\u001F{packageId}\u001F{version}";
        }

        private static DocumentationSidebarItem CreateRouteItem(
            string sidebarKind,
            string groupName,
            string packageId,
            string version,
            string namespaceName,
            string itemKey,
            string parentItemKey,
            string title,
            string icon,
            string actionName,
            string routeGroupName,
            string routePackageId,
            string routeVersion,
            string routeNamespaceName,
            string routeObjectName,
            int sortOrder
        )
        {
            return CreateRouteItem(
                sidebarKind,
                groupName,
                packageId,
                version,
                namespaceName,
                itemKey,
                parentItemKey,
                title,
                icon,
                actionName,
                "Documentation",
                routeGroupName,
                routePackageId,
                routeVersion,
                routeNamespaceName,
                routeObjectName,
                sortOrder);
        }

        private static DocumentationSidebarItem CreateRouteItem(
            string sidebarKind,
            string groupName,
            string packageId,
            string version,
            string namespaceName,
            string itemKey,
            string parentItemKey,
            string title,
            string icon,
            string actionName,
            string controllerName,
            string routeGroupName,
            string routePackageId,
            string routeVersion,
            string routeNamespaceName,
            string routeObjectName,
            int sortOrder
        )
        {
            return new DocumentationSidebarItem
            {
                SidebarKind = sidebarKind,
                GroupName = groupName,
                PackageId = packageId,
                Version = version,
                NamespaceName = namespaceName,
                ItemKey = itemKey,
                ParentItemKey = parentItemKey,
                ItemKind = "Route",
                Title = title,
                Icon = icon,
                ControllerName = controllerName,
                ActionName = actionName,
                RouteGroupName = routeGroupName,
                RoutePackageId = routePackageId,
                RouteVersion = routeVersion,
                RouteNamespaceName = routeNamespaceName,
                RouteObjectName = routeObjectName,
                SortOrder = sortOrder
            };
        }

        #endregion
    }
}
