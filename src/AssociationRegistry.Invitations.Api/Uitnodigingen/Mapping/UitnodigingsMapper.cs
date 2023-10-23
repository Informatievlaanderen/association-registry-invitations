using AssociationRegistry.Invitations.Api.Infrastructure.Extensions;
using NodaTime;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Mapping;

public static class UitnodigingsMapper
{
    public static Uitnodiging ToModel(this Requests.UitnodigingsRequest request) =>
        new()
        {
            VCode = request.VCode,
            Boodschap = request.Boodschap,
            Uitnodiger = new Uitnodiger
            {
                VertegenwoordigerId = request.Uitnodiger.VertegenwoordigerId,
            },
            Uitgenodigde = new Uitgenodigde
            {
                Insz = request.Uitgenodigde.Insz,
                Voornaam = request.Uitgenodigde.Voornaam,
                Achternaam = request.Uitgenodigde.Achternaam,
                Email = request.Uitgenodigde.Email,
            },
        };

    public static Responses.Uitnodiging ToResponse(this Uitnodiging model) =>
        new()
        {
            UitnodigingId = model.Id,
            VCode = model.VCode,
            Boodschap = model.Boodschap,
            Status = model.Status,
            DatumRegistratie = Instant.FromDateTimeOffset(model.DatumRegistratie).AsFormattedString(),
            DatumLaatsteAanpassing = Instant.FromDateTimeOffset(model.DatumLaatsteAanpassing).AsFormattedString(),
            Uitnodiger = new Responses.Uitnodiger
            {
                VertegenwoordigerId = model.Uitnodiger.VertegenwoordigerId,
            },
            Uitgenodigde = new Responses.Uitgenodigde
            {
                Insz = model.Uitgenodigde.Insz,
                Voornaam = model.Uitgenodigde.Voornaam,
                Achternaam = model.Uitgenodigde.Achternaam,
                Email = model.Uitgenodigde.Email,
            },
        };

    public static Responses.UitnodigingsDetail ToDetail(this Uitnodiging model) =>
        new()
        {
            UitnodigingId = model.Id,
            VCode = model.VCode,
            Boodschap = model.Boodschap,
            Status = model.Status,
            DatumRegistratie = Instant.FromDateTimeOffset(model.DatumRegistratie).AsFormattedString(),
            DatumLaatsteAanpassing = Instant.FromDateTimeOffset(model.DatumLaatsteAanpassing).AsFormattedString(),
            Uitnodiger = new Responses.UitnodigingsDetail.UitnodigerDetail
            {
                VertegenwoordigerId = model.Uitnodiger.VertegenwoordigerId,
            },
            Uitgenodigde = new Responses.UitnodigingsDetail.UitgenodigdeDetail
            {
                Insz = model.Uitgenodigde.Insz,
                Voornaam = model.Uitgenodigde.Voornaam,
                Achternaam = model.Uitgenodigde.Achternaam,
                Email = model.Uitgenodigde.Email,
            },
        };
}
