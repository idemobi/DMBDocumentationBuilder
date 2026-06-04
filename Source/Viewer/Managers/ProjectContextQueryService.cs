#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Provides read-only SQLite queries for project-context files stored with generated documentation.
    /// </summary>
    /// <remarks>
    ///     Project context records are intended for AI and developer assistance. This service formats those
    ///     records into compact text blocks without modifying the generated documentation database.
    /// </remarks>
    public sealed class ProjectContextQueryService
    {
        #region Instance fields and properties

        private readonly string _sqliteDatabasePath;

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectContextQueryService" /> class.
        /// </summary>
        /// <param name="sqliteDatabasePath">Absolute or relative path to the generated documentation SQLite database.</param>
        public ProjectContextQueryService(string sqliteDatabasePath)
        {
            _sqliteDatabasePath = sqliteDatabasePath;
        }

        #endregion

        #region Instance methods

        /// <summary>
        ///     Gets all project-context files for a package version as a merged text bundle.
        /// </summary>
        /// <param name="packageId">Package identifier whose context files should be loaded.</param>
        /// <param name="version">Package version whose context files should be loaded.</param>
        /// <returns>A formatted context bundle, or an explanatory message when no context file exists.</returns>
        public string GetProjectContext(string packageId, string version)
        {
            IReadOnlyList<ProjectContextFileResult> results = GetProjectContextFiles(packageId, version);

            if (results.Count == 0)
            {
                return $"No project context found for package '{packageId}' version '{version}'.";
            }

            List<string> blocks = [];

            foreach (ProjectContextFileResult item in results)
            {
                StringBuilder block = new();
                block.AppendLine($"# Context File: {item.FileName}");
                block.AppendLine($"Path: {item.FilePath}");
                block.AppendLine($"Type: {item.ContextType}");
                block.AppendLine($"Source: {item.SourceFolderType}");
                block.AppendLine($"Depth: {item.DirectoryDepth}");
                block.AppendLine();
                block.AppendLine(item.Content);

                blocks.Add(block.ToString().Trim());
            }

            return string.Join("\n\n", blocks);
        }

        /// <summary>
        ///     Gets the ordered project-context file records for a package version.
        /// </summary>
        /// <param name="packageId">Package identifier whose context files should be loaded.</param>
        /// <param name="version">Package version whose context files should be loaded.</param>
        /// <returns>Project-context file records ordered by source folder, directory depth, context type, and file name.</returns>
        public IReadOnlyList<ProjectContextFileResult> GetProjectContextFiles(string packageId, string version)
        {
            using SqliteConnection connection = OpenConnection();

            const string sql = """
                               SELECT FileName, FilePath, ContextType, SourceFolderType, DirectoryDepth, Content
                               FROM DocumentationProjectContextFiles
                               WHERE PackageId = @PackageId
                                 AND Version = @Version
                               ORDER BY
                                 CASE SourceFolderType
                                     WHEN '.aiassistant' THEN 0
                                     WHEN '.ai' THEN 1
                                     ELSE 100
                                 END,
                                 DirectoryDepth,
                                 CASE ContextType
                                     WHEN 'Rules' THEN 0
                                     WHEN 'Architecture' THEN 1
                                     WHEN 'Project' THEN 2
                                     WHEN 'Domain' THEN 3
                                     WHEN 'CodingStyle' THEN 4
                                     WHEN 'Glossary' THEN 5
                                     WHEN 'Readme' THEN 6
                                     ELSE 100
                                 END,
                                 FileName
                               """;

            using SqliteCommand command = new(sql, connection);
            command.Parameters.AddWithValue("@PackageId", packageId);
            command.Parameters.AddWithValue("@Version", version);

            using SqliteDataReader reader = command.ExecuteReader();

            List<ProjectContextFileResult> results = [];

            while (reader.Read())
            {
                results.Add(new ProjectContextFileResult
                {
                    FileName = reader["FileName"]?.ToString() ?? string.Empty,
                    FilePath = reader["FilePath"]?.ToString() ?? string.Empty,
                    ContextType = reader["ContextType"]?.ToString() ?? string.Empty,
                    SourceFolderType = reader["SourceFolderType"]?.ToString() ?? string.Empty,
                    DirectoryDepth = Convert.ToInt32(reader["DirectoryDepth"]),
                    Content = reader["Content"]?.ToString() ?? string.Empty
                });
            }

            return results;
        }

        /// <summary>
        ///     Lists project-context files for a package version without including their full content.
        /// </summary>
        /// <param name="packageId">Package identifier whose context files should be listed.</param>
        /// <param name="version">Package version whose context files should be listed.</param>
        /// <returns>A formatted file list, or an explanatory message when no context file exists.</returns>
        public string ListProjectContextFiles(string packageId, string version)
        {
            IReadOnlyList<ProjectContextFileResult> results = GetProjectContextFiles(packageId, version);

            if (results.Count == 0)
            {
                return $"No project context files found for package '{packageId}' version '{version}'.";
            }

            List<string> lines = [];

            foreach (ProjectContextFileResult item in results)
            {
                lines.Add($"- {item.FileName} [{item.ContextType}] {item.SourceFolderType} depth={item.DirectoryDepth}\n  {item.FilePath}");
            }

            return string.Join("\n\n", lines);
        }

        private SqliteConnection OpenConnection()
        {
            SqliteConnection connection = new($"Data Source={_sqliteDatabasePath}");
            connection.Open();
            return connection;
        }

        /// <summary>
        ///     Searches project-context file contents for a package version.
        /// </summary>
        /// <param name="query">Text used in the SQLite `LIKE` filter.</param>
        /// <param name="packageId">Package identifier whose context files should be searched.</param>
        /// <param name="version">Package version whose context files should be searched.</param>
        /// <returns>A formatted list of matching context excerpts, or an explanatory message when no result exists.</returns>
        public string SearchProjectContext(string query, string packageId, string version)
        {
            using SqliteConnection connection = OpenConnection();

            const string sql = """
                               SELECT FileName, FilePath, ContextType, SourceFolderType, DirectoryDepth, Content
                               FROM DocumentationProjectContextFiles
                               WHERE PackageId = @PackageId
                                 AND Version = @Version
                                 AND Content LIKE @Query
                               ORDER BY
                                 CASE SourceFolderType
                                     WHEN '.aiassistant' THEN 0
                                     WHEN '.ai' THEN 1
                                     ELSE 100
                                 END,
                                 DirectoryDepth,
                                 FileName
                               LIMIT 20
                               """;

            using SqliteCommand command = new(sql, connection);
            command.Parameters.AddWithValue("@PackageId", packageId);
            command.Parameters.AddWithValue("@Version", version);
            command.Parameters.AddWithValue("@Query", $"%{query}%");

            using SqliteDataReader reader = command.ExecuteReader();

            List<string> blocks = [];

            while (reader.Read())
            {
                string fileName = reader["FileName"]?.ToString() ?? string.Empty;
                string filePath = reader["FilePath"]?.ToString() ?? string.Empty;
                string contextType = reader["ContextType"]?.ToString() ?? string.Empty;
                string sourceFolderType = reader["SourceFolderType"]?.ToString() ?? string.Empty;
                int directoryDepth = Convert.ToInt32(reader["DirectoryDepth"]);
                string content = reader["Content"]?.ToString() ?? string.Empty;

                StringBuilder block = new();
                block.AppendLine($"# {fileName}");
                block.AppendLine($"Type: {contextType}");
                block.AppendLine($"Source: {sourceFolderType}");
                block.AppendLine($"Depth: {directoryDepth}");
                block.AppendLine($"Path: {filePath}");
                block.AppendLine();
                block.AppendLine(DocumentationMcpTextFormatter.LimitText(content, 4000));

                blocks.Add(block.ToString().Trim());
            }

            if (blocks.Count == 0)
            {
                return $"No project context result found for '{query}' in package '{packageId}' version '{version}'.";
            }

            return string.Join("\n\n", blocks);
        }

        #endregion
    }
}