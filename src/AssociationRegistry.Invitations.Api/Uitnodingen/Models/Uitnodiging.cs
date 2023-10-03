using Marten.Schema;

namespace AssociationRegistry.Invitations.Api.Uitnodingen.Models;

public class Uitnodiging
{
    [Identity]
    public Guid Id { get; set; }
    public string VCode { get; set; }
    public string Boodschap { get; set; }
    public Uitnodiger Uitnodiger { get; set; }
    public Uitgenodigde Uitgenodigde { get; set; }
}
