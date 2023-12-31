namespace AssociationRegistry.Invitations.Api.Gecombineerd.Ophalen;

using Infrastructure.Extensions;
using NodaTime;
using Swashbuckle.AspNetCore.Filters;

internal class GecombineerdResponseExamples : IExamplesProvider<GecombineerdResponse>
{
    private readonly IClock _clock;

    public GecombineerdResponseExamples(IClock clock)
    {
        _clock = clock;
    }
    public GecombineerdResponse GetExamples()
        => new()
        {
            Uitnodigingen = new []
            {
                new Uitnodigingen.Ophalen.VoorVereniging.Uitnodiging
                {
                    UitnodigingId = Guid.NewGuid(),
                    VCode = "V0000001",
                    Boodschap = "Boodschap voor uitgenodigde",
                    Uitgenodigde = new Uitnodigingen.Ophalen.VoorVereniging.Uitgenodigde
                    {
                        Voornaam = "John",
                        Achternaam = "Doe",
                        Email = "john.doe@example.com",
                        Insz = "00000000001",
                    },
                    Uitnodiger = new Uitnodigingen.Ophalen.VoorVereniging.Uitnodiger
                    {
                        VertegenwoordigerId = 12345,
                    },
                    Status = UitnodigingsStatus.All[Random.Shared.Next(0, UitnodigingsStatus.All.Length - 1)],
                    DatumRegistratie = _clock.GetCurrentInstant().AsFormattedString(),
                    DatumLaatsteAanpassing = _clock.GetCurrentInstant().AsFormattedString(),
                },
                new Uitnodigingen.Ophalen.VoorVereniging.Uitnodiging
                {
                    UitnodigingId = Guid.NewGuid(),
                    VCode = "V0000001",
                    Boodschap = "Boodschap voor uitgenodigde",
                    Uitgenodigde = new Uitnodigingen.Ophalen.VoorVereniging.Uitgenodigde
                    {
                        Voornaam = "Jane",
                        Achternaam = "Smith",
                        Email = "jane.smith@example.com",
                        Insz = "00000000002",
                    },
                    Uitnodiger = new Uitnodigingen.Ophalen.VoorVereniging.Uitnodiger
                    {
                        VertegenwoordigerId = 12345,
                    },
                    Status = UitnodigingsStatus.All[Random.Shared.Next(0, UitnodigingsStatus.All.Length - 1)],
                    DatumRegistratie = _clock.GetCurrentInstant().AsFormattedString(),
                    DatumLaatsteAanpassing = _clock.GetCurrentInstant().AsFormattedString(),
                },
            },
            Aanvragen = new[]
            {
                new Aanvragen.Ophalen.VoorVereniging.Aanvraag
                {
                    AanvraagId = Guid.NewGuid(),
                    VCode = "V0000001",
                    Boodschap = "Boodschap voor uitgenodigde",
                    Aanvrager = new Aanvragen.Ophalen.VoorVereniging.Aanvrager
                    {
                        Voornaam = "John",
                        Achternaam = "Doe",
                        Email = "john.doe@example.com",
                        Insz = "00000000001",
                    },
                    Status = AanvraagStatus.All[Random.Shared.Next(minValue: 0, AanvraagStatus.All.Length - 1)],
                    DatumRegistratie = _clock.GetCurrentInstant().AsFormattedString(),
                    DatumLaatsteAanpassing = _clock.GetCurrentInstant().AsFormattedString(),
                },
                new Aanvragen.Ophalen.VoorVereniging.Aanvraag
                {
                    AanvraagId = Guid.NewGuid(),
                    VCode = "V0000001",
                    Boodschap = "Boodschap voor uitgenodigde",
                    Aanvrager = new Aanvragen.Ophalen.VoorVereniging.Aanvrager
                    {
                        Voornaam = "Jane",
                        Achternaam = "Smith",
                        Email = "jane.smith@example.com",
                        Insz = "00000000002",
                    },
                    Status = AanvraagStatus.All[Random.Shared.Next(minValue: 0, AanvraagStatus.All.Length - 1)],
                    DatumRegistratie = _clock.GetCurrentInstant().AsFormattedString(),
                    DatumLaatsteAanpassing = _clock.GetCurrentInstant().AsFormattedString(),
                },
            },

        };
}
