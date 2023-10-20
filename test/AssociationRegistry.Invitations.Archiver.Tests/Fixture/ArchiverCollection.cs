namespace AssociationRegistry.Invitations.Archiver.Tests.Fixture;

[CollectionDefinition(Name)]
public class ArchiverCollection : ICollectionFixture<ArchiverFixture>
{
    public const string Name = nameof(ArchiverCollection);

}
