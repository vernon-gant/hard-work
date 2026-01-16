using System.Collections.Generic;

namespace Import.First;

public class ImportRowError
{
    public required long Row { get; init; }

    public List<string> ErrorMessages { get; init; } = new();
}