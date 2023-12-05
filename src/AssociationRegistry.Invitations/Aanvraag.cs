namespace AssociationRegistry.Invitations;

using Marten.Schema;

public record Aanvraag
{
    [Identity] public Guid Id { get; set; }
    public string VCode { get; set; }
    public string Boodschap { get; set; }
    public AanvraagStatus Status { get; set; }
    public Validator? Validator { get; set; }
    public Aanvrager Aanvrager { get; set; }
    public DateTimeOffset DatumRegistratie { get; set; }
    public DateTimeOffset DatumLaatsteAanpassing { get; set; }
}
