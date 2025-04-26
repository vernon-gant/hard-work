using FluentValidation.Results;
using NSubstitute;
using OneOf;

namespace TrainBuilder.tests;

public class InsertionValidatorTests
{
    private InsertionValidator _validator;
    private ITrain _train;
    private ICarriage _carriage;

    private const int DefaultTrainSize = 5;
    private const int MaxTrainSize = 10;

    [OneTimeSetUp]
    public void Setup()
    {
        _validator = new InsertionValidator();
        _train = Substitute.For<ITrain>();
        _carriage = Substitute.For<ICarriage>();
    }

    [Test]
    public void WhenInsertionIdxIsNegative_MustReturnFalse()
    {
        _train.Count.Returns(DefaultTrainSize);
        var context = new InsertionContext(_train, -1, _carriage);

        var result = _validator.Validate(context);

        AssertValidationFailed(result);
    }

    [Test]
    public void WhenInsertionIdxIsOutOfRange_MustReturnFalse()
    {
        _train.Count.Returns(DefaultTrainSize);
        var context = new InsertionContext(_train, DefaultTrainSize, _carriage);

        var result = _validator.Validate(context);

        AssertValidationFailed(result);
    }

    [Test]
    public void WhenInsertionIdxIsValid_MustReturnTrue()
    {
        _train.Count.Returns(DefaultTrainSize);
        var context = new InsertionContext(_train, DefaultTrainSize - 2, _carriage);

        var result = _validator.Validate(context);

        Assert.That(result.IsValid);
    }

    [Test]
    public void WhenTrainIsFull_MustReturnFalse()
    {
        _train.Count.Returns(MaxTrainSize);
        var context = new InsertionContext(_train, 0, _carriage);

        var result = _validator.Validate(context);

        AssertValidationFailed(result);
    }

    [Test]
    public void WhenSleeperRuleIsViolated_MustReturnFalse()
    {
        _train.Count.Returns(DefaultTrainSize);
        const int insertionIdx = 0;
        _train.GetFromIdxInclusive(insertionIdx).Returns(OneOf<List<ICarriage>, IdxOutOfRange>.FromT0([new DinnerCarriage(20), new DinnerCarriage(20)]));
        _carriage = new SleeperCarriage(20);
        var context = new InsertionContext(_train, insertionIdx, _carriage);

        var result = _validator.Validate(context);

        AssertValidationFailed(result);
    }

    private static void AssertValidationFailed(ValidationResult result)
    {
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Count.EqualTo(1));
        });
    }
}