using FluentValidation;
using NSubstitute;

namespace TrainBuilder.tests;

public class ListTrainTests
{
    private IValidator<InsertionContext> _validator;
    private ITrain _train;

    [SetUp]
    public void Setup()
    {
        _validator = Substitute.For<IValidator<InsertionContext>>();
        _train = new ListTrain(_validator);
    }

    [Test]
    public void GivenEmptyTrain_WhenInsertingCarriage_ShouldContainExactlyOneCarriage()
    {
        var carriage = Substitute.For<ICarriage>();
        _validator.Validate(Arg.Any<InsertionContext>()).Returns(new FluentValidation.Results.ValidationResult());

        var result = _train.InsertCarriage(carriage, 0);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsT0);
            Assert.That(_train.Count, Is.EqualTo(1));
        });

        Assert.Multiple(() =>
        {
            var carriagesResult = _train.GetFromIdxInclusive(0);
            Assert.That(carriagesResult.IsT0);
            var carriages = carriagesResult.AsT0;
            Assert.That(carriages, Has.Count.EqualTo(1));
            Assert.That(carriages[0], Is.EqualTo(carriage));
        });
    }

    [Test]
    public void GivenNonEmptyTrain_WhenInsertingAtStart_ShouldShiftExistingCarriagesRight()
    {
        var existingCarriage1 = Substitute.For<ICarriage>();
        var existingCarriage2 = Substitute.For<ICarriage>();
        var newCarriage = Substitute.For<ICarriage>();
        _validator.Validate(Arg.Any<InsertionContext>()).Returns(new FluentValidation.Results.ValidationResult());
        _train.InsertCarriage(existingCarriage1, 0);
        _train.InsertCarriage(existingCarriage2, 1);

        var insertResult = _train.InsertCarriage(newCarriage, 0);

        Assert.Multiple(() =>
        {
            Assert.That(insertResult.IsT0);
            Assert.That(_train.Count, Is.EqualTo(3));
        });
        Assert.Multiple(() =>
        {
            var carriagesResult = _train.GetFromIdxInclusive(0);
            Assert.That(carriagesResult.IsT0);
            var carriages = carriagesResult.AsT0;
            Assert.That(carriages, Has.Count.EqualTo(3));
            Assert.That(carriages[0], Is.EqualTo(newCarriage));
            Assert.That(carriages[1], Is.EqualTo(existingCarriage1));
            Assert.That(carriages[2], Is.EqualTo(existingCarriage2));
        });
    }

    [Test]
    public void GivenNonEmptyTrain_WhenInsertingAtLastIndex_ShouldAppendNewCarriage()
    {
        var existingCarriage1 = Substitute.For<ICarriage>();
        var existingCarriage2 = Substitute.For<ICarriage>();
        var newCarriage = Substitute.For<ICarriage>();
        _validator.Validate(Arg.Any<InsertionContext>()).Returns(new FluentValidation.Results.ValidationResult());
        _train.InsertCarriage(existingCarriage1, 0);
        _train.InsertCarriage(existingCarriage2, 1);


        var insertResult = _train.InsertCarriage(newCarriage, 2);

        Assert.Multiple(() =>
        {
            Assert.That(insertResult.IsT0);
            Assert.That(_train.Count, Is.EqualTo(3));
        });
        Assert.Multiple(() =>
        {
            var carriagesResult = _train.GetFromIdxInclusive(0);
            Assert.That(carriagesResult.IsT0);
            var carriages = carriagesResult.AsT0;
            Assert.That(carriages, Has.Count.EqualTo(3));
            Assert.That(carriages[0], Is.EqualTo(existingCarriage1));
            Assert.That(carriages[1], Is.EqualTo(existingCarriage2));
            Assert.That(carriages[2], Is.EqualTo(newCarriage));
        });
    }

    [Test]
    public void GivenNonEmptyTrain_WhenInsertingAtMiddleIndex_ShouldInsertCorrectlyAndShiftOthers()
    {
        var carriage1 = Substitute.For<ICarriage>();
        var carriage2 = Substitute.For<ICarriage>();
        var carriage3 = Substitute.For<ICarriage>();
        var newCarriage = Substitute.For<ICarriage>();
        _validator.Validate(Arg.Any<InsertionContext>()).Returns(new FluentValidation.Results.ValidationResult());
        _train.InsertCarriage(carriage1, 0);
        _train.InsertCarriage(carriage2, 1);
        _train.InsertCarriage(carriage3, 2);


        var insertResult = _train.InsertCarriage(newCarriage, 1);

        Assert.Multiple(() =>
        {
            Assert.That(insertResult.IsT0);
            Assert.That(_train.Count, Is.EqualTo(4));
        });
        Assert.Multiple(() =>
        {
            var carriagesResult = _train.GetFromIdxInclusive(0);
            Assert.That(carriagesResult.IsT0);
            var carriages = carriagesResult.AsT0;
            Assert.That(carriages, Has.Count.EqualTo(4));
            Assert.That(carriages[0], Is.EqualTo(carriage1));
            Assert.That(carriages[1], Is.EqualTo(newCarriage));
            Assert.That(carriages[2], Is.EqualTo(carriage2));
            Assert.That(carriages[3], Is.EqualTo(carriage3));
        });
    }

    [Test]
    public void GivenInvalidInsertion_WhenValidatorFails_ShouldReturnErrorWithMessage()
    {
        var carriage = Substitute.For<ICarriage>();

        _validator.Validate(Arg.Any<InsertionContext>())
            .Returns(new FluentValidation.Results.ValidationResult([
                new FluentValidation.Results.ValidationFailure("idx", "Index is invalid")
            ]));

        var result = _train.InsertCarriage(carriage, -100);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsT1);
            Assert.That(result.AsT1.Value, Is.EqualTo("Index is invalid"));
            Assert.That(_train.Count, Is.EqualTo(0));
        });
    }
}