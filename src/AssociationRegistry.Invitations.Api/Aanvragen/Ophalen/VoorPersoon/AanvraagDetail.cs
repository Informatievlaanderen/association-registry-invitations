namespace AssociationRegistry.Invitations.Api.Aanvragen.Ophalen.VoorPersoon;

using Newtonsoft.Json;
using System.Text.Json.Serialization;

public class AanvraagDetail
{
    public Guid AanvraagId { get; set; }
    public string VCode { get; set; } = null!;
    public string Boodschap { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string DatumRegistratie { get; set; } = null!;
    public string DatumLaatsteAanpassing { get; set; } = null!;
    public ValidatorDetail? Validator { get; set; }
    public AanvragerDetail Aanvrager { get; set; } = null!;

    public class AanvragerDetail
    {
        public string Insz { get; set; } = null!;
        public string Voornaam { get; set; } = null!;
        public string Achternaam { get; set; } = null!;

        [JsonProperty("e-mail")]
        [JsonPropertyName("e-mail")]
        public string Email { get; set; } = null!;
    }
    public class ValidatorDetail
    {
        public int VertegenwoordigerId { get; set; }
    }
}
