using System;
using Moq;
using NUnit.Framework;

namespace Import.Improved.Tests;

public class StringSpecialValueValidatorTests
{
    private StringSpecialValueValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new StringSpecialValueValidator();
    }

    [Test]
    public void Validate_ParsableSpecialAttribute_ValidInput_ShouldPass()
    {
        var mockAttribute = new Mock<ParsableValueSpecialAttribute>(Guid.NewGuid(), "Age", "42");
        mockAttribute.Setup(a => a.IsParsable).Returns(true);
        mockAttribute.Setup(a => a.MustHaveFormatMessage).Returns(string.Empty);

        var result = _validator.Validate(mockAttribute.Object);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_ParsableSpecialAttribute_InvalidInput_ShouldFail()
    {
        var mockAttribute = new Mock<ParsableValueSpecialAttribute>(Guid.NewGuid(), "Age", "abc");
        mockAttribute.Setup(a => a.IsParsable).Returns(false);
        mockAttribute.Setup(a => a.MustHaveFormatMessage).Returns(string.Empty);

        var result = _validator.Validate(mockAttribute.Object);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
    }
}