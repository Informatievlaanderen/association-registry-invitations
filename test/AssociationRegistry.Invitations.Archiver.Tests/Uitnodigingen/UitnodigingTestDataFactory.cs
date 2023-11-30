namespace AssociationRegistry.Invitations.Archiver.Tests.Uitnodigingen;

using AssociationRegistry.Invitations.Archiver.Tests.Autofixture;
using AutoFixture;
using Fixture;
using NodaTime;

public class UitnodigingTestDataFactory
{
    private readonly IFixture? _autoFixture;
    public Uitnodigingen NietOverTijd { get; }
    public Uitnodigingen OverTijd { get; }
    public Instant Date { get; }


    public UitnodigingTestDataFactory(Instant date, AppSettings options)
    {
        _autoFixture = new AutoFixture.Fixture()
            .CustomizeAll();
        
        var uitnodiging = _autoFixture.Create<Uitnodiging>() with
        {
            DatumRegistratie = date.ToDateTimeOffset(),
            DatumLaatsteAanpassing = date.ToDateTimeOffset(),
        };

        Date = date;
        NietOverTijd = new Uitnodigingen
        {
            WachtOpAntwoord = uitnodiging with { Status = UitnodigingsStatus.WachtOpAntwoord },
            Aanvaard = uitnodiging with { Status = UitnodigingsStatus.Aanvaard },
            Geweigerd = uitnodiging with { Status = UitnodigingsStatus.Geweigerd },
            Ingetrokken = uitnodiging with { Status = UitnodigingsStatus.Ingetrokken },
            Verlopen = uitnodiging with { Status = UitnodigingsStatus.Verlopen },
        };
        OverTijd = new Uitnodigingen
        {
            WachtOpAntwoord = NietOverTijd.WachtOpAntwoord with { DatumLaatsteAanpassing = ArchiverDateHelper.CalculateArchivalStartDate(options.Bewaartijden.WachtOpAntwoord, date).ToDateTimeOffset() },
            Aanvaard = NietOverTijd.Aanvaard with { DatumLaatsteAanpassing = ArchiverDateHelper.CalculateArchivalStartDate(options.Bewaartijden.Aanvaard, date).ToDateTimeOffset() },
            Geweigerd = NietOverTijd.Geweigerd with { DatumLaatsteAanpassing  = ArchiverDateHelper.CalculateArchivalStartDate(options.Bewaartijden.Geweigerd, date).ToDateTimeOffset() },
            Ingetrokken = NietOverTijd.Ingetrokken with { DatumLaatsteAanpassing  = ArchiverDateHelper.CalculateArchivalStartDate(options.Bewaartijden.Geweigerd, date).ToDateTimeOffset() },
            Verlopen = NietOverTijd.Verlopen with { DatumLaatsteAanpassing  = ArchiverDateHelper.CalculateArchivalStartDate(options.Bewaartijden.Verlopen, date).ToDateTimeOffset() },
        };
    }

    public IEnumerable<Uitnodiging> Build()
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