using Marten;
using Microsoft.Extensions.Hosting;
using NodaTime;

namespace AssociationRegistry.Invitations.Archiver;

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
        
        ArchiveerUitnodigingenWachtOpAntwoord(session);
        _logger.LogInformation($"{nameof(ArchiveerUitnodigingenWachtOpAntwoord)} voltooid.");
        ArchiveerUitnodigingenAanvaard(session);
        _logger.LogInformation($"{nameof(ArchiveerUitnodigingenAanvaard)} voltooid.");
        ArchiveerUitnodigingenGeweigerd(session);
        _logger.LogInformation($"{nameof(ArchiveerUitnodigingenGeweigerd)} voltooid.");
        ArchiveerUitnodigingenIngetrokken(session);
        _logger.LogInformation($"{nameof(ArchiveerUitnodigingenIngetrokken)} voltooid.");
        ArchiveerUitnodigingenVerlopen(session);
        _logger.LogInformation($"{nameof(ArchiveerUitnodigingenVerlopen)} voltooid.");

        ArchiveerAanvragenWachtOpAntwoord(session);
        _logger.LogInformation($"{nameof(ArchiveerAanvragenWachtOpAntwoord)} voltooid.");
        ArchiveerAanvragenAanvaard(session);
        _logger.LogInformation($"{nameof(ArchiveerAanvragenAanvaard)} voltooid.");
        ArchiveerAanvragenGeweigerd(session);
        _logger.LogInformation($"{nameof(ArchiveerAanvragenGeweigerd)} voltooid.");
        ArchiveerAanvragenIngetrokken(session);
        _logger.LogInformation($"{nameof(ArchiveerAanvragenIngetrokken)} voltooid.");
        ArchiveerAanvragenVerlopen(session);
        _logger.LogInformation($"{nameof(ArchiveerAanvragenVerlopen)} voltooid.");

        await session.SaveChangesAsync(stoppingToken);
    }
    
    private static void LogBewaartijden(ILogger<ArchiverService> logger, AppSettings.BewaartijdenOptions options)
    {
        logger.LogInformation(JsonConvert.SerializeObject(options));
    }
    
    private void ArchiveerUitnodigingenWachtOpAntwoord(IDocumentSession session)
    {
        var archivalStartDate = ArchiverDateHelper.CalculateArchivalStartDate(_options.WachtOpAntwoord, _clock.GetCurrentInstant())
                                                  .ToDateTimeOffset();

        var uitnodigingen = session
                           .Query<Uitnodiging>()
                           .Where(u =>
                                      u.Status.Status == UitnodigingsStatus.WachtOpAntwoord.Status &&
                                      u.DatumLaatsteAanpassing < archivalStartDate)
                           .ToList()
                           .Select(uitnodiging => uitnodiging with
                            {
                                Status = UitnodigingsStatus.Verlopen,
                                DatumLaatsteAanpassing = _clock.GetCurrentInstant().ToDateTimeOffset(),
                            });

        _logger.LogInformation("Beginnen met archiveren van {UitnodigingenAantal} uitnodigingen met status 'WachtOpAntwoord' die ouder zijn dan {StartDate}", uitnodigingen.Count(), archivalStartDate.UtcDateTime);
        session.Store(uitnodigingen);
    }

    private void ArchiveerUitnodigingenAanvaard(IDocumentSession session)
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

    private void ArchiveerUitnodigingenGeweigerd(IDocumentSession session)
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

    private void ArchiveerUitnodigingenIngetrokken(IDocumentSession session)
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

    private void ArchiveerUitnodigingenVerlopen(IDocumentSession session)
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
    
    private void ArchiveerAanvragenWachtOpAntwoord(IDocumentSession session)
    {
        var archivalStartDate = ArchiverDateHelper.CalculateArchivalStartDate(_options.WachtOpAntwoord, _clock.GetCurrentInstant())
                                                  .ToDateTimeOffset();

        var aanvragen = session
                           .Query<Aanvraag>()
                           .Where(u =>
                                      u.Status.Status == AanvraagStatus.WachtOpAntwoord.Status &&
                                      u.DatumLaatsteAanpassing < archivalStartDate)
                           .ToList()
                           .Select(s => s with
                            {
                                Status = AanvraagStatus.Verlopen,
                                DatumLaatsteAanpassing = _clock.GetCurrentInstant().ToDateTimeOffset(),
                            });

        _logger.LogInformation("Beginnen met archiveren van {AanvragenAantal} aanvragen met status 'WachtOpAntwoord' die ouder zijn dan {StartDate}", aanvragen.Count(), archivalStartDate.UtcDateTime);
        session.Store(aanvragen);
    }

    private void ArchiveerAanvragenAanvaard(IDocumentSession session)
    {
        var archivalStartDate = ArchiverDateHelper
                               .CalculateArchivalStartDate(
                                    _options.Aanvaard, _clock.GetCurrentInstant())
                               .ToDateTimeOffset();

        _logger.LogInformation("Beginnen met archiveren van aanvragen met status 'Aanvaard' die ouder zijn dan {StartDate}",
                               archivalStartDate);

        session.DeleteWhere<Aanvraag>(u => u.Status.Status == AanvraagStatus.Aanvaard.Status &&
                                           u.DatumLaatsteAanpassing <= archivalStartDate);
    }

    private void ArchiveerAanvragenGeweigerd(IDocumentSession session)
    {
        var archivalStartDate = ArchiverDateHelper
                               .CalculateArchivalStartDate(
                                    _options.Geweigerd, _clock.GetCurrentInstant())
                               .ToDateTimeOffset();

        _logger.LogInformation("Beginnen met archiveren van aanvragen met status 'Geweigerd' die ouder zijn dan {StartDate}",
                               archivalStartDate);

        session.DeleteWhere<Aanvraag>(u =>
                                          u.Status.Status == AanvraagStatus.Geweigerd.Status &&
                                          u.DatumLaatsteAanpassing <= archivalStartDate);
    }

    private void ArchiveerAanvragenIngetrokken(IDocumentSession session)
    {
        var archivalStartDate = ArchiverDateHelper
                               .CalculateArchivalStartDate(
                                    _options.Ingetrokken, _clock.GetCurrentInstant())
                               .ToDateTimeOffset();

        _logger.LogInformation("Beginnen met archiveren van aanvragen met status 'Ingetrokken' die ouder zijn dan {StartDate}",
                               archivalStartDate);

        session.DeleteWhere<Aanvraag>(u =>
                                          u.Status.Status == AanvraagStatus.Ingetrokken.Status &&
                                          u.DatumLaatsteAanpassing <= archivalStartDate);
    }

    private void ArchiveerAanvragenVerlopen(IDocumentSession session)
    {
        var archivalStartDate = ArchiverDateHelper
                               .CalculateArchivalStartDate(
                                    _options.Verlopen, _clock.GetCurrentInstant())
                               .ToDateTimeOffset();

        _logger.LogInformation("Beginnen met archiveren van aanvragen met status 'Verlopen' die ouder zijn dan {StartDate}",
                               archivalStartDate);

        session.DeleteWhere<Aanvraag>(u =>
                                             u.Status.Status == AanvraagStatus.Verlopen.Status &&
                                             u.DatumLaatsteAanpassing <= archivalStartDate);
    }
}