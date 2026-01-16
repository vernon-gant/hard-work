using OneOf;
using OneOf.Types;

namespace Import.Improved;

public interface ISpecialAttributeRegistry
{
    bool Exists(string title);
}