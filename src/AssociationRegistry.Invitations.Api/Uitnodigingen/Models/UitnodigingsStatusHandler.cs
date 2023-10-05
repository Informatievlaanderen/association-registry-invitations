using System.Globalization;
using Marten;
using NodaTime;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Models;

public class UitnodigingsStatusHandler
{
    private readonly IClock _clock;
    private readonly IDocumentSession _session;

    public UitnodigingsStatusHandler(IClock clock, IDocumentSession session)
    {
        _clock = clock;
        _session = session;
    }
    
    public async Task SetStatus(Uitnodiging uitnodiging, UitnodigingsStatus status, CancellationToken cancellationToken)
    {
        uitnodiging.Status = status;
        uitnodiging.DatumLaatsteAanpassing = _clock.GetCurrentInstant().ToString("g", CultureInfo.InvariantCulture);
        
        _session.Store(uitnodiging);
        await _session.SaveChangesAsync(cancellationToken);
    }
}