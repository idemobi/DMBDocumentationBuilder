#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBDocumentationImprovementByClaude;
using DMBDocumentationImprovementByGroq;
using DMBDocumentationImprovementByLMStudio;
using DMBDocumentationImprovementByMistral;
using DMBDocumentationImprovementByOllama;
using DMBDocumentationImprovementByOpenAI;
using NUnit.Framework;

#endregion

namespace DMBDocumentationImprovementByAIUnitTest;

[TestFixture]
public sealed class ModelHelperTests
{
    [Test]
    public void ClaudeModelHelperMapsCapabilitiesDefaultsAndFallbacks()
    {
        Assert.Multiple(() =>
        {
            Assert.That(ClaudeModel.ClaudeOpus46.ToModelString(), Is.EqualTo("claude-opus-4-6"));
            Assert.That(ClaudeModel.ClaudeSonnet46.ToModelString(), Is.EqualTo("claude-sonnet-4-6"));
            Assert.That(ClaudeModel.ClaudeHaiku45.ToModelString(), Is.EqualTo("claude-haiku-4-5"));
            Assert.That(ClaudeModelHelper.GetRecommendedDefault(), Is.EqualTo(ClaudeModel.ClaudeSonnet46));
            Assert.That(ClaudeModel.ClaudeOpus46.GetFallbackModel(), Is.EqualTo(ClaudeModel.ClaudeSonnet46));
            Assert.That(ClaudeModel.ClaudeSonnet46.GetFallbackModel(), Is.EqualTo(ClaudeModel.ClaudeHaiku45));
            Assert.That(ClaudeModel.ClaudeHaiku45.IsFastModel(), Is.True);
            Assert.That(ClaudeModel.ClaudeOpus46.IsHighQualityModel(), Is.True);
            Assert.That(ClaudeModel.ClaudeSonnet46.IsHighQualityModel(), Is.True);
        });
    }

    [Test]
    public void GroqModelHelperMapsCapabilitiesDefaultsAndFallbacks()
    {
        Assert.Multiple(() =>
        {
            Assert.That(GroqModel.Llama31_8B_Instant.ToModelString(), Is.EqualTo("llama-3.1-8b-instant"));
            Assert.That(GroqModel.Llama31_70B_Versatile.ToModelString(), Is.EqualTo("llama-3.1-70b-versatile"));
            Assert.That(GroqModel.Mixtral_8x7B.ToModelString(), Is.EqualTo("mixtral-8x7b-32768"));
            Assert.That(GroqModelHelper.GetRecommendedDefault(), Is.EqualTo(GroqModel.Llama31_8B_Instant));
            Assert.That(GroqModel.Llama31_70B_Versatile.GetFallbackModel(), Is.EqualTo(GroqModel.Llama31_8B_Instant));
            Assert.That(GroqModel.Llama31_8B_Instant.IsFastModel(), Is.True);
            Assert.That(GroqModel.Llama31_70B_Versatile.IsHighQualityModel(), Is.True);
        });
    }

    [Test]
    public void LMStudioModelHelperMapsModelStringsStorageKeysAndDefault()
    {
        Assert.Multiple(() =>
        {
            Assert.That(LMStudioModel.DeepSeekCoder.ToModelString(), Is.EqualTo("deepseek-coder"));
            Assert.That(LMStudioModel.Qwen2_5Coder.ToModelString(), Is.EqualTo("qwen/qwen2.5-coder-14b"));
            Assert.That(LMStudioModel.Gemma4.ToModelString(), Is.EqualTo("google/gemma-4-e4b"));
            Assert.That(LMStudioModel.Qwen2_5Coder.ToStorageKey(), Is.EqualTo("Qwen2_5Coder_14b"));
            Assert.That(LMStudioModelHelper.GetRecommendedDefault(), Is.EqualTo(LMStudioModel.Gemma4));
        });
    }

