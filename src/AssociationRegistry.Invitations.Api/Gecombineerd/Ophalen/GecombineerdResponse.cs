namespace AssociationRegistry.Invitations.Api.Gecombineerd.Ophalen;

public class GecombineerdResponse
{
    public Aanvragen.Ophalen.VoorVereniging.Aanvraag[] Aanvragen { get; set; } = Array.Empty<Aanvragen.Ophalen.VoorVereniging.Aanvraag>();
    public Uitnodigingen.Ophalen.VoorVereniging.Uitnodiging[] Uitnodigingen { get; set; } = Array.Empty<Uitnodigingen.Ophalen.VoorVereniging.Uitnodiging>();
}