using Moq;
using NUnit.Framework;

namespace Import.Improved.Tests;
public class HeaderValidatorTests
{
    private Mock<ISpecialAttributeRegistry> _customFieldRegistryMock;

    [SetUp]
    public void Setup()
    {
        _customFieldRegistryMock = new Mock<ISpecialAttributeRegistry>();
    }

    [Test]
    public void Validate_WhenAssetHeaderMatchesAndAllSpecialAttributesExist_ReturnsValid()
    {
        // Arrange
        var context = new HeaderValidationContext
                      {
                          AssetHeader = ["Description", "Title"],
                          RequiredAssetHeader = ["Description", "Title"],
                          SpecialAttributeTitles = ["SpecialAttribute1", "SpecialAttribute2"]
                      };
        _customFieldRegistryMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);

        var validator = new HeaderValidator(_customFieldRegistryMock.Object);

        // Act
        var result = validator.Validate(context);

        // Assert
        Assert.That(result.IsValid);
    }

    [Test]
    public void Validate_WhenAssetHeaderDoesNotMatchAndAllSpecialAttributesExist_ReturnsHeaderMismatchErrors()
    {
        // Arrange
        var context = new HeaderValidationContext
        {
            AssetHeader = ["Description", "Title"],
            RequiredAssetHeader = ["Asset", "Due Date"],
            SpecialAttributeTitles = ["SpecialAttribute1", "SpecialAttribute2"]
        };
        _customFieldRegistryMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);

        var validator = new HeaderValidator(_customFieldRegistryMock.Object);

        // Act
        var result = validator.Validate(context);

        // Assert
        Assert.That(!result.IsValid);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
    }

    [Test]
    public void Validate_WhenAssetHeaderMatchesAndNotAllSpecialAttributesExist_ReturnsMissingSpecialAttributeErrors()
    {
        // Arrange
        var context = new HeaderValidationContext
        {
            AssetHeader = ["Description", "Title"],
            RequiredAssetHeader = ["Description", "Title"],
            SpecialAttributeTitles = ["SpecialAttribute1", "SpecialAttribute2", "SpecialAttribute3"]
        };
        _customFieldRegistryMock.Setup<bool>(x => x.Exists(context.SpecialAttributeTitles[0])).Returns(true);
        _customFieldRegistryMock.Setup<bool>(x => x.Exists(context.SpecialAttributeTitles[1])).Returns(false);
        _customFieldRegistryMock.Setup<bool>(x => x.Exists(context.SpecialAttributeTitles[2])).Returns(false);

        var validator = new HeaderValidator(_customFieldRegistryMock.Object);

        // Act
        var result = validator.Validate(context);

        // Assert
        Assert.That(!result.IsValid);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
    }

    [Test]
    public void Validate_WhenAssetHeaderDoesNotMatchAndNotAllSpecialAttributesExist_ReturnsHeaderMismatchAndMissingSpecialAttributeErrors()
    {
        // Arrange
        var context = new HeaderValidationContext
        {
            AssetHeader = ["Description", "Title"],
            RequiredAssetHeader = ["Asset", "Due Date"],
            SpecialAttributeTitles = ["SpecialAttribute1", "SpecialAttribute2", "SpecialAttribute3"]
        };
        _customFieldRegistryMock.Setup<bool>(x => x.Exists(context.SpecialAttributeTitles[0])).Returns(true);
        _customFieldRegistryMock.Setup<bool>(x => x.Exists(context.SpecialAttributeTitles[1])).Returns(false);
        _customFieldRegistryMock.Setup<bool>(x => x.Exists(context.SpecialAttributeTitles[2])).Returns(false);

        var validator = new HeaderValidator(_customFieldRegistryMock.Object);

        // Act
        var result = validator.Validate(context);

        // Assert
        Assert.That(!result.IsValid);
        Assert.That(result.Errors, Has.Count.EqualTo(2));
    }
}