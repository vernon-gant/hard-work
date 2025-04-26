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
}