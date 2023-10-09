using Microsoft.AspNetCore.Mvc;

namespace AssociationRegistry.Invitations.Api.Infrastructure.Swagger;

public class ProducesJsonAttribute : ProducesAttribute
{
    public ProducesJsonAttribute() : base("application/json")
    {
    }
}
