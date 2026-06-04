#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBExampleToRaw;

/// <summary>
///     Describes the source and output settings used by <see cref="DMBExampleToRawAgent" />.
/// </summary>
public sealed class DMBExampleToRawOptions
{
    #region Instance fields and properties

    /// <summary>
    ///     Gets a value indicating whether the target directory is deleted before files are generated.
    /// </summary>
    public bool CleanTargetDirectory { get; init; } = true;

    /// <summary>
    ///     Gets the HTML fragment written before each escaped source example.
    /// </summary>
    public string RawCodePrefix { get; init; } = "<div class=\"dmb-demo-code border rounded-3 overflow-hidden bg-body\"><div class=\"dmb-demo-code-header px-3 py-2 border-bottom bg-body-tertiary d-flex align-items-center\"><span class=\"badge text-bg-secondary font-monospace\">C#</span></div><pre class=\"language-csharp mb-0 border-0 rounded-0\"><code class=\"language-csharp\">";

    /// <summary>
    ///     Gets the HTML fragment written after each escaped source example.
    /// </summary>
    public string RawCodeSuffix { get; init; } = "</code></pre></div>";

    /// <summary>
    ///     Gets the source directory containing example partial files.
    /// </summary>
    public required string SourceDirectoryPath { get; init; }

    /// <summary>
    ///     Gets the file search pattern used to find source examples.
    /// </summary>
    /// <remarks>
    ///     The default value targets Razor partial files.
    /// </remarks>
    public string SourceSearchPattern { get; init; } = "*.cshtml";

    /// <summary>
    ///     Gets the target directory that receives generated raw example partial files.
    /// </summary>
    public required string TargetDirectoryPath { get; init; }

    #endregion
}