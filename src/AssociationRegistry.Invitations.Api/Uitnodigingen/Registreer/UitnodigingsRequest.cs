using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Registreer;

public class UitnodigingsRequest
{
    public string VCode { get; set; } = null!;
    public string Boodschap { get; set; } = null!;
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
