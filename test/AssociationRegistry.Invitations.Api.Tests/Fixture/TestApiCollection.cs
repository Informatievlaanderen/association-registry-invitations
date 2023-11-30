namespace AssociationRegistry.Invitations.Api.Tests.Fixture;

[CollectionDefinition(Name)]
public class TestApiCollection : ICollectionFixture<TestApiFixture>
{
    public const string Name = nameof(TestApiCollection);

}
