#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBDocumentationBuilder;
using NUnit.Framework;

#endregion

namespace DMBDocumentationBuilderUnitTest;

[TestFixture]
public sealed class DocumentationXmlModelTests
{
    [Test]
    public void HasPropertiesAreFalseForEmptyModel()
    {
        DocumentationXmlModel model = new();

        Assert.Multiple(() =>
        {
            Assert.That(model.HasExample, Is.False);
            Assert.That(model.HasExceptions, Is.False);
            Assert.That(model.HasParameters, Is.False);
            Assert.That(model.HasRemarks, Is.False);
            Assert.That(model.HasReturns, Is.False);
            Assert.That(model.HasSeeAlsos, Is.False);
            Assert.That(model.HasSummary, Is.False);
            Assert.That(model.HasTypeParameters, Is.False);
            Assert.That(model.HasValue, Is.False);
        });
    }

    [Test]
    public void HasPropertiesReflectXmlContentAndCollections()
    {
        DocumentationXmlModel model = new()
        {
            ExampleHtml = "<code>example</code>",
            RemarksHtml = "<p>remarks</p>",
            ReturnsHtml = "<p>returns</p>",
            SummaryHtml = "<p>summary</p>",
            ValueHtml = "<p>value</p>"
        };

        model.Exceptions.Add(new DocumentationXmlNamedItem { Name = "InvalidOperationException", Html = "error" });
        model.Parameters.Add(new DocumentationXmlNamedItem { Name = "value", Html = "value" });
        model.SeeAlsos.Add(new DocumentationXmlLinkItem { Label = "Other", Href = "/other" });
        model.TypeParameters.Add(new DocumentationXmlNamedItem { Name = "TValue", Html = "value type" });

        Assert.Multiple(() =>
        {
            Assert.That(model.HasExample, Is.True);
            Assert.That(model.HasExceptions, Is.True);
            Assert.That(model.HasParameters, Is.True);
            Assert.That(model.HasRemarks, Is.True);
            Assert.That(model.HasReturns, Is.True);
            Assert.That(model.HasSeeAlsos, Is.True);
            Assert.That(model.HasSummary, Is.True);
            Assert.That(model.HasTypeParameters, Is.True);
            Assert.That(model.HasValue, Is.True);
        });
    }
}