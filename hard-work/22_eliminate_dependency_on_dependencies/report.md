# 2. Dependency on the Shared Format

Authorities post every `n` months an XML containing
entities such as `Skills`, `SkillToWorkEnvironments` or `SkillToSubskills`. At the beginning they were posting just XML files and we had a bunch of DTOs with XMLElement attributes, which had to be adjusted. After 3 times it was proposed that they post together with XML files also XSD definitions. The idea was that every time if something changed in the XSD, we just generate new models using the `dotnet-xscgen` and then implement the visitor for the types we 