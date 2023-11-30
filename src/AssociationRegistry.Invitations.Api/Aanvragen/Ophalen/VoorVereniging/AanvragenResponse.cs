namespace AssociationRegistry.Invitations.Api.Aanvragen.Ophalen.VoorVereniging;

using Infrastructure.Extensions;
using Newtonsoft.Json;
using NodaTime;
using System.Text.Json.Serialization;

public class AanvragenResponse
{
    public Aanvraag[] Aanvragen { get; set; } = Array.Empty<Aanvraag>();
}

public class Aanvraag
{
    public Guid AanvraagId { get; set; }
    public string VCode { get; set; } = null!;
    public string Boodschap { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string DatumRegistratie { get; set; } = null!;
    public string DatumLaatsteAanpassing { get; set; } = null!;
    public Aanvrager Aanvrager { get; set; } = null!;
}

public class Aanvrager
{
    public string Insz { get; set; } = null!;
    public string Voornaam { get; set; } = null!;
    public string Achternaam { get; set; } = null!;

    [JsonProperty("e-mail")]
    [JsonPropertyName("e-mail")]
    public string Email { get; set; } = null!;
}

public static class AanvraagMapper
{
    public static Aanvraag ToResponse(Invitations.Aanvraag model) =>
        new()
        {
            AanvraagId = model.Id,
            VCode = model.VCode,
            Boodschap = model.Boodschap,
            Status = model.Status,
            DatumRegistratie = Instant.FromDateTimeOffset(model.DatumRegistratie).AsFormattedString(),
            DatumLaatsteAanpassing = Instant.FromDateTimeOffset(model.DatumLaatsteAanpassing).AsFormattedString(),
            Aanvrager = new Aanvrager
            {
                Insz = model.Aanvrager.Insz,
                Voornaam = model.Aanvrager.Voornaam,
                Achternaam = model.Aanvrager.Achternaam,
                Email = model.Aanvrager.Email,
            },
        };
}
