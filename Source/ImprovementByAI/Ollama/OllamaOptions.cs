#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBDocumentationImprovementByAI;

#endregion

namespace DMBDocumentationImprovementByOllama
{
    /// <summary>
    ///     Configures Ollama documentation improvement execution.
    /// </summary>
    public sealed class OllamaOptions
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the SQLite documentation database path to improve.
        /// </summary>
        public string DatabasePath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets a value indicating whether existing AI output should be regenerated.
        /// </summary>
        public bool ForceRegenerate { get; init; } = false;

        /// <summary>
        ///     Gets the custom prompt fragment used for keyword generation.
        /// </summary>
        public string KeywordsPrompt { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the maximum model JSON length included in prompts.
        /// </summary>
        public int MaxModelJsonLength { get; init; } = 8000;

        /// <summary>
        ///     Gets the maximum number of documentation objects to process; zero means no explicit limit.
        /// </summary>
        public int MaxObjectsToProcess { get; init; } = 0;

        /// <summary>
        ///     Gets the Ollama model selected for generation.
        /// </summary>
        public OllamaModel Model { get; init; } = OllamaModel.Qwen25Coder14B;

        /// <summary>
        ///     Gets the documentation object selection mode used before AI generation starts.
        /// </summary>
        public DocumentationAIObjectSelectionMode ObjectSelectionMode { get; init; } = DocumentationAIObjectSelectionMode.LatestVersion;

        /// <summary>
        ///     Gets the project context prompt fragment included in AI requests.
        /// </summary>
        public string ProjectContextPrompt { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the timeout applied to each AI request.
        /// </summary>
        public TimeSpan RequestTimeout { get; init; } = TimeSpan.FromMinutes(10);

        /// <summary>
        ///     Gets the custom prompt fragment used for short summary generation.
        /// </summary>
        public string ShortSummaryPrompt { get; init; } = string.Empty;

        /// <summary>
        ///     Gets a value indicating whether the Ollama server should be started when it is not already running.
        /// </summary>
        public bool StartOllamaServerIfNeeded { get; init; } = true;

        /// <summary>
        ///     Gets a value indicating whether the Ollama model should be unloaded after processing.
        /// </summary>
        public bool StopModelWhenFinished { get; init; } = true;

        /// <summary>
        ///     Gets the custom prompt fragment used for summary generation.
        /// </summary>
        public string SummaryPrompt { get; init; } = string.Empty;

        #endregion
    }
}