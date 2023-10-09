using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AssociationRegistry.Invitations.Api.Infrastructure;

public class ApiControllerSpec : IApiControllerSpecification
{
    private readonly Type ApiControllerType = typeof(ApiController).GetTypeInfo();

    public bool IsSatisfiedBy(ControllerModel controller) =>
        ApiControllerType.IsAssignableFrom(controller.ControllerType);
}