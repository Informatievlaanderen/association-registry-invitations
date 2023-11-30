namespace AssociationRegistry.Invitations.Archiver.Tests.Aanvragen;

using Marten;
using Marten.Schema;

public class AanvraagTestData : IInitialData
{
    private readonly AanvraagTestDataFactory _factory;
    public Dictionary<AanvraagStatus, IReadOnlyCollection<Aanvraag>> Collection = new();

    public AanvraagTestData(AanvraagTestDataFactory testDataFactory)
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

public record Aanvragen
{
    public Aanvraag WachtOpAntwoord { get; set; }
    public Aanvraag Aanvaard { get; set; }
    public Aanvraag Geweigerd { get; set; }
    public Aanvraag Ingetrokken { get; set; }
    public Aanvraag Verlopen { get; set; }
}