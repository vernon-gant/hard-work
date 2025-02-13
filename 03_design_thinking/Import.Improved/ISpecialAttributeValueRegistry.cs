using System;
using System.Collections.Generic;
using OneOf;
using OneOf.Types;

namespace Import.Improved;

public interface ISpecialAttributeValueRegistry
{
    OneOf<List<string>, NotFound> GetValidDropdownValues(Guid attributeId);

    OneOf<List<string>, NotFound> GetValidMultiSelectValues(Guid attributeId);
}