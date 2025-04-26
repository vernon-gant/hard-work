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

    [SetUp]
    public void Setup()
    {
        _validator = new InsertionValidator();
        _train = Substitute.For<ITrain>();
        _carriage = Substitute.For<ICarriage>();
    }

    [Test]
    public void GivenFullTrain_ThenValidationFails()
    {
        _train.Count.Returns(MaxTrainSize);
        var context = new InsertionContext(_train, 5, _carriage);

        var result = _validator.Validate(context);

        AssertValidationFailed(result);
    }

    [Test]
    public void GivenNegativeIndex_ThenValidationFails()
    {
        _train.Count.Returns(DefaultTrainSize);
        var context = new InsertionContext(_train, -1, _carriage);

        var result = _validator.Validate(context);

        AssertValidationFailed(result);
    }

    [Test]
    public void GivenEmptyTrain_WhenInsertingAtIndexGreaterThanZero_ThenValidationFails()
    {
        _train.Count.Returns(0);
        var context = new InsertionContext(_train, 1, _carriage);

        var result = _validator.Validate(context);

        AssertValidationFailed(result);
    }

    [Test]
    public void GivenNonEmptyTrain_WhenInsertingAtIndexGreaterThanCount_ThenValidationFails()
    {
        _train.Count.Returns(DefaultTrainSize);
        var context = new InsertionContext(_train, DefaultTrainSize + 1, _carriage);

        var result = _validator.Validate(context);

        AssertValidationFailed(result);
    }

    [Test]
    public void GivenEmptyTrain_WhenInsertingAtIndexZero_ThenValidationSucceeds()
    {
        _train.Count.Returns(0);
        var context = new InsertionContext(_train, 0, _carriage);

        var result = _validator.Validate(context);

        Assert.That(result.IsValid);
    }

    [Test]
    public void GivenNonEmptyTrain_WhenInsertingAtIndexZero_ThenValidationSucceeds()
    {
        _train.Count.Returns(DefaultTrainSize);
        var context = new InsertionContext(_train, 0, _carriage);

        var result = _validator.Validate(context);

        Assert.That(result.IsValid);
    }

    [Test]
    public void GivenNonFullTrain_WhenInsertingAtLastValidIndex_ThenValidationSucceeds()
    {
        _train.Count.Returns(DefaultTrainSize);
        var context = new InsertionContext(_train, DefaultTrainSize, _carriage);

        var result = _validator.Validate(context);

        Assert.That(result.IsValid);
    }

    [Test]
    public void GivenNonFullTrain_WhenInsertingAtMiddleIndex_ThenValidationSucceeds()
    {
        _train.Count.Returns(DefaultTrainSize);
        var context = new InsertionContext(_train, DefaultTrainSize / 2, _carriage);

        var result = _validator.Validate(context);

        Assert.That(result.IsValid);
    }

    [Test]
    public void GivenTrainWithNineCarriages_WhenInsertingTenthCarriage_ThenValidationSucceeds()
    {
        _train.Count.Returns(MaxTrainSize - 1);
        var context = new InsertionContext(_train, MaxTrainSize - 1, _carriage);

        var result = _validator.Validate(context);

        Assert.That(result.IsValid);
    }

    [Test]
    public void GivenSleeperCarriage_WhenNoTwoPassengerCarriagesAfterInsertion_ThenValidationFails()
    {
        _train.Count.Returns(DefaultTrainSize);
        const int insertionIdx = 0;
        _train.GetFromIdxInclusive(insertionIdx).Returns(OneOf<List<ICarriage>, IdxOutOfRange>.FromT0([new DinnerCarriage(), new DinnerCarriage()]));
        _carriage = new SleeperCarriage();
        var context = new InsertionContext(_train, insertionIdx, _carriage);

        var result = _validator.Validate(context);

        AssertValidationFailed(result);
    }

    [Test]
    public void GivenSleeperCarriage_WhenInsertingAtEndOfTrain_ThenValidationFails()
    {
        _train.Count.Returns(DefaultTrainSize);
        const int insertionIdx = DefaultTrainSize;
        _train.GetFromIdxInclusive(DefaultTrainSize).Returns(OneOf<List<ICarriage>, IdxOutOfRange>.FromT0([]));
        _carriage = new SleeperCarriage();
        var context = new InsertionContext(_train, insertionIdx, _carriage);

        var result = _validator.Validate(context);

        AssertValidationFailed(result);
    }

    [Test]
    public void GivenSleeperCarriage_WhenOnlyOnePassengerCarriageAfterInsertion_ThenValidationFails()
    {
        _train.Count.Returns(DefaultTrainSize);
        const int insertionIdx = 3;
        _train.GetFromIdxInclusive(insertionIdx).Returns(OneOf<List<ICarriage>, IdxOutOfRange>.FromT0([new PassengerCarriage(), new DinnerCarriage()]));
        _carriage = new SleeperCarriage();
        var context = new InsertionContext(_train, insertionIdx, _carriage);

        var result = _validator.Validate(context);

        AssertValidationFailed(result);
    }

    [Test]
    public void GivenSleeperCarriage_WhenTwoPassengerCarriagesAfterInsertion_ThenValidationSucceeds()
    {
        _train.Count.Returns(DefaultTrainSize);
        const int insertionIdx = 2;
        _train.GetFromIdxInclusive(insertionIdx).Returns(OneOf<List<ICarriage>, IdxOutOfRange>.FromT0([new PassengerCarriage(), new PassengerCarriage(), new PassengerCarriage()]));
        _carriage = new SleeperCarriage();
        var context = new InsertionContext(_train, insertionIdx, _carriage);

        var result = _validator.Validate(context);

        Assert.That(result.IsValid);
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