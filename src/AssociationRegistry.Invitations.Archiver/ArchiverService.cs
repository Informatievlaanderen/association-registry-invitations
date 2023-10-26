using Marten;
using Microsoft.Extensions.Hosting;
using NodaTime;

namespace AssociationRegistry.Invitations.Archiver;

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

public class ArchiverService : BackgroundService
{
    private readonly IDocumentStore _store;
    private readonly IClock _clock;
    private readonly ILogger<ArchiverService> _logger;
    private readonly AppSettings.BewaartijdenOptions _options;

    public ArchiverService(IDocumentStore store, IClock clock, AppSettings options, ILogger<ArchiverService> logger)
    {
        _store = store;
        _clock = clock;
        _logger = logger;
        _options = options.Bewaartijden;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LogBewaartijden(_logger, _options);

        var session = _store.LightweightSession();

        ArchiveerWachtOpAntwoord(session);
        _logger.LogInformation($"{nameof(ArchiveerWachtOpAntwoord)} voltooid.");
        ArchiveerAanvaard(session);
        _logger.LogInformation($"{nameof(ArchiveerAanvaard)} voltooid.");
        ArchiveerGeweigerd(session);
        _logger.LogInformation($"{nameof(ArchiveerGeweigerd)} voltooid.");
        ArchiveerIngetrokken(session);
        _logger.LogInformation($"{nameof(ArchiveerIngetrokken)} voltooid.");
        ArchiveerVerlopen(session);
        _logger.LogInformation($"{nameof(ArchiveerVerlopen)} voltooid.");

        await session.SaveChangesAsync(stoppingToken);
    }

    private static void LogBewaartijden(ILogger<ArchiverService> logger, AppSettings.BewaartijdenOptions options)
    {
        logger.LogInformation(JsonConvert.SerializeObject(options));
    }

    private void ArchiveerWachtOpAntwoord(IDocumentSession session)
    {
        var archivalStartDate = ArchiverDateHelper
                               .CalculateArchivalStartDate(
                                    _options.WachtOpAntwoord, _clock.GetCurrentInstant())
                               .ToDateTimeOffset();
        var uitnodigingenWachtOpAntwoord = session
                                             .Query<Uitnodiging>()
                                             .Where(u => u.Status.Status == UitnodigingsStatus.WachtOpAntwoord.Status)
                                             .ToList();
        var uitnodigingenVerlopen = uitnodigingenWachtOpAntwoord
                                   .Where(u => u.DatumLaatsteAanpassing.UtcDateTime < archivalStartDate.UtcDateTime)
                                   .Select(uitnodiging => uitnodiging with
                                    {
                                        Status = UitnodigingsStatus.Verlopen,
                                        DatumLaatsteAanpassing = _clock.GetCurrentInstant().ToDateTimeOffset().UtcDateTime,
                                    })
                                   .ToList();

        _logger.LogInformation("Beginnen met archiveren van uitnodigingen met status 'WachtOpAntwoord' die ouder zijn dan {StartDate}", archivalStartDate.UtcDateTime);
        foreach (var uitnodiging in uitnodigingenWachtOpAntwoord)
        {
            _logger.LogInformation("Uitnodiging {UitnodigingId} heeft DatumRegistratie {DatumRegistratie} en DatumLaatsteAanpassing {DatumLaatsteAanpassing} : klaar voor archiveren {WhereResult}", uitnodiging.Id, uitnodiging.DatumRegistratie.UtcDateTime, uitnodiging.DatumLaatsteAanpassing.UtcDateTime, uitnodiging.DatumLaatsteAanpassing.UtcDateTime < archivalStartDate.UtcDateTime);
        }
        _logger.LogInformation($"Er werden {uitnodigingenVerlopen.Count()} uitnodigingen gevonden die van WachtOpAntwoord naar Verlopen moeten veranderen.");

        session.Store(uitnodigingenVerlopen.ToArray());
    }

    private void ArchiveerAanvaard(IDocumentSession session)
    {
        var archivalStartDate = ArchiverDateHelper
                               .CalculateArchivalStartDate(
                                    _options.Aanvaard, _clock.GetCurrentInstant())
                               .ToDateTimeOffset();

        _logger.LogInformation("Beginnen met archiveren van uitnodigingen met status 'Aanvaard' die ouder zijn dan {StartDate}",
                               archivalStartDate);

        session.DeleteWhere<Uitnodiging>(u => u.Status.Status == UitnodigingsStatus.Aanvaard.Status &&
                                              u.DatumLaatsteAanpassing <= archivalStartDate);
    }

    private void ArchiveerGeweigerd(IDocumentSession session)
    {
        var archivalStartDate = ArchiverDateHelper
                               .CalculateArchivalStartDate(
                                    _options.Geweigerd, _clock.GetCurrentInstant())
                               .ToDateTimeOffset();

        _logger.LogInformation("Beginnen met archiveren van uitnodigingen met status 'Geweigerd' die ouder zijn dan {StartDate}",
                               archivalStartDate);

        session.DeleteWhere<Uitnodiging>(u =>
                                             u.Status.Status == UitnodigingsStatus.Geweigerd.Status &&
                                             u.DatumLaatsteAanpassing <= archivalStartDate);
    }

    private void ArchiveerIngetrokken(IDocumentSession session)
    {
        var archivalStartDate = ArchiverDateHelper
                               .CalculateArchivalStartDate(
                                    _options.Ingetrokken, _clock.GetCurrentInstant())
                               .ToDateTimeOffset();

        _logger.LogInformation("Beginnen met archiveren van uitnodigingen met status 'Ingetrokken' die ouder zijn dan {StartDate}",
                               archivalStartDate);

        session.DeleteWhere<Uitnodiging>(u =>
                                             u.Status.Status == UitnodigingsStatus.Ingetrokken.Status &&
                                             u.DatumLaatsteAanpassing <= archivalStartDate);
    }

    private void ArchiveerVerlopen(IDocumentSession session)
    {
        var archivalStartDate = ArchiverDateHelper
                               .CalculateArchivalStartDate(
                                    _options.Verlopen, _clock.GetCurrentInstant())
                               .ToDateTimeOffset();

        _logger.LogInformation("Beginnen met archiveren van uitnodigingen met status 'Verlopen' die ouder zijn dan {StartDate}",
                               archivalStartDate);

        session.DeleteWhere<Uitnodiging>(u =>
                                             u.Status.Status == UitnodigingsStatus.Verlopen.Status &&
                                             u.DatumLaatsteAanpassing <= archivalStartDate);
    }
}