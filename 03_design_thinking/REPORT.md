# Report

Initially I got a task to implement a small import module for our system at work. Of course I can not show the real code
because of the NDA, but I rewrote some parts to demonstrate the problem. Suppose we have a system with entities like assets which we
would like to import from any file. As assets we can have for example books, dvds, magazines or whatever. And each entity can have a set 
of special attributes. Each special attribute can be of different type like integer, float, currency, string, date which are parsable and
also those which are entered by titles and then mapped internally to ids. For example a dropdown list where we can create multiple values
but we do not want a customer to enter the ids so they enter them by titles.

When I just started of course I have not read the article about reasoning on 3 different levels. I even did not write any tests first.
And this was a big problem because I was more and more confusing the logical part and the code parts. Now I understand that I just need
to delete everything and rewrite the module completely but at the task begin I just started with the tests. No thoughts about the architecture
for now.

We treat not all modules I wrote but only several of them. Our assets also have fields like condition and category which are entered
by title in each entry and them we check if they exist so that we can map them to ids.

```c#
public class DictionaryAssetRegistryTests
{
    [Test]
    public void TryGetCategory_WithExistingCategory_ReturnsSuccessAndId()
    {
        // Arrange
        var categories = new List<AssetCategory>
        {
            new() { Id = Guid.NewGuid(), Title = "A", Description = "Category A" }
        };
        var registry = new DictionaryAssetRegistry(categories, []);

        // Act
        var result = registry.TryGetCategory("A", out var category);

        // Assert
        Assert.That(result, Is.EqualTo(QueryStatus.Success));
        Assert.That(category!, Is.EqualTo(categories[0].Id));
    }

    [Test]
    public void TryGetCategory_WithNonExistingCategory_ReturnsNotFoundAndNull()
    {
        // Arrange
        var categories = new List<AssetCategory>
        {
            new() { Id = Guid.NewGuid(), Title = "A", Description = "Category A" }
        };
        var registry = new DictionaryAssetRegistry(categories, []);

        // Act
        var result = registry.TryGetCategory("B", out var category);

        // Assert
        Assert.That(result, Is.EqualTo(QueryStatus.NotFound));
        Assert.That(category, Is.Null);
    }

    [Test]
    public void TryGetCategory_WithDuplicateCategory_ReturnsDuplicateAndNull()
    {
        // Arrange
        var categories = new List<AssetCategory>
        {
            new() { Id = Guid.NewGuid(), Title = "A", Description = "Category A" },
            new() { Id = Guid.NewGuid(), Title = "A", Description = "Category A" }
        };
        var registry = new DictionaryAssetRegistry(categories, []);

        // Act
        var result = registry.TryGetCategory("A", out var category);

        // Assert
        Assert.That(result, Is.EqualTo(QueryStatus.Duplicate));
        Assert.That(category, Is.Null);
    }

    [Test]
    public void TryGetCategory_WithEmptyRegistry_ReturnsNotFoundAndNull()
    {
        // Arrange
        var categories = new List<AssetCategory>();
        var registry = new DictionaryAssetRegistry(categories, []);

        // Act
        var result = registry.TryGetCategory("A", out var category);

        // Assert
        Assert.That(result, Is.EqualTo(QueryStatus.NotFound));
        Assert.That(category, Is.Null);
    }

    [Test]
    public void TryGetconditon_WithExistingconditon_ReturnsSuccessAndId()
    {
        // Arrange
        var conditons = new List<AssetCondition>
        {
            new() { Id = Guid.NewGuid(), Title = "A", Description = "conditon A" }
        };
        var registry = new DictionaryAssetRegistry([], conditons);

        // Act
        var result = registry.TryGetCondition("A", out var conditon);

        // Assert
        Assert.That(result, Is.EqualTo(QueryStatus.Success));
        Assert.That(conditon!, Is.EqualTo(conditons[0].Id));
    }

    [Test]
    public void TryGetconditon_WithNonExistingconditon_ReturnsNotFoundAndNull()
    {
        // Arrange
        var conditons = new List<AssetCondition>
        {
            new() { Id = Guid.NewGuid(), Title = "A", Description = "conditon A" }
        };
        var registry = new DictionaryAssetRegistry([], conditons);

        // Act
        var result = registry.TryGetCondition("B", out var conditon);

        // Assert
        Assert.That(result, Is.EqualTo(QueryStatus.NotFound));
        Assert.That(conditon, Is.Null);
    }

    [Test]
    public void TryGetconditon_WithDuplicateconditon_ReturnsDuplicateAndNull()
    {
        // Arrange
        var conditons = new List<AssetCondition>
        {
            new() { Id = Guid.NewGuid(), Title = "A", Description = "conditon A" },
            new() { Id = Guid.NewGuid(), Title = "A", Description = "conditon A" }
        };
        var registry = new DictionaryAssetRegistry([], conditons);

        // Act
        var result = registry.TryGetCondition("A", out var conditon);

        // Assert
        Assert.That(result, Is.EqualTo(QueryStatus.Duplicate));
        Assert.That(conditon, Is.Null);
    }

    [Test]
    public void TryGetconditon_WithEmptyRegistry_ReturnsNotFoundAndNull()
    {
        // Arrange
        var conditons = new List<AssetCondition>();
        var registry = new DictionaryAssetRegistry([], conditons);

        // Act
        var result = registry.TryGetCondition("A", out var conditon);

        // Assert
        Assert.That(result, Is.EqualTo(QueryStatus.NotFound));
        Assert.That(conditon, Is.Null);
    }
}
```

