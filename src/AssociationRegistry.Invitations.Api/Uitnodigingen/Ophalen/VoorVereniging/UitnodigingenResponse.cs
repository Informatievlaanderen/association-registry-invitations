using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Ophalen.VoorVereniging;

using Infrastructure.Extensions;
using NodaTime;

public class UitnodigingenResponse
{
    public Uitnodiging[] Uitnodigingen { get; set; } = Array.Empty<Uitnodiging>();
}

public class Uitnodiging
{
    public Guid UitnodigingId { get; set; }
    public string VCode { get; set; } = null!;
    public string Boodschap { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string DatumRegistratie { get; set; } = null!;
    public string DatumLaatsteAanpassing { get; set; } = null!;
    public Uitnodiger Uitnodiger { get; set; } = null!;
    public Uitgenodigde Uitgenodigde { get; set; } = null!;
}

public class Uitnodiger
{
    public int VertegenwoordigerId { get; set; }
}

public class Uitgenodigde
{
    public string Insz { get; set; } = null!;
    public string Voornaam { get; set; } = null!;
    public string Achternaam { get; set; } = null!;
    
    [JsonProperty("e-mail")]
    [JsonPropertyName("e-mail")]
    public string Email { get; set; } = null!;
}

public static class UitnodigingMapper
{
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