using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Requests;

public class UitnodigingsRequest
{
    public string VCode { get; set; }
    public string Boodschap { get; set; }
    public Uitnodiger Uitnodiger { get; set; }
    public Uitgenodigde Uitgenodigde { get; set; }
}

public class Uitnodiger
{
    public int VertegenwoordigerId { get; set; }
}

public class Uitgenodigde
{
    public string Insz { get; set; }
    public string Voornaam { get; set; }
    public string Achternaam { get; set; }

    [JsonProperty("e-mail")]
    [JsonPropertyName("e-mail")]
    public string Email { get; set; }
}
