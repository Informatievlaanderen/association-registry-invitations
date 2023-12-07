namespace AssociationRegistry.Invitations.Api.Aanvragen.Ophalen.VoorVereniging;

using Infrastructure.Extensions;
using NodaTime;
using Swashbuckle.AspNetCore.Filters;

internal class AanvragenResponseExamples : IExamplesProvider<AanvragenResponse>
{
    private readonly IClock _clock;

    public AanvragenResponseExamples(IClock clock)
    {
        _clock = clock;
    }

    public AanvragenResponse GetExamples()
        => new()
        {
            Aanvragen = new[]
            {
                new Aanvraag
                {
                    AanvraagId = Guid.NewGuid(),
                    VCode = "V0000001",
                    Boodschap = "Boodschap voor uitgenodigde",
                    Aanvrager = new Aanvrager
                    {
                        Voornaam = "John",
                        Achternaam = "Doe",
                        Email = "john.doe@example.com",
                        Insz = "00000000001",
                    },
                    Validator = new Validator
                    {
                        VertegenwoordigerId = 1,
                    },
                    Status = AanvraagStatus.All[Random.Shared.Next(minValue: 0, AanvraagStatus.All.Length - 1)],
                    DatumRegistratie = _clock.GetCurrentInstant().AsFormattedString(),
                    DatumLaatsteAanpassing = _clock.GetCurrentInstant().AsFormattedString(),
                },
                new Aanvraag
                {
                    AanvraagId = Guid.NewGuid(),
                    VCode = "V0000001",
                    Boodschap = "Boodschap voor uitgenodigde",
                    Aanvrager = new Aanvrager
                    {
                        Voornaam = "Jane",
                        Achternaam = "Smith",
                        Email = "jane.smith@example.com",
                        Insz = "00000000002",
                    },
                    Validator = new Validator
                    {
                        VertegenwoordigerId = 3,
                    },
                    Status = AanvraagStatus.All[Random.Shared.Next(minValue: 0, AanvraagStatus.All.Length - 1)],
                    DatumRegistratie = _clock.GetCurrentInstant().AsFormattedString(),
                    DatumLaatsteAanpassing = _clock.GetCurrentInstant().AsFormattedString(),
                },
            },
        };
}
