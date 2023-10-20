using AssociationRegistry.Invitations.Api.Infrastructure.Extensions;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using AssociationRegistry.Invitations.Archiver;
using Marten;
using Microsoft.Extensions.Hosting;
using NodaTime;

public class Archival : BackgroundService
{
    private readonly IDocumentStore _store;
    private readonly IClock _clock;

    public Archival(IDocumentStore store, IClock clock)
    {
        _store = store;
        _clock = clock;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var session = _store.QuerySession();
        var uitnodigingen = new Dictionary<UitnodigingsStatus, IReadOnlyList<Uitnodiging>>
        {
            {
                UitnodigingsStatus.WachtOpAntwoord, await session
                    .Query<Uitnodiging>()
                    .Where(u => u.Status.Status == UitnodigingsStatus.WachtOpAntwoord.Status)
                    .ToListAsync(stoppingToken)
            },
            {
                UitnodigingsStatus.Aanvaard, await session
                    .Query<Uitnodiging>()
                    .Where(u => u.Status.Status == UitnodigingsStatus.Aanvaard.Status)
                    .ToListAsync(stoppingToken)
            },
            {
                UitnodigingsStatus.Geweigerd, await session
                    .Query<Uitnodiging>()
                    .Where(u => u.Status.Status == UitnodigingsStatus.Geweigerd.Status)
                    .ToListAsync(stoppingToken)
            },
            {
                UitnodigingsStatus.Ingetrokken, await session
                    .Query<Uitnodiging>()
                    .Where(u => u.Status.Status == UitnodigingsStatus.Ingetrokken.Status)
                    .ToListAsync(stoppingToken)
            },
            {
                UitnodigingsStatus.Verlopen, await session
                    .Query<Uitnodiging>()
                    .Where(u => u.Status.Status == UitnodigingsStatus.Verlopen.Status)
                    .ToListAsync(stoppingToken)
            }
        };

    }
}