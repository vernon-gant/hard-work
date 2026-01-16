using System;
using Moq;
using NUnit.Framework;

namespace Import.Improved.Tests;

public class TitleIdSpecialAttributeValidatorTests
{
    private Mock<ISpecialAttributeValueRegistry> _registryMock;
    private TitleIdSpecialAttributeValidator _validator;

    [SetUp]
    public void Setup()
    {
        _registryMock = new Mock<ISpecialAttributeValueRegistry>();
        _validator = new TitleIdSpecialAttributeValidator(_registryMock.Object);
    }

    [Test]
    public void Validate_TitleIdSpecialAttribute_ValidValue_ShouldPass()
    {
        var mockAttribute = new Mock<TitleIdSpecialAttribute>(Guid.NewGuid(), "Category", "Approved");

        mockAttribute.Setup(a => a.ContainsValidTitles(_registryMock.Object)).Returns(true);

        var result = _validator.Validate(mockAttribute.Object);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_TitleIdSpecialAttribute_InvalidValue_ShouldFail()
    {
        var mockAttribute = new Mock<TitleIdSpecialAttribute>(Guid.NewGuid(), "Category", "Approved");

        mockAttribute.Setup(a => a.ContainsValidTitles(_registryMock.Object)).Returns(false);

        var result = _validator.Validate(mockAttribute.Object);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
    }
}