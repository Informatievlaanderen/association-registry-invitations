using Microsoft.AspNetCore.Mvc;

namespace AssociationRegistry.Invitations.Api;

public class ProducesJsonAttribute : ProducesAttribute
{
    public ProducesJsonAttribute() : base("application/json")
    {
    }
}
