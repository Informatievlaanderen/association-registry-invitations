using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Responses;

public class UitnodigingenResponse
{
    public Uitnodiging[] Uitnodigingen { get; set; }
}

public class Uitnodiging
{
    public Guid Id { get; set; }
    public string VCode { get; set; }
    public string Boodschap { get; set; }
    public string Status { get; set; }
    public string DatumRegistratie { get; set; }
    public string DatumLaatsteAanpassing { get; set; }
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