    [Test]
    public void MistralModelHelperMapsCodingDefaultsAndFallbacks()
    {
        Assert.Multiple(() =>
        {
            Assert.That(MistralModel.MistralLargeLatest.ToModelString(), Is.EqualTo("mistral-large-latest"));
            Assert.That(MistralModel.CodestralLatest.ToModelString(), Is.EqualTo("codestral-latest"));
            Assert.That(MistralModel.DevstralSmallLatest.ToModelString(), Is.EqualTo("devstral-small-latest"));
            Assert.That(MistralModelHelper.GetRecommendedDefault(), Is.EqualTo(MistralModel.MistralMediumLatest));
            Assert.That(MistralModel.MistralLargeLatest.GetFallbackModel(), Is.EqualTo(MistralModel.MistralMediumLatest));
            Assert.That(MistralModel.MistralMediumLatest.GetFallbackModel(), Is.EqualTo(MistralModel.Ministral8BLatest));
            Assert.That(MistralModel.CodestralLatest.GetFallbackModel(), Is.EqualTo(MistralModel.DevstralSmallLatest));
            Assert.That(MistralModel.CodestralLatest.IsCodingModel(), Is.True);
            Assert.That(MistralModel.DevstralSmallLatest.IsCodingModel(), Is.True);
        });
    }

    [Test]
    public void OllamaModelHelperMapsCapabilitiesDefaultsSizesAndFallbacks()
    {
        Assert.Multiple(() =>
        {
            Assert.That(OllamaModel.Qwen25Coder14B.ToModelString(), Is.EqualTo("qwen2.5-coder:14b"));
            Assert.That(OllamaModel.Gemma3_27B.ToModelString(), Is.EqualTo("gemma3:27b"));
            Assert.That(OllamaModelHelper.GetRecommendedDefault(), Is.EqualTo(OllamaModel.Qwen25Coder14B));
            Assert.That(OllamaModel.Qwen25Coder32B.GetEstimatedSizeInGB(), Is.EqualTo(20));
            Assert.That(OllamaModel.Phi3Mini.GetEstimatedSizeInGB(), Is.EqualTo(2));
            Assert.That(OllamaModel.Qwen25Coder32B.IsHeavyModel(), Is.True);
            Assert.That(OllamaModel.Qwen25Coder7B.IsCodingModel(), Is.True);
            Assert.That(OllamaModel.Qwen25Coder32B.GetFallbackModel(), Is.EqualTo(OllamaModel.Qwen25Coder14B));
            Assert.That(OllamaModel.Gemma3_12B.GetFallbackModel(), Is.EqualTo(OllamaModel.Llama31_8B));
            Assert.That(OllamaModel.Llama31_8B.GetFallbackModel(), Is.EqualTo(OllamaModel.Mistral7B));
        });
    }

    [Test]
    public void OpenAIModelHelperMapsModelStringsDefaultsAndFallbacks()
    {
        Assert.Multiple(() =>
        {
            Assert.That(OpenAIModel.Gpt54.ToModelString(), Is.EqualTo("gpt-5.4"));
            Assert.That(OpenAIModel.Gpt54Mini.ToModelString(), Is.EqualTo("gpt-5.4-mini"));
            Assert.That(OpenAIModel.Gpt54Nano.ToModelString(), Is.EqualTo("gpt-5.4-nano"));
            Assert.That(OpenAIModelHelper.GetRecommendedDefault(), Is.EqualTo(OpenAIModel.Gpt54Mini));
            Assert.That(OpenAIModel.Gpt54.GetFallbackModel(), Is.EqualTo(OpenAIModel.Gpt54Mini));
            Assert.That(OpenAIModel.Gpt54Mini.GetFallbackModel(), Is.EqualTo(OpenAIModel.Gpt54Nano));
            Assert.That(OpenAIModel.Gpt54Nano.GetFallbackModel(), Is.EqualTo(OpenAIModel.Gpt54Nano));
        });
    }
}