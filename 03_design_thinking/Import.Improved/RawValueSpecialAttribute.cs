using System;

namespace Import.Improved;

public abstract class SpecialAttribute(Guid attributeId, string title, string value)
{
    public Guid AttributeId { get; } = attributeId;

    public string Title { get; } = title;

    public string Value { get; } = value;
}

public abstract class ParsableValueSpecialAttribute(Guid attributeId, string title, string value) : SpecialAttribute(attributeId, title, value)
{
    public abstract bool IsParsable { get; }

    public abstract string MustHaveFormatMessage { get; }
}

public abstract class TitleIdSpecialAttribute(Guid attributeId, string title, string value) : SpecialAttribute(attributeId, title, value)
{
    public abstract bool ContainsValidTitles(ISpecialAttributeValueRegistry valueRegistry);
}