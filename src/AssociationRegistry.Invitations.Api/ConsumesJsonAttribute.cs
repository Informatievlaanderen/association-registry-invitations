using Microsoft.AspNetCore.Mvc;

namespace AssociationRegistry.Invitations.Api;

public class ConsumesJsonAttribute : ConsumesAttribute
{
    public ConsumesJsonAttribute() : base("application/json")
    {
    }
}
