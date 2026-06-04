#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

#endregion

namespace DMBDocumentationImprovementByAI
{
    /// <summary>
    ///     Updates project files for generated documentation database copy behavior.
    /// </summary>
    public static class ProjectFileHelper
    {
        #region Public methods

        /// <summary>
        ///     Ensures generated documentation databases are copied to the output directory by the project file.
        /// </summary>
        public static void EnsureDocumentationDatabasesCopyAlways(string csprojPath, bool enable)
        {
            if (!enable)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(csprojPath))
            {
                throw new ArgumentException("csprojPath is required.", nameof(csprojPath));
            }

            if (!File.Exists(csprojPath))
            {
                throw new FileNotFoundException("Project file not found.", csprojPath);
            }

            XDocument document = XDocument.Load(csprojPath, LoadOptions.PreserveWhitespace);
            XElement projectElement = document.Root
                                      ?? throw new InvalidOperationException("Invalid .csproj: missing root element.");

            XNamespace ns = projectElement.Name.Namespace;

            EnsureNoneUpdate(projectElement, ns, @"Documentation\data.db");
            EnsureNoneUpdate(projectElement, ns, @"Documentation\*.db");
            EnsureNoneUpdate(projectElement, ns, @"Documentation\**\*.db");

            document.Save(csprojPath);

            Console.WriteLine($"[PROJECT] Documentation database CopyAlways rules ensured in '{csprojPath}'.");
        }

        /// <summary>
        ///     Removes generated documentation database copy directives from the project file.
        /// </summary>
        public static void RemoveDocumentationDatabasesCopyAlways(string csprojPath, bool enable)
        {
            if (!enable)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(csprojPath))
            {
                throw new ArgumentException("csprojPath is required.", nameof(csprojPath));
            }

            if (!File.Exists(csprojPath))
            {
                throw new FileNotFoundException("Project file not found.", csprojPath);
            }

            XDocument document = XDocument.Load(csprojPath, LoadOptions.PreserveWhitespace);
            XElement projectElement = document.Root
                                      ?? throw new InvalidOperationException("Invalid .csproj: missing root element.");

            XNamespace ns = projectElement.Name.Namespace;

            RemoveNoneUpdate(projectElement, ns, @"Documentation\data.db");
            RemoveNoneUpdate(projectElement, ns, @"Documentation\*.db");
            RemoveNoneUpdate(projectElement, ns, @"Documentation\**\*.db");

            RemoveEmptyItemGroups(projectElement, ns);

            document.Save(csprojPath);

            Console.WriteLine($"[PROJECT] Documentation database CopyAlways rules removed from '{csprojPath}'.");
        }

        #endregion

        #region Private methods

        private static void EnsureNoneUpdate(XElement projectElement, XNamespace ns, string updateValue)
        {
            XElement? existingNoneElement = projectElement
                .Elements(ns + "ItemGroup")
                .Elements(ns + "None")
                .FirstOrDefault(x => string.Equals((string?)x.Attribute("Update"), updateValue, StringComparison.Ordinal));

            if (existingNoneElement != null)
            {
                EnsureCopyToOutputAlways(existingNoneElement, ns);
                return;
            }

            XElement? targetItemGroup = FindReusableItemGroup(projectElement, ns);

            if (targetItemGroup == null)
            {
                targetItemGroup = new XElement(ns + "ItemGroup");
                projectElement.Add(targetItemGroup);
            }

            XElement noneElement = new XElement(ns + "None");
            noneElement.SetAttributeValue("Update", updateValue);
            noneElement.Add(new XElement(ns + "CopyToOutputDirectory", "Always"));

            targetItemGroup.Add(noneElement);
        }

        private static void RemoveNoneUpdate(XElement projectElement, XNamespace ns, string updateValue)
        {
            List<XElement> noneElements = projectElement
                .Elements(ns + "ItemGroup")
                .Elements(ns + "None")
                .Where(x => string.Equals((string?)x.Attribute("Update"), updateValue, StringComparison.Ordinal))
                .ToList();

            foreach (XElement noneElement in noneElements)
            {
                noneElement.Remove();
            }
        }

        private static void RemoveEmptyItemGroups(XElement projectElement, XNamespace ns)
        {
            List<XElement> emptyItemGroups = projectElement
                .Elements(ns + "ItemGroup")
                .Where(itemGroup => !itemGroup.Elements().Any())
                .ToList();

            foreach (XElement itemGroup in emptyItemGroups)
            {
                itemGroup.Remove();
            }
        }

        private static XElement? FindReusableItemGroup(XElement projectElement, XNamespace ns)
        {
            return projectElement
                .Elements(ns + "ItemGroup")
                .FirstOrDefault(itemGroup =>
                    itemGroup.Elements(ns + "None").Any() &&
                    !itemGroup.Elements().Any(x => x.Name != ns + "None"));
        }

        private static void EnsureCopyToOutputAlways(XElement noneElement, XNamespace ns)
        {
            XElement? copyElement = noneElement.Element(ns + "CopyToOutputDirectory");

            if (copyElement == null)
            {
                noneElement.Add(new XElement(ns + "CopyToOutputDirectory", "Always"));
                return;
            }

            copyElement.Value = "Always";
        }

        #endregion
    }
}