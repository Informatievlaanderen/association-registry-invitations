namespace AssociationRegistry.Invitations.Api.Aanvragen.StatusWijziging;

using Marten;
using NodaTime;

public class AanvraagStatusHandler
{
    private readonly IClock _clock;
    private readonly IDocumentStore _store;

    public AanvraagStatusHandler(IClock clock, IDocumentStore store)
    {
        _clock = clock;
        _store = store;
    }

    public async Task SetStatus(
        Aanvraag aanvraag,
        AanvraagStatus status,
        Invitations.Validator? validator,
        CancellationToken cancellationToken)
    {
        await using var session = _store.LightweightSession();
        aanvraag.Status = status;

        aanvraag.Validator = validator;

        aanvraag.DatumLaatsteAanpassing = _clock.GetCurrentInstant().ToDateTimeOffset();

        session.Store(aanvraag);
        await session.SaveChangesAsync(cancellationToken);
    }
}
