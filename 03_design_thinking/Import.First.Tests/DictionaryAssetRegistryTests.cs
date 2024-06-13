using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Import.First.Tests;

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