using NSubstitute;

namespace TrainBuilder.tests;

public class InsertionValidatorTests
{
    private InsertionValidator _validator;
    private ITrain _train;
    private ICarriage _carriage;

    private const int DefaultTrainSize = 5;

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

        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Count.EqualTo(1));
        });
    }
}