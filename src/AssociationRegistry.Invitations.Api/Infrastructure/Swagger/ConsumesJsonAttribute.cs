using Microsoft.AspNetCore.Mvc;

namespace AssociationRegistry.Invitations.Api.Infrastructure.Swagger;

public class ConsumesJsonAttribute : ConsumesAttribute
{
    public ConsumesJsonAttribute() : base("application/json")
    {
    }
}
