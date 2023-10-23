using Marten;
using Marten.Schema;

namespace AssociationRegistry.Invitations.Archiver.Tests.Fixture;

public class UitnodigingTestData : IInitialData
{
    private readonly UitnodigingTestDataFactory _factory;
    public Dictionary<UitnodigingsStatus, IReadOnlyCollection<Uitnodiging>> Collection = new();

    public UitnodigingTestData(UitnodigingTestDataFactory testDataFactory)
    {
        _factory = testDataFactory;
    }

    public async Task Populate(IDocumentStore store, CancellationToken cancellation)
    {
        await using var session = store.LightweightSession();
        session.StoreObjects(_factory.Build());
        await session.SaveChangesAsync(cancellation);
    }
}