using System;

namespace Import.Improved;

public class IntegerSpecialAttribute(Guid attributeId, string title, string value) : ParsableValueSpecialAttribute(attributeId, title, value)
{
    public override bool IsParsable => int.TryParse(Value, out _);
    public override string MustHaveFormatMessage => "Value must be an integer like '123'.";
}