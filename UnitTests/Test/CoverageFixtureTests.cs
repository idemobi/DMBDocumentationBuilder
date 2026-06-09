#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBDocumentationTest.Coverage;
using DMBDocumentationTest.Secondary;
using NUnit.Framework;

#endregion

namespace DMBDocumentationTestUnitTest;

[TestFixture]
public sealed class CoverageFixtureTests
{
    [Test]
    public void CoverageExtensionsReturnReadableLabels()
    {
        CoverageIntermediateClass classFixture = new();
        CoverageReadonlyStruct structFixture = new(7, "seven");
        CoverageRecord recordFixture = new("id", "label");

        Assert.Multiple(() =>
        {
            Assert.That(classFixture.ToCoverageLabel("prefix"), Is.EqualTo("prefix:intermediate"));
            Assert.That(structFixture.ToCoverageLabel(), Is.EqualTo("7:seven"));
            Assert.That(recordFixture.ToCoverageLabel(), Is.EqualTo("label"));
        });
    }

    [Test]
    public void CoverageIntermediateClassExposesDeterministicValues()
    {
        CoverageIntermediateClass fixture = new();

        Assert.Multiple(() =>
        {
            Assert.That(fixture.Name, Is.EqualTo("intermediate"));
            Assert.That(fixture.Count, Is.EqualTo(1));
            Assert.That(fixture.Describe(42), Is.EqualTo("42"));
            Assert.Throws<ArgumentNullException>(() => fixture.Describe<string>(null!));
        });
    }

    [Test]
    public void CoverageOperatorValueSupportsAdditionAndConversions()
    {
        CoverageOperatorValue sum = (CoverageOperatorValue)3 + (CoverageOperatorValue)4;

        Assert.That((int)sum, Is.EqualTo(7));
    }

    [Test]
    public void CoverageRecordConvertRaisesChangedAndReturnsIdScopedValue()
    {
        CoverageRecord record = new("record-id", "Record label");
        bool changed = false;
        record.Changed += (_, _) => changed = true;

        string result = record.Convert("source");

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo("record-id:source"));
            Assert.That(changed, Is.True);
        });
    }

    [Test]
    public void StaticGenericStructAndSecondaryFixturesKeepStableBehavior()
    {
        CoverageGenericClass<CoverageMarker> genericFixture = new();
        CoverageGenericCases<CoverageMarker, int, CoverageMarker> genericCases = new();
        CoverageSecondaryNamespaceCases secondaryFixture = new();

        Assert.Multiple(() =>
        {
            Assert.That(CoverageStaticClass.CreateLabel("value"), Is.EqualTo("label:value"));
            Assert.That(genericFixture.Create(), Is.TypeOf<CoverageMarker>());
            Assert.That(genericCases.PassThrough("text"), Is.EqualTo("text"));
            Assert.That(genericCases.UseUnmanaged(12), Is.EqualTo(12));
            Assert.That(genericCases.UseNullable<string>(null), Is.Null);
            Assert.That(new CoverageReadonlyStruct(5, "five").Format(), Is.EqualTo("5:five"));
            Assert.That(secondaryFixture.GetMarker(), Is.EqualTo("secondary"));
        });
    }
}