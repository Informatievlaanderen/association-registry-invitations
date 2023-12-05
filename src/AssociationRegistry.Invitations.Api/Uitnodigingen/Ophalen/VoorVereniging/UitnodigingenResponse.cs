using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Ophalen.VoorVereniging;

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
    public Validator? Validator { get; set; }
    public Uitnodiger Uitnodiger { get; set; } = null!;
    public Uitgenodigde Uitgenodigde { get; set; } = null!;
}

public class Uitnodiger
{
    public int VertegenwoordigerId { get; set; }
}

public class Validator
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
