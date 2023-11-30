namespace AssociationRegistry.Invitations.Archiver.Tests.Uitnodigingen;

using Marten;
using Marten.Schema;

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

public record Uitnodigingen
{
    public Uitnodiging WachtOpAntwoord { get; set; }
    public Uitnodiging Aanvaard { get; set; }
    public Uitnodiging Geweigerd { get; set; }
    public Uitnodiging Ingetrokken { get; set; }
    public Uitnodiging Verlopen { get; set; }
}