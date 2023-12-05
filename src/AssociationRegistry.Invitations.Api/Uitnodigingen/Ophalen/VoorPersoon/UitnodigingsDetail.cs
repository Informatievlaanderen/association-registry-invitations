using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Ophalen.VoorPersoon;

public class UitnodigingsDetail
{
    public Guid UitnodigingId { get; set; }
    public string VCode { get; set; } = null!;
    public string Boodschap { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string DatumRegistratie { get; set; } = null!;
    public string DatumLaatsteAanpassing { get; set; } = null!;
    public ValidatorDetail? Validator { get; set; }
    public UitnodigerDetail Uitnodiger { get; set; } = null!;
    public UitgenodigdeDetail Uitgenodigde { get; set; } = null!;

    public class UitnodigerDetail
    {
        public int VertegenwoordigerId { get; set; }
    }

    public class ValidatorDetail
    {
        public int VertegenwoordigerId { get; set; }
    }

    public class UitgenodigdeDetail
    {
        public string Insz { get; set; } = null!;
        public string Voornaam { get; set; } = null!;
        public string Achternaam { get; set; } = null!;

        [JsonProperty("e-mail")]
        [JsonPropertyName("e-mail")]
        public string Email { get; set; } = null!;
    }
}