So I just came up that we from somewhere get the categories and conditions which exist in the system and then create an API to check for
existence of the category or condition. However already here I felt that I am just following the logic of the test and now thinking about the architecture of my solution.
I will talk about that later. Next tests were about parsing. Because we have different arts of assets the parsing is different. But I just wrote
tests for book parsing

```c#
public class BookImportTests
{
    [Test]
    public Task Parse_ValidBookImport_ReturnsParsedImport()
    {
        // Arrange
        var stringedImport = new StringedImport(new List<string> { "Title", "Category", "Condition", "Author", "ISBN", "Publisher", "YearPublished", "Price", "Rating" }, new List<List<string>>
            {
                new() { "The Hobbit", "Fantasy", "New", "J.R.R. Tolkien", "978-0547928227", "Houghton Mifflin", "1937", "15.99", "4.5" },
                new() { "1984", "Dystopian", "Used", "George Orwell", "978-0451524935", "Signet Classic", "1949", "8.50", "4.8" }
            });

        var bookImport = new BookImport(stringedImport);

        // Act
        var result = bookImport.Parse();

        // Assert
        Assert.That(result.IsT0, Is.True);
        var parsedImport = result.AsT0;
        Assert.That(parsedImport.ParsedEntries.Count, Is.EqualTo(2));

        return Verify(parsedImport);
    }

    [Test]
    public void Parse_InvalidPrice_ReportsError()
    {
        // Arrange
        var stringedImport = new StringedImport(new List<string> { "Title", "Category", "Condition", "Author", "ISBN", "Publisher", "YearPublished", "Price", "Rating" }, new List<List<string>>
            {
                new() { "The Hobbit", "Fantasy", "New", "J.R.R. Tolkien", "978-0547928227", "Houghton Mifflin", "1937", "15.99", "4.5" },
                new() { "1984", "Dystopian", "Used", "George Orwell", "978-0451524935", "Signet Classic", "1949", "8.50", "4.8" },
                new() { "The Great Gatsby", "Classic", "New", "F. Scott Fitzgerald", "978-0743273565", "Scribner", "1925", "Not a number", "4.5" }
            });

        var bookImport = new BookImport(stringedImport);

        // Act
        var result = bookImport.Parse();

        // Assert
        Assert.That(result.IsT1, Is.True);
        var errors = result.AsT1;
        Assert.That(errors.Count, Is.EqualTo(1));
        Assert.That(errors[0].Row, Is.EqualTo(4));
    }
}
```

The problem of the benefit is that I already had this code written and now I understood what this means when we adapt tests to the existing code. Now comes the interesting part.


## 3rd level of reasoning

Many things became clear after reading the 2 articles about reasoning on different levels. I literally understood my problem why it took me so long to develop this module and where the
whole confusion came from. I was not thinking about the core part of the architecture. What is our main idea or intent? What do we shape the architecture around?
How do we represent it in code? After some thinking it became clear that the main intent of the import is to take the data, validate it on different levels like
parsing, checking for existence of categories and conditions, special attributes and so on and then save. However not only the error checking is that important but also
reporting the errors to the user. It does not bring us a lot if we say that import has invalid data or we say that some data is missing. We must
more less precisely say what is on which row is wrong. What could not be parsed and for what reason. This is our main idea and we want all our components to be
as much as possible responsive. So I though that we can build our system around the idea of properties. That each entity at some points has a set of properties which
we can then use to validate and build the error message. For example if we take a header - header consists of an asset header part and special attribute titles.
Asset header must be equal to the required one say by the book entity. Any special attribute title must be present in the system. Its values must be present as well.
So on the third level we build our main validation and parsing and parsing architecture around properties and validating them. I started with validating the header.
So now our tests must just represent concrete cases of the header validation based on properties.

