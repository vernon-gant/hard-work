public abstract class CustomField
{
    public required string Title { get; init; }

    public required string Value { get; init; }

    public abstract string ToEventValue();
}

public class TextCustomField : CustomField
{
    public override string ToEventValue => Value;
}

public class MultiSelectCustomField : CustomField
{
    public override string ToEventValue => $"[{Value}]"
}


public class CustomFieldHandler
{
    // This is a covariant call. Covariance as already mentioned works in C# only for return types(that's why arrays are covariant per default)
    // and only for interfaces and delegates. Here for example we could pass an array of TextCustomFields or any other. If we would have here instead an interface
    // say ImportParser<out T> where T : CustomField and we would pass as parameter ImportParser<CustomField> we could as argument also pass ImportParser<TextCustomField> thanks to covariance
    public void ProcessFields<T>(T[] customFields) where T : CustomField
    {
        foreach(field : customFields)
        {
            ...
        }
    }

    // Normal polymorphic method where the actual type of customField will be deduced in runtime
    // and we can operate on the base class reference and thans to dynamic binding this will work.
    // so as arguemtn we could pass handler.PersistField(new MultiSelectCustomField{...}); 
    public void PersistField(CustomField customField)
    {
        string eventValue = customField.ToEventValue();
        ...
    }
}