namespace AssociationRegistry.Invitations.Api.Aanvragen.Ophalen.VoorPersoon;

using Infrastructure.Extensions;
using NodaTime;
using Swashbuckle.AspNetCore.Filters;

internal class AanvraagDetailExamples : IExamplesProvider<AanvraagDetail>
{
    private readonly IClock _clock;

    public AanvraagDetailExamples(IClock clock)
    {
        _clock = clock;
    }

    public AanvraagDetail GetExamples()
        => new()
        {
            AanvraagId = Guid.NewGuid(),
            VCode = "V0000001",
            Boodschap = "Boodschap voor uitgenodigde",
            Status = AanvraagStatus.WachtOpAntwoord,
            DatumRegistratie = _clock.GetCurrentInstant().AsFormattedString(),
            DatumLaatsteAanpassing = _clock.GetCurrentInstant().AsFormattedString(),
            Validator = new AanvraagDetail.ValidatorDetail
            {
                VertegenwoordigerId = 1,
            },
            Aanvrager = new AanvraagDetail.AanvragerDetail
            {
                Voornaam = "John",
                Achternaam = "Doe",
                Email = "john.doe@example.com",
                Insz = "00000000000",
            },
        };
}
