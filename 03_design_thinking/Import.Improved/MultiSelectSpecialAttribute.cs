using System;
using System.Diagnostics;
using System.Linq;

namespace Import.Improved;

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