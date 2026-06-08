#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.IO;
using DMBDocumentationBuilder;
using NUnit.Framework;

#endregion

namespace DMBDocumentationBuilderUnitTest;

[TestFixture]
public sealed class DocumentationDatabaseManagerPurgeTests
{
    [Test]
    public void PurgeVersionsRejectsGlobalAndMiddleWildcards()
    {
        string databasePath = CreateDatabasePath();

        try
        {
            Assert.That(
                () => DocumentationDatabaseManager.PurgeVersions(databasePath, "*"),
                Throws.ArgumentException);
            Assert.That(
                () => DocumentationDatabaseManager.PurgeVersions(databasePath, "1.*.3"),
                Throws.ArgumentException);
        }
        finally
        {
            DeleteDatabase(databasePath);
        }
    }

    [Test]
    public void PurgeVersionsRemovesExactVersionRows()
    {
        string databasePath = CreateDatabasePath();

        try
        {
            SaveObject(databasePath, "1.2", "ExactVersionObject");
            SaveObject(databasePath, "1.3", "KeptVersionObject");

            int deletedRowCount = DocumentationDatabaseManager.PurgeVersions(databasePath, "1.2");

            Assert.That(deletedRowCount, Is.EqualTo(1));
            Assert.That(DocumentationDatabaseManager.PurgeVersions(databasePath, "1.2"), Is.EqualTo(0));
            Assert.That(DocumentationDatabaseManager.PurgeVersions(databasePath, "1.3"), Is.EqualTo(1));
        }
        finally
        {
            DeleteDatabase(databasePath);
        }
    }

    [Test]
    public void PurgeVersionsRemovesWildcardPrefixRows()
    {
        string databasePath = CreateDatabasePath();

        try
        {
            SaveObject(databasePath, "1.2", "PrefixVersionObject");
            SaveObject(databasePath, "1.2.5", "ChildVersionObject");
            SaveObject(databasePath, "1.20", "NeighborVersionObject");

            int deletedRowCount = DocumentationDatabaseManager.PurgeVersions(databasePath, "1.2.*");

            Assert.That(deletedRowCount, Is.EqualTo(2));
            Assert.That(DocumentationDatabaseManager.PurgeVersions(databasePath, "1.20"), Is.EqualTo(1));
        }
        finally
        {
            DeleteDatabase(databasePath);
        }
    }

    private static string CreateDatabasePath()
    {
        string directoryPath = Path.Combine(Path.GetTempPath(), "DMBDocumentationBuilderUnitTests");
        Directory.CreateDirectory(directoryPath);

        return Path.Combine(directoryPath, $"{Guid.NewGuid():N}.db");
    }

    private static void DeleteDatabase(string databasePath)
    {
        foreach (string filePath in new[] { databasePath, $"{databasePath}-shm", $"{databasePath}-wal" })
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    private static void SaveObject(
        string databasePath,
        string version,
        string objectName
    )
    {
        DocumentationDatabaseManager.SaveObject(
            databasePath,
            "TestPackage",
            version,
            "TestNamespace",
            objectName,
            "Class",
            new { objectName, version },
            $"<p>{objectName}</p>",
            objectName,
            objectName,
            $"/documentation/test/{version}/{objectName}");
    }
}
