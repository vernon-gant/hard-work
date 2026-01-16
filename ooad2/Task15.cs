// Not so long ago faced this situation. Let's imagine that we have an enity custom field which has a type like "drpopdown" or "checkbox"
// and depending on the type we do different parsing and serialization. In our project this was done exactly by using an enum

public enum CustomFieldType
{
    Text,
    MultilineText,
    ...
}

// with subsequent switch for logic. But when I was implementing import functionality which included import of custom fields I quickly realised that I can not really reuse
// existing code and need to make some small adjustment - but I did not want to copy switch and change some internals there.

// There are actually 2 things which are inherently different for custom fields of different types - how they are serialized into value which is then stored in the database and
// how they are parsed. And for import I also needed a custom error detail when parsing went wrong like "must be of format ..." so I got following ADT

public abstract class CustomFieldType(string value)
{
    bool IsParsable();

    string ToEventValue();

    string ParsingErrorDetil();
}

// and for example a datetime type

public class DateTimeCustomFieldType(string value) : CustomFieldType(value)
{
    public override bool IsParsable => DateTime.TryParse(value);

    public override string ToEventValue()
    {
        if (!IsParsable)
            throw new InvalidOperationException(); // I know that better would be status codes - but just for example

        return DateTime.Parse(value).ToString(some format);
    }

    public override string ParsingErrorDetail => "DateTime custom field type must have format ...";
}

// A very elegant way of encapsulating many possible enum values into types!