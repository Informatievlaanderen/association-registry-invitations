using Marten.Schema;

namespace AssociationRegistry.Invitations;

public record Uitnodiging
{
    [Identity] public Guid Id { get; set; }
    public string VCode { get; set; }
    public string Boodschap { get; set; }
    public UitnodigingsStatus Status { get; set; }
    public Uitnodiger Uitnodiger { get; set; }
    public Uitgenodigde Uitgenodigde { get; set; }
    public DateTimeOffset DatumRegistratie { get; set; }
    public DateTimeOffset DatumLaatsteAanpassing { get; set; }
}