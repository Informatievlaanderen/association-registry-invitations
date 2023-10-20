using AssociationRegistry.Invitations.Api.Infrastructure.Extensions;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using AssociationRegistry.Invitations.Archiver.Tests.Autofixture;
using AutoFixture;
using NodaTime;

namespace AssociationRegistry.Invitations.Archiver.Tests.Fixture;

public class UitnodigingTestDataFactory
{
    private readonly IFixture? _autoFixture;
    public Uitnodigingen NietOverTijdUitnodigingen { get; }
    public Uitnodigingen OverTijdUitnodigingen { get; }
    public Instant Date { get; }


    public UitnodigingTestDataFactory(Instant date, ArchiverOptions options)
    {
        _autoFixture = new AutoFixture.Fixture()
            .CustomizeAll();
        
        var uitnodiging = _autoFixture.Create<Uitnodiging>() with
        {
            DatumRegistratie = date.ToDateTimeOffset(),
            DatumLaatsteAanpassing = date.ToDateTimeOffset()
        };

        Date = date;
        NietOverTijdUitnodigingen = new Uitnodigingen
        {
            WachtOpAntwoord = uitnodiging with { Status = UitnodigingsStatus.WachtOpAntwoord },
            Aanvaard = uitnodiging with { Status = UitnodigingsStatus.Aanvaard },
            Geweigerd = uitnodiging with { Status = UitnodigingsStatus.Geweigerd },
            Ingetrokken = uitnodiging with { Status = UitnodigingsStatus.Ingetrokken },
            Verlopen = uitnodiging with { Status = UitnodigingsStatus.Verlopen }
        };
        OverTijdUitnodigingen = new Uitnodigingen
        {
            WachtOpAntwoord = NietOverTijdUitnodigingen.WachtOpAntwoord with { DatumLaatsteAanpassing = ArchiverDateHelper.CalculateArchivalStartDate(options.Bewaartijden.WachtOpAntwoord, date).ToDateTimeOffset() },
            Aanvaard = NietOverTijdUitnodigingen.Aanvaard with { DatumLaatsteAanpassing = ArchiverDateHelper.CalculateArchivalStartDate(options.Bewaartijden.Aanvaard, date).ToDateTimeOffset() },
            Geweigerd = NietOverTijdUitnodigingen.Geweigerd with { DatumLaatsteAanpassing  = ArchiverDateHelper.CalculateArchivalStartDate(options.Bewaartijden.Geweigerd, date).ToDateTimeOffset() },
            Ingetrokken = NietOverTijdUitnodigingen.Ingetrokken with { DatumLaatsteAanpassing  = ArchiverDateHelper.CalculateArchivalStartDate(options.Bewaartijden.Geweigerd, date).ToDateTimeOffset() },
            Verlopen = NietOverTijdUitnodigingen.Verlopen with { DatumLaatsteAanpassing  = ArchiverDateHelper.CalculateArchivalStartDate(options.Bewaartijden.Verlopen, date).ToDateTimeOffset() }
        };
    }

    public IEnumerable<Uitnodiging> Build()
    {
        return new[]
        {
            NietOverTijdUitnodigingen.WachtOpAntwoord, 
            NietOverTijdUitnodigingen.Aanvaard, 
            NietOverTijdUitnodigingen.Geweigerd, 
            NietOverTijdUitnodigingen.Ingetrokken, 
            NietOverTijdUitnodigingen.Verlopen, 
            OverTijdUitnodigingen.WachtOpAntwoord,
            OverTijdUitnodigingen.Aanvaard,
            OverTijdUitnodigingen.Geweigerd,
            OverTijdUitnodigingen.Ingetrokken,
            OverTijdUitnodigingen.Verlopen
        };
    }
}