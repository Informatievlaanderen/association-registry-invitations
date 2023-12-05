using AssociationRegistry.Invitations.Api.Infrastructure.Extensions;
using NodaTime;
using Swashbuckle.AspNetCore.Filters;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Ophalen.VoorPersoon;

internal class UitnodigingsDetailExamples : IExamplesProvider<UitnodigingsDetail>
{
    private readonly IClock _clock;

    public UitnodigingsDetailExamples(IClock clock)
    {
        _clock = clock;
    }

    public UitnodigingsDetail GetExamples()
        => new()
        {
            UitnodigingId = Guid.NewGuid(),
            VCode = "V0000001",
            Boodschap = "Boodschap voor uitgenodigde",
            Status = UitnodigingsStatus.WachtOpAntwoord,
            DatumRegistratie = _clock.GetCurrentInstant().AsFormattedString(),
            DatumLaatsteAanpassing = _clock.GetCurrentInstant().AsFormattedString(),
            Validator = new UitnodigingsDetail.ValidatorDetail
            {
                VertegenwoordigerId = 2,
            },
            Uitnodiger = new UitnodigingsDetail.UitnodigerDetail
            {
                VertegenwoordigerId = 12345,
            },
            Uitgenodigde = new UitnodigingsDetail.UitgenodigdeDetail
            {
                Voornaam = "John",
                Achternaam = "Doe",
                Email = "john.doe@example.com",
                Insz = "00000000000",
            },
        };
}
