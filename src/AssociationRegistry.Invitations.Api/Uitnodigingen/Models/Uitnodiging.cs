using Marten.Schema;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Models;

public class Uitnodiging
{
    [Identity] public Guid Id { get; set; }
    public string VCode { get; set; }
    public string Boodschap { get; set; }
    public UitnodigingsStatus Status { get; set; }
    public Uitnodiger Uitnodiger { get; set; }
    public Uitgenodigde Uitgenodigde { get; set; }
    public string DatumRegistratie { get; set; }
    
    public string DatumLaatsteAanpassing { get; set; }
}