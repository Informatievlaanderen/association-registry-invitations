namespace AssociationRegistry.Invitations.Archiver.Tests.Aanvragen;

using AssociationRegistry.Invitations.Archiver.Tests.Autofixture;
using AssociationRegistry.Invitations.Archiver.Tests.Fixture;
using AutoFixture;
using NodaTime;
using Uitnodigingen;

public class AanvraagTestDataFactory
{
    private readonly IFixture? _autoFixture;
    public Aanvragen NietOverTijd { get; }
    public Aanvragen OverTijd { get; }
    public Instant Date { get; }


    public AanvraagTestDataFactory(Instant date, AppSettings options)
    {
        _autoFixture = new AutoFixture.Fixture()
            .CustomizeAll();
        
        var aanvraag = _autoFixture.Create<Aanvraag>() with
        {
            DatumRegistratie = date.ToDateTimeOffset(),
            DatumLaatsteAanpassing = date.ToDateTimeOffset(),
        };

        Date = date;
        NietOverTijd = new Aanvragen
        {
            WachtOpAntwoord = aanvraag with { Status = AanvraagStatus.WachtOpAntwoord },
            Aanvaard = aanvraag with { Status = AanvraagStatus.Aanvaard },
            Geweigerd = aanvraag with { Status = AanvraagStatus.Geweigerd },
            Ingetrokken = aanvraag with { Status = AanvraagStatus.Ingetrokken },
            Verlopen = aanvraag with { Status = AanvraagStatus.Verlopen },
        };
        OverTijd = new Aanvragen
        {
            WachtOpAntwoord = NietOverTijd.WachtOpAntwoord with { DatumLaatsteAanpassing = ArchiverDateHelper.CalculateArchivalStartDate(options.Bewaartijden.WachtOpAntwoord, date).ToDateTimeOffset() },
            Aanvaard = NietOverTijd.Aanvaard with { DatumLaatsteAanpassing = ArchiverDateHelper.CalculateArchivalStartDate(options.Bewaartijden.Aanvaard, date).ToDateTimeOffset() },
            Geweigerd = NietOverTijd.Geweigerd with { DatumLaatsteAanpassing  = ArchiverDateHelper.CalculateArchivalStartDate(options.Bewaartijden.Geweigerd, date).ToDateTimeOffset() },
            Ingetrokken = NietOverTijd.Ingetrokken with { DatumLaatsteAanpassing  = ArchiverDateHelper.CalculateArchivalStartDate(options.Bewaartijden.Geweigerd, date).ToDateTimeOffset() },
            Verlopen = NietOverTijd.Verlopen with { DatumLaatsteAanpassing  = ArchiverDateHelper.CalculateArchivalStartDate(options.Bewaartijden.Verlopen, date).ToDateTimeOffset() },
        };
    }

    public IEnumerable<Aanvraag> Build()
    {
        return new[]
        {
            NietOverTijd.WachtOpAntwoord, 
            NietOverTijd.Aanvaard, 
            NietOverTijd.Geweigerd, 
            NietOverTijd.Ingetrokken, 
            NietOverTijd.Verlopen, 
            OverTijd.WachtOpAntwoord,
            OverTijd.Aanvaard,
            OverTijd.Geweigerd,
            OverTijd.Ingetrokken,
            OverTijd.Verlopen,
        };
    }
}