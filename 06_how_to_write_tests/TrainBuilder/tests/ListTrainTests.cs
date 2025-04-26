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

}