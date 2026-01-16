using System;
using System.Diagnostics;

namespace Import.Improved;

public class DropdownSpecialAttribute(Guid attributeId, string title, string value) : TitleIdSpecialAttribute(attributeId, title, value)
{
    public override bool ContainsValidTitles(ISpecialAttributeValueRegistry valueRegistry)
    {
        var validValues = valueRegistry.GetValidDropdownValues(AttributeId);
        Debug.Assert(validValues.IsT0);
        return validValues.AsT0.Contains(Value);
    }
}