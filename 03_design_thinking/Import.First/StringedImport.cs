using System.Collections.Generic;

namespace Import.First;

public record StringedImport(List<string> Headers, List<List<string>> Entries);