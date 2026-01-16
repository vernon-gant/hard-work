// By default inherits from Object. General approach for most methods is string serialization
// for example it can help us to create a clone of current object with just 2 method
// calls
public abstract class General
{
    public virtual void CopyTo(General target)
    {
        string serialized = Serialize();
        General deserialized = Deserialize(serialized);
        target.CopyFrom(deserialized);
    }

    public virtual General Clone()
    {
        return Deserialize(Serialize());
    }

    public virtual bool DeepEquals(General other)
    {
        return Serialize() == other.Serialize();
    }

    public virtual string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }

    public virtual General Deserialize(string serialized)
    {
        return (General)JsonSerializer.Deserialize(serialized, this.GetType());
    }

    public virtual void Print()
    {
        Console.WriteLine(ToString());
    }

    public virtual bool IsTypeOf(Type type)
    {
        return this.GetType() == type;
    }

    public virtual Type GetTypeReal()
    {
        return this.GetType();
    }

    protected virtual void CopyFrom(General other)
    {
        string serialized = other.Serialize();
        General temp = Deserialize(serialized);
        PropertyInfo[] properties = this.GetType().GetProperties();

        foreach (var property in properties)
        {
            if (property.CanWrite)
            {
                property.SetValue(this, property.GetValue(temp));
            }
        }
    }
}


public class MyAny : General
{
    // This class does not add new functionality but is open for future extension
}