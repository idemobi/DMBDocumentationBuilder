#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Text;

#endregion

namespace DMBExampleToRaw;

/// <summary>
///     Generates escaped raw-code Razor partials from source example partials.
/// </summary>
/// <remarks>
///     The generated files are intended for documentation and demo pages that show the source code beside rendered
///     examples.
/// </remarks>
public sealed class DMBExampleToRawAgent
{
    #region Instance methods

    /// <summary>
    ///     Generates raw example files from the configured source directory.
    /// </summary>
    /// <param name="options">The generation options containing source and target paths.</param>
    /// <returns>The number of generated raw example files.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">Thrown when a required path option is empty or whitespace.</exception>
    /// <exception cref="DirectoryNotFoundException">
    ///     Thrown when <see cref="DMBExampleToRawOptions.SourceDirectoryPath" /> does
    ///     not exist.
    /// </exception>
    public int GenerateRawFiles(DMBExampleToRawOptions options)
    {
        options = options ?? throw new ArgumentNullException(nameof(options));

        ValidateOptions(options);

        if (!Directory.Exists(options.SourceDirectoryPath))
        {
            throw new DirectoryNotFoundException($"Directory not found: {options.SourceDirectoryPath}");
        }

        Console.WriteLine($"Source : {options.SourceDirectoryPath}");
        Console.WriteLine($"Target : {options.TargetDirectoryPath}");
        Console.WriteLine();

        if (options.CleanTargetDirectory && Directory.Exists(options.TargetDirectoryPath))
        {
            Directory.Delete(options.TargetDirectoryPath, true);
        }

        Directory.CreateDirectory(options.TargetDirectoryPath);

        IEnumerable<string> files = Directory
            .EnumerateFiles(options.SourceDirectoryPath, options.SourceSearchPattern, SearchOption.AllDirectories)
            .OrderBy(file => file, StringComparer.Ordinal);

        int generatedCount = 0;

        foreach (string sourceFile in files)
        {
            string relativePath = Path.GetRelativePath(options.SourceDirectoryPath, sourceFile);
            string targetRelativePath = Path.ChangeExtension(relativePath, null) + "_Raw.cshtml";
            string targetFilePath = Path.Combine(options.TargetDirectoryPath, targetRelativePath);

            Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath)!);

            Console.WriteLine($"Source : {relativePath}");
            Console.WriteLine($"Target : {targetRelativePath}");

            string content = File.ReadAllText(sourceFile, Encoding.UTF8);
            string processedContent = ProcessContent(content);

            StringBuilder builder = new();
            builder.Append(options.RawCodePrefix);
            builder.Append(processedContent);
            builder.Append(options.RawCodeSuffix);

            File.WriteAllText(targetFilePath, builder.ToString(), Encoding.UTF8);

            generatedCount++;

            Console.WriteLine("Done");
            Console.WriteLine();
        }

        Console.WriteLine($"All _Raw.cshtml files generated in {options.TargetDirectoryPath}.");

        return generatedCount;
    }

    #endregion

    #region Private methods

    private static string ProcessContent(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        string text = input.Replace("\r\n", "\n");

        text = text.Trim();

        text = text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;");

        text = text.Replace("@", "@@");

        return text;
    }

    private static void ValidateOptions(DMBExampleToRawOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.SourceDirectoryPath))
        {
            throw new ArgumentException("The source directory path is required.", nameof(options));
        }

        if (string.IsNullOrWhiteSpace(options.TargetDirectoryPath))
        {
            throw new ArgumentException("The target directory path is required.", nameof(options));
        }

        if (string.IsNullOrWhiteSpace(options.SourceSearchPattern))
        {
            throw new ArgumentException("The source search pattern is required.", nameof(options));
        }
    }

    #endregion
}