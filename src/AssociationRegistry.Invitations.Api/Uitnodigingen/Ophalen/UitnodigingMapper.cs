namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Ophalen.VoorVereniging;

using Infrastructure.Extensions;
using NodaTime;
using VoorPersoon;

public static class UitnodigingMapper
{
    public static UitnodigingsDetail ToDetail(Invitations.Uitnodiging model) =>
        new()
        {
            UitnodigingId = model.Id,
            VCode = model.VCode,
            Boodschap = model.Boodschap,
            Status = model.Status,
            DatumRegistratie = Instant.FromDateTimeOffset(model.DatumRegistratie).AsFormattedString(),
            DatumLaatsteAanpassing = Instant.FromDateTimeOffset(model.DatumLaatsteAanpassing).AsFormattedString(),
            Uitnodiger = new UitnodigingsDetail.UitnodigerDetail
            {
                VertegenwoordigerId = model.Uitnodiger.VertegenwoordigerId,
            },

            Uitgenodigde = new UitnodigingsDetail.UitgenodigdeDetail
            {
                Insz = model.Uitgenodigde.Insz,
                Voornaam = model.Uitgenodigde.Voornaam,
                Achternaam = model.Uitgenodigde.Achternaam,
                Email = model.Uitgenodigde.Email,
            },
        };

    public static Uitnodiging ToResponse(Invitations.Uitnodiging model) =>
        new()
        {
            UitnodigingId = model.Id,
            VCode = model.VCode,
            Boodschap = model.Boodschap,
            Status = model.Status,
            DatumRegistratie = Instant.FromDateTimeOffset(model.DatumRegistratie).AsFormattedString(),
            DatumLaatsteAanpassing = Instant.FromDateTimeOffset(model.DatumLaatsteAanpassing).AsFormattedString(),
            Uitnodiger = new Uitnodiger
            {
                VertegenwoordigerId = model.Uitnodiger.VertegenwoordigerId,
            },

            Uitgenodigde = new Uitgenodigde
            {
                Insz = model.Uitgenodigde.Insz,
                Voornaam = model.Uitgenodigde.Voornaam,
                Achternaam = model.Uitgenodigde.Achternaam,
                Email = model.Uitgenodigde.Email,
            },
        };
}
