namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Mapping;

public static class UitnodigingsMapper
{
    public static Models.Uitnodiging ToModel(this Requests.UitnodigingsRequest request) =>
        new()
        {
            VCode = request.VCode,
            Boodschap = request.Boodschap,
            Uitnodiger = new Models.Uitnodiger
            {
                VertegenwoordigerId = request.Uitnodiger.VertegenwoordigerId,
            },
            Uitgenodigde = new Models.Uitgenodigde
            {
                Insz = request.Uitgenodigde.Insz,
                Voornaam = request.Uitgenodigde.Voornaam,
                Achternaam = request.Uitgenodigde.Achternaam,
                Email = request.Uitgenodigde.Email,
            },
        };

    public static Responses.Uitnodiging ToResponse(this Models.Uitnodiging model) =>
        new()
        {
            UitnodigingId = model.Id,
            VCode = model.VCode,
            Boodschap = model.Boodschap,
            Status = model.Status,
            DatumRegistratie = model.DatumRegistratie,
            DatumLaatsteAanpassing = model.DatumLaatsteAanpassing,
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

    public static Responses.UitnodigingsDetail ToDetail(this Models.Uitnodiging model) =>
        new()
        {
            UitnodigingId = model.Id,
            VCode = model.VCode,
            Boodschap = model.Boodschap,
            Status = model.Status,
            DatumRegistratie = model.DatumRegistratie,
            DatumLaatsteAanpassing = model.DatumLaatsteAanpassing,
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
