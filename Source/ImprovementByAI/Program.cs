#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBDocumentationImprovementByAI;
using DMBDocumentationImprovementByLMStudio;
using DMBDocumentationImprovementByOllama;

#endregion

ProjectFileHelper.EnsureDocumentationDatabasesCopyAlways("../../../../../../Source/labs_idemobi_com/labs_idemobi_com.csproj", true);

int numberOfObjectToProcess = 60;

string projectContextPrompt =
    """
    This database contains generated technical documentation for a .NET / C# codebase.
    The generated AI text will be displayed in final HTML pages to help AI systems and advanced readers.
    Prefer architectural role, responsibility, likely usage, and integration context.
    Avoid marketing style and avoid repeating the object name excessively.
    """;
string summaryPrompt =
    """
    When possible, explain where the object probably fits in the documentation pipeline,
    what kind of responsibility it carries, and what another component might expect from it.
    """;

string shortSummaryPrompt =
    """
    The sentence must be compact, precise, and useful as a quick machine-readable descriptor.
    """;

string keywordsPrompt =
    """
    Prefer nouns and stable technical phrases. Avoid generic words such as object, system, tool, simple, data.
    """;

OllamaRuntime.Run(new OllamaOptions
{
    DatabasePath = "../../../../../../labs_idemobi_com/Documentation/data.db",
    Model = OllamaModel.Phi3Mini,
    StartOllamaServerIfNeeded = true,
    StopModelWhenFinished = true,
    ForceRegenerate = false,
    MaxObjectsToProcess = numberOfObjectToProcess,
    ProjectContextPrompt = projectContextPrompt,
    SummaryPrompt = summaryPrompt,
    ShortSummaryPrompt = shortSummaryPrompt,
    KeywordsPrompt = keywordsPrompt
});

LMStudioRuntime.Run(new LMStudioOptions()
{
    DatabasePath = "../../../../../../labs_idemobi_com/Documentation/data.db",
    Model = LMStudioModel.Qwen2_5Coder,
    ForceRegenerate = false,
    MaxObjectsToProcess = numberOfObjectToProcess,
    ProjectContextPrompt = projectContextPrompt,
    SummaryPrompt = summaryPrompt,
    ShortSummaryPrompt = shortSummaryPrompt,
    KeywordsPrompt = keywordsPrompt
});

LMStudioRuntime.Run(new LMStudioOptions()
{
    DatabasePath = "../../../../../../labs_idemobi_com/Documentation/data.db",
    Model = LMStudioModel.Gemma4,
    ForceRegenerate = false,
    MaxObjectsToProcess = numberOfObjectToProcess,
    ProjectContextPrompt = projectContextPrompt,
    SummaryPrompt = summaryPrompt,
    ShortSummaryPrompt = shortSummaryPrompt,
    KeywordsPrompt = keywordsPrompt
});