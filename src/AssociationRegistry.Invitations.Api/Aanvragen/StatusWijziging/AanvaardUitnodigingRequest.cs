namespace AssociationRegistry.Invitations.Api.Aanvragen.StatusWijziging;

public class WijzigAanvraagStatusRequest
{
    public Validator Validator { get; set; } = null!;
}

public class Validator
{
    public int VertegenwoordigerId { get; set; }
}