```c#
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
        _customFieldRegistryMock.Setup(x => x.Exists(context.SpecialAttributeTitles[0])).Returns(true);
        _customFieldRegistryMock.Setup(x => x.Exists(context.SpecialAttributeTitles[1])).Returns(false);
        _customFieldRegistryMock.Setup(x => x.Exists(context.SpecialAttributeTitles[2])).Returns(false);

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
        _customFieldRegistryMock.Setup(x => x.Exists(context.SpecialAttributeTitles[0])).Returns(true);
        _customFieldRegistryMock.Setup(x => x.Exists(context.SpecialAttributeTitles[1])).Returns(false);
        _customFieldRegistryMock.Setup(x => x.Exists(context.SpecialAttributeTitles[2])).Returns(false);

        var validator = new HeaderValidator(_customFieldRegistryMock.Object);

        // Act
        var result = validator.Validate(context);

        // Assert
        Assert.That(!result.IsValid);
        Assert.That(result.Errors, Has.Count.EqualTo(2));
    }
}
```

We do not assert the error messages themselves because they can change but we are interested in the count of errors. So now we have a clear idea of what we want to achieve.
The rest is to put in code our intent. For this we have a perfect FluentValidation library which allows us to declarative define what we want to achieve and then also specify
the error message. That's what we want : property -> validation -> error message. And we can do it in a declarative way.

```c#
public class HeaderValidator : AbstractValidator<HeaderValidationContext>
{
    public HeaderValidator(ISpecialAttributeRegistry registry)
    {
        RuleFor(x => x)
            .Must(x => x.AssetHeader.SequenceEqual(x.RequiredAssetHeader))
            .WithMessage("Asset header does not match required asset header");

        RuleFor(x => x.SpecialAttributeTitles)
            .Must(x => x.All(registry.Exists))
            .WithMessage((_, list) => $"Following special attributes do not exist: {string.Join(", ", list.Where(x => !registry.Exists(x)))}");
    }
}
```

We still want to keep the header not tightly coupled to only book or whatever so we just pass to the validation context required and given headers together with
special attributes list. We also want the same approach for validation of the special attributes. We want to validate them based on their type. And the validation here
will differ for parsable and non-parsable attributes. For example for parsable we want to check if the value can be parsed to the given type. For non-parsable we pass a title
which must correlate to a special attribute id. So we again define tests for the 2 validators we want to have together with different special attribute.

```c#
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
```

Maybe a bit short but our test follow the idea that for each parsable special attribute we want to check if the value can be parsed. For non-parsable we want to check if the given
title or titles is present in the system. So we define the validator for the non-parsable special attributes.

```c#
public class TitleIdSpecialAttributeValidatorTests
{
    private Mock<ISpecialAttributeValueRegistry> _registryMock;
    private TitleIdSpecialAttributeValidator _validator;

    [SetUp]
    public void Setup()
    {
        _registryMock = new Mock<ISpecialAttributeValueRegistry>();
        _validator = new TitleIdSpecialAttributeValidator(_registryMock.Object);
    }

    [Test]
    public void Validate_TitleIdSpecialAttribute_ValidValue_ShouldPass()
    {
        var mockAttribute = new Mock<TitleIdSpecialAttribute>(Guid.NewGuid(), "Category", "Approved");

        mockAttribute.Setup(a => a.ContainsValidTitles(_registryMock.Object)).Returns(true);

        var result = _validator.Validate(mockAttribute.Object);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_TitleIdSpecialAttribute_InvalidValue_ShouldFail()
    {
        var mockAttribute = new Mock<TitleIdSpecialAttribute>(Guid.NewGuid(), "Category", "Approved");

        mockAttribute.Setup(a => a.ContainsValidTitles(_registryMock.Object)).Returns(false);

        var result = _validator.Validate(mockAttribute.Object);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
    }
}
```

At this points we do not care which special attribute we validate. We just pass the special attribute to the validator and then we check if the value can be validated.
After all we can implement our special attributes and validators.

```c#
public class MultiSelectSpecialAttribute(Guid attributeId, string title, string value) : TitleIdSpecialAttribute(attributeId, title, value)
{
    public override bool ContainsValidTitles(ISpecialAttributeValueRegistry valueRegistry)
    {
        var validValues = valueRegistry.GetValidMultiSelectValues(AttributeId);
        Debug.Assert(validValues.IsT0);
        var values = Value.Split(',');
        return values.All(validValues.AsT0.Contains);
    }
}
```

Just as an example. Must also say that discriminated unions with the help of OneOf really help during testing as we can easily assert that the outcome of some operation is of specific type
and not just null or whatever. And no try catches!


## Conclusion

Now I started to understand what thinking on the third level means. We do not think about the code and as it was said we think it terms of abstraction
and logical building of the system. TDD pulls us a bit back to the second level as we must start think about dependencies and so on. But the true thinking
must still start on the third level where we use our ADTs to represent logical parts of the system. But even without ADT we can start thinking on the higher
level say how the import process works in general without thinking about the code. But it takes time to learn...