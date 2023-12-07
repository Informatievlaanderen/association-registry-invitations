using Swashbuckle.AspNetCore.Filters;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Ophalen.VoorVereniging;

using Infrastructure.Extensions;
using NodaTime;

internal class UitnodigingenResponseExamples : IExamplesProvider<UitnodigingenResponse>
{
    private readonly IClock _clock;

    public UitnodigingenResponseExamples(IClock clock)
    {
        _clock = clock;
    }

    public UitnodigingenResponse GetExamples()
        => new()
        {
            Uitnodigingen = new[]
            {
                new Uitnodiging
                {
                    UitnodigingId = Guid.NewGuid(),
                    VCode = "V0000001",
                    Boodschap = "Boodschap voor uitgenodigde",
                    Uitgenodigde = new Uitgenodigde
                    {
                        Voornaam = "John",
                        Achternaam = "Doe",
                        Email = "john.doe@example.com",
                        Insz = "00000000001",
                    },
                    Uitnodiger = new Uitnodiger
                    {
                        VertegenwoordigerId = 12345,
                    },
                    Status = UitnodigingsStatus.All[Random.Shared.Next(0, UitnodigingsStatus.All.Length - 1)],
                    DatumRegistratie = _clock.GetCurrentInstant().AsFormattedString(),
                    DatumLaatsteAanpassing = _clock.GetCurrentInstant().AsFormattedString(),
                },
                new Uitnodiging
                {
                    UitnodigingId = Guid.NewGuid(),
                    VCode = "V0000001",
                    Boodschap = "Boodschap voor uitgenodigde",
                    Uitgenodigde = new Uitgenodigde
                    {
                        Voornaam = "Jane",
                        Achternaam = "Smith",
                        Email = "jane.smith@example.com",
                        Insz = "00000000002",
                    },
                    Uitnodiger = new Uitnodiger
                    {
                        VertegenwoordigerId = 12345,
                    },
                    Status = UitnodigingsStatus.All[Random.Shared.Next(0, UitnodigingsStatus.All.Length - 1)],
                    DatumRegistratie = _clock.GetCurrentInstant().AsFormattedString(),
                    DatumLaatsteAanpassing = _clock.GetCurrentInstant().AsFormattedString(),
                },
            },
        };
}
