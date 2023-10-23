using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using Marten;
using Microsoft.Extensions.Hosting;
using NodaTime;

namespace AssociationRegistry.Invitations.Archiver;

public class ArchiverService : BackgroundService
{
    private readonly IDocumentStore _store;
    private readonly IClock _clock;
    private readonly AppSettings.BewaartijdenOptions _options;

    public ArchiverService(IDocumentStore store, IClock clock, AppSettings options)
    {
        _store = store;
        _clock = clock;
        _options = options.Bewaartijden;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var session = _store.LightweightSession();
        ArchiveerWachtOpAntwoord(session);
        ArchiveerAanvaard(session);
        ArchiveerGeweigerd(session);
        ArchiveerIngetrokken(session);
        ArchiveerVerlopen(session);
        await session.SaveChangesAsync(stoppingToken);
    }

    private void ArchiveerWachtOpAntwoord(IDocumentSession session)
    {
        var uitnodigingen = session
            .Query<Uitnodiging>()
            .Where(u =>
                u.Status.Status == UitnodigingsStatus.WachtOpAntwoord.Status &&
                u.DatumLaatsteAanpassing <= ArchiverDateHelper.CalculateArchivalStartDate(_options.WachtOpAntwoord, _clock.GetCurrentInstant()).ToDateTimeOffset())
            .ToList()
            .Select(uitnodiging => uitnodiging with
            {
                Status = UitnodigingsStatus.Verlopen,
                DatumLaatsteAanpassing = _clock.GetCurrentInstant().ToDateTimeOffset()
            });
        session.Store(uitnodigingen);
    }

    private void ArchiveerAanvaard(IDocumentSession session) =>
        session.DeleteWhere<Uitnodiging>(u =>
            u.Status.Status == UitnodigingsStatus.Aanvaard.Status &&
            u.DatumLaatsteAanpassing <= ArchiverDateHelper.CalculateArchivalStartDate(_options.Aanvaard, _clock.GetCurrentInstant()).ToDateTimeOffset());

    private void ArchiveerGeweigerd(IDocumentSession session) =>
        session.DeleteWhere<Uitnodiging>(u =>
            u.Status.Status == UitnodigingsStatus.Geweigerd.Status &&
            u.DatumLaatsteAanpassing <= ArchiverDateHelper.CalculateArchivalStartDate(_options.Geweigerd, _clock.GetCurrentInstant()).ToDateTimeOffset());

    private void ArchiveerIngetrokken(IDocumentSession session) =>
        session.DeleteWhere<Uitnodiging>(u =>
            u.Status.Status == UitnodigingsStatus.Ingetrokken.Status &&
            u.DatumLaatsteAanpassing <= ArchiverDateHelper.CalculateArchivalStartDate(_options.Ingetrokken, _clock.GetCurrentInstant()).ToDateTimeOffset());

    private void ArchiveerVerlopen(IDocumentSession session) =>
        session.DeleteWhere<Uitnodiging>(u =>
            u.Status.Status == UitnodigingsStatus.Verlopen.Status &&
            u.DatumLaatsteAanpassing <= ArchiverDateHelper.CalculateArchivalStartDate(_options.Verlopen, _clock.GetCurrentInstant()).ToDateTimeOffset());
}