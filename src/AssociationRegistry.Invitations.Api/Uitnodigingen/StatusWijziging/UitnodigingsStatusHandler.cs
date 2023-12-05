using Marten;
using NodaTime;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.StatusWijziging;

public class UitnodigingsStatusHandler
{
    private readonly IClock _clock;
    private readonly IDocumentStore _store;

    public UitnodigingsStatusHandler(IClock clock, IDocumentStore store)
    {
        _clock = clock;
        _store = store;
    }

    public async Task SetStatus(
        Uitnodiging uitnodiging,
        UitnodigingsStatus status,
        Invitations.Validator? validator,
        CancellationToken cancellationToken)
    {
        await using var session = _store.LightweightSession();
        uitnodiging.Status = status;

        uitnodiging.Validator = validator;

        uitnodiging.DatumLaatsteAanpassing = _clock.GetCurrentInstant().ToDateTimeOffset();

        session.Store(uitnodiging);
        await session.SaveChangesAsync(cancellationToken);
    }
}
