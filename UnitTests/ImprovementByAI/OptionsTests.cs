#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBDocumentationImprovementByAI;
using DMBDocumentationImprovementByLMStudio;
using DMBDocumentationImprovementByOllama;
using DMBDocumentationImprovementByOpenAI;
using NUnit.Framework;

#endregion

namespace DMBDocumentationImprovementByAIUnitTest;

[TestFixture]
public sealed class OptionsTests
{
    [Test]
    public void LMStudioOptionsExposeStableDefaults()
    {
        LMStudioOptions options = new();

        Assert.Multiple(() =>
        {
            Assert.That(options.ApiToken, Is.Empty);
            Assert.That(options.BaseUrl, Is.EqualTo("http://localhost:1234/v1/"));
            Assert.That(options.DatabasePath, Is.Empty);
            Assert.That(options.ForceRegenerate, Is.False);
            Assert.That(options.MaxModelJsonLength, Is.EqualTo(8000));
            Assert.That(options.MaxObjectsToProcess, Is.EqualTo(0));
            Assert.That(options.Model, Is.EqualTo(LMStudioModel.Gemma4));
            Assert.That(options.ObjectSelectionMode, Is.EqualTo(DocumentationAIObjectSelectionMode.LatestVersion));
            Assert.That(options.RequestTimeout, Is.EqualTo(TimeSpan.FromMinutes(10)));
        });
    }

    [Test]
    public void OllamaOptionsExposeStableDefaults()
    {
        OllamaOptions options = new();

        Assert.Multiple(() =>
        {
            Assert.That(options.DatabasePath, Is.Empty);
            Assert.That(options.ForceRegenerate, Is.False);
            Assert.That(options.MaxModelJsonLength, Is.EqualTo(8000));
            Assert.That(options.MaxObjectsToProcess, Is.EqualTo(0));
            Assert.That(options.Model, Is.EqualTo(OllamaModel.Qwen25Coder14B));
            Assert.That(options.ObjectSelectionMode, Is.EqualTo(DocumentationAIObjectSelectionMode.LatestVersion));
            Assert.That(options.RequestTimeout, Is.EqualTo(TimeSpan.FromMinutes(10)));
            Assert.That(options.StartOllamaServerIfNeeded, Is.True);
            Assert.That(options.StopModelWhenFinished, Is.True);
        });
    }

    [Test]
    public void OpenAIOptionsExposeStableDefaults()
    {
        OpenAIOptions options = new();

        Assert.Multiple(() =>
        {
            Assert.That(options.ApiKey, Is.Null);
            Assert.That(options.DatabasePath, Is.Empty);
            Assert.That(options.ForceRegenerate, Is.False);
            Assert.That(options.MaxModelJsonLength, Is.EqualTo(8000));
            Assert.That(options.MaxObjectsToProcess, Is.EqualTo(0));
            Assert.That(options.Model, Is.EqualTo(OpenAIModel.Gpt54Mini));
            Assert.That(options.ObjectSelectionMode, Is.EqualTo(DocumentationAIObjectSelectionMode.LatestVersion));
            Assert.That(options.RequestTimeout, Is.EqualTo(TimeSpan.FromMinutes(5)));
        });
    }
}