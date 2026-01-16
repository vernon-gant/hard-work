using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;

namespace Import.First.Tests;

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

        return Verifier.Verify(parsedImport);
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