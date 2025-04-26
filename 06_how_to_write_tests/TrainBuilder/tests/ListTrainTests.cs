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
}