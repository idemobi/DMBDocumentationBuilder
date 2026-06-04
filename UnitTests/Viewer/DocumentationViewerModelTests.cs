#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBDocumentationViewer;
using NUnit.Framework;

#endregion

namespace DMBDocumentationViewerUnitTest;

[TestFixture]
public sealed class DocumentationViewerModelTests
{
    [Test]
    public void DefaultDisplayFlagsIncludeAllOptionalMemberFields()
    {
        DocumentationMemberDisplayFlags flags = DocumentationMemberDisplayFlags.Default;

        Assert.Multiple(() =>
        {
            Assert.That(flags.HasFlag(DocumentationMemberDisplayFlags.Signature), Is.True);
            Assert.That(flags.HasFlag(DocumentationMemberDisplayFlags.Summary), Is.True);
            Assert.That(flags.HasFlag(DocumentationMemberDisplayFlags.Obsolete), Is.True);
            Assert.That(flags.HasFlag(DocumentationMemberDisplayFlags.ExtensionType), Is.True);
            Assert.That(flags.HasFlag(DocumentationMemberDisplayFlags.ReferenceLink), Is.True);
            Assert.That((int)flags, Is.EqualTo(31));
        });
    }

    [Test]
    public void DocumentationViewerConfigurationDefaultsMatchPackageContract()
    {
        DMBDocumentationViewerConfiguration configuration = new();

        Assert.Multiple(() =>
        {
            Assert.That(configuration.McpEndpoint, Is.EqualTo("/mcp"));
            Assert.That(configuration.ApiDescription(), Is.False);
            Assert.That(configuration.NeedsConfigFileOrAppSettings(), Is.False);
        });
    }

    [Test]
    public void MemberDisplayEnumsKeepExpectedOrdering()
    {
        Assert.Multiple(() =>
        {
            Assert.That((int)DocumentationMemberDisplayMode.Default, Is.EqualTo(0));
            Assert.That((int)DocumentationMemberDisplayMode.SignatureOnly, Is.EqualTo(1));
            Assert.That((int)DocumentationMemberDisplayMode.SignatureAndDescription, Is.EqualTo(2));
            Assert.That((int)DocumentationMemberKind.Constructor, Is.EqualTo(0));
            Assert.That((int)DocumentationMemberKind.ExtensionMethod, Is.EqualTo(4));
        });
    }
}