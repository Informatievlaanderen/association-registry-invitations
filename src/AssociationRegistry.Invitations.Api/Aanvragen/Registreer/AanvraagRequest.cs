namespace AssociationRegistry.Invitations.Api.Aanvragen.Registreer;

using Newtonsoft.Json;
using System.Text.Json.Serialization;

public class AanvraagRequest
{
    public string VCode { get; set; } = null!;
    public string Boodschap { get; set; } = null!;
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