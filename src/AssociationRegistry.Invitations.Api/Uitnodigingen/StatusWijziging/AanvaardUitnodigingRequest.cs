namespace AssociationRegistry.Invitations.Api.Uitnodigingen.StatusWijziging;

public class WijzigUitnodigingStatusRequest
{
    public Validator Validator { get; set; } = null!;
}

public class Validator
{
    public int VertegenwoordigerId { get; set; }
}
