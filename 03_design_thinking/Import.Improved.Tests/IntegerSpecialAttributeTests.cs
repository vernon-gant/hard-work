using System;
using NUnit.Framework;

namespace Import.Improved.Tests;

public class IntegerSpecialAttributeTests
{
    [Test]
    public void IsParsable_PositiveInteger_ReturnsTrue()
    {
        var attribute = new IntegerSpecialAttribute(Guid.NewGuid(), "Age", "42");

        Assert.That(attribute.IsParsable, Is.True);
    }

    [Test]
    public void IsParsable_NegativeInteger_ReturnsTrue()
    {
        var attribute = new IntegerSpecialAttribute(Guid.NewGuid(), "Age", "-42");

        Assert.That(attribute.IsParsable, Is.True);
    }


    [Test]
    public void IsParsable_InvalidInteger_ReturnsFalse()
    {
        var attribute = new IntegerSpecialAttribute(Guid.NewGuid(), "Age", "abc");

        Assert.That(attribute.IsParsable, Is.False);
    }
}