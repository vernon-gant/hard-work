using System;
using System.Collections.Generic;
using System.Linq;

namespace Import.First;

public enum QueryStatus
{
    Success,
    NotFound,
    Duplicate
}

public class DictionaryAssetRegistry
{
    private readonly Dictionary<string, List<Guid>> _categoryToId;
    private readonly Dictionary<string, List<Guid>> _conditionToId;

    public DictionaryAssetRegistry(List<AssetCategory> categories, List<AssetCondition> conditions)
    {
        _categoryToId = categories.GroupBy(x => x.Title).ToDictionary(x => x.Key, x => x.Select(y => y.Id).ToList());
        _conditionToId = conditions.GroupBy(x => x.Title).ToDictionary(x => x.Key, x => x.Select(y => y.Id).ToList());
    }

    public QueryStatus TryGetCategory(string title, out Guid? categoryId)
    {
        if (_categoryToId.TryGetValue(title, out var ids))
        {
            if (ids.Count == 1)
            {
                categoryId = ids[0];
                return QueryStatus.Success;
            }

            categoryId = null;
            return QueryStatus.Duplicate;
        }

        categoryId = null;
        return QueryStatus.NotFound;
    }

    public QueryStatus TryGetCondition(string title, out Guid? conditionId)
    {
        if (_conditionToId.TryGetValue(title, out var ids))
        {
            if (ids.Count == 1)
            {
                conditionId = ids[0];
                return QueryStatus.Success;
            }

            conditionId = null;
            return QueryStatus.Duplicate;
        }

        conditionId = null;
        return QueryStatus.NotFound;
    }
}