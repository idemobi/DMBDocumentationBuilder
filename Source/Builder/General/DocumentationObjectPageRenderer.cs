#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Text;

#endregion

namespace DMBDocumentationBuilder
{
    internal static class DocumentationObjectPageRenderer
    {
        #region Static methods

        /// <summary>
        ///     Writes generated documentation pages for the supplied documentation models.
        /// </summary>
        /// <param name="items">The items value used by the documentation generation operation.</param>
        /// <param name="outputDirectory">The outputDirectory value used by the documentation generation operation.</param>
        public static void RenderPages(
            IEnumerable<DocumentationObjectItem> items,
            string outputDirectory
        )
        {
            Directory.CreateDirectory(outputDirectory);

            foreach (DocumentationObjectItem item in items)
            {
                string namespaceFolder = DocumentationPathHelper.NamespaceToFolder(item.NamespaceName);
                string finalDirectory = Path.Combine(outputDirectory, namespaceFolder);

                Directory.CreateDirectory(finalDirectory);

                string filePath = Path.Combine(finalDirectory, $"{item.ObjectName}.cshtml");

                StringBuilder sb = new();
                sb.AppendLine("@* Auto-generated. Do not edit manually. *@");
                sb.AppendLine("@{");
                sb.AppendLine("    Layout = \"_Layout\";");
                sb.AppendLine("}");
                sb.Append("<h1>")
                    .Append(System.Net.WebUtility.HtmlEncode(item.KindName))
                    .Append(' ')
                    .Append(System.Net.WebUtility.HtmlEncode(item.ObjectName))
                    .AppendLine("</h1>");

                File.WriteAllText(filePath, sb.ToString());
            }
        }

        #endregion
    }
}