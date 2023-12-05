namespace AssociationRegistry.Invitations.Api.Aanvragen.Ophalen.VoorVereniging;

using Infrastructure.Extensions;
using NodaTime;
using VoorPersoon;

public static class AanvraagMapper
{
    public static AanvraagDetail ToDetail(Invitations.Aanvraag model) =>
        new()
        {
            AanvraagId = model.Id,
            VCode = model.VCode,
            Boodschap = model.Boodschap,
            Status = model.Status,
            DatumRegistratie = Instant.FromDateTimeOffset(model.DatumRegistratie).AsFormattedString(),
            DatumLaatsteAanpassing = Instant.FromDateTimeOffset(model.DatumLaatsteAanpassing).AsFormattedString(),
            Validator = model.Validator is null
                ? null
                : new AanvraagDetail.ValidatorDetail
                {
                    VertegenwoordigerId = model.Validator.VertegenwoordigerId,
                },
            Aanvrager = new AanvraagDetail.AanvragerDetail
            {
                Insz = model.Aanvrager.Insz,
                Voornaam = model.Aanvrager.Voornaam,
                Achternaam = model.Aanvrager.Achternaam,
                Email = model.Aanvrager.Email,
            },
        };

    public static Aanvraag ToResponse(Invitations.Aanvraag model) =>
        new()
        {
            AanvraagId = model.Id,
            VCode = model.VCode,
            Boodschap = model.Boodschap,
            Status = model.Status,
            DatumRegistratie = Instant.FromDateTimeOffset(model.DatumRegistratie).AsFormattedString(),
            DatumLaatsteAanpassing = Instant.FromDateTimeOffset(model.DatumLaatsteAanpassing).AsFormattedString(),
            Validator = model.Validator is null
                ? null
                : new Validator
                {
                    VertegenwoordigerId = model.Validator.VertegenwoordigerId,
                },
            Aanvrager = new Aanvrager
            {
                Insz = model.Aanvrager.Insz,
                Voornaam = model.Aanvrager.Voornaam,
                Achternaam = model.Aanvrager.Achternaam,
                Email = model.Aanvrager.Email,
            },
        };
}
