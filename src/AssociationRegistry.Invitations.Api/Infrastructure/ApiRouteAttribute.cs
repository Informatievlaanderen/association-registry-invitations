using Microsoft.AspNetCore.Mvc;

namespace AssociationRegistry.Invitations.Api.Infrastructure;

public class ApiRouteAttribute : RouteAttribute
{
    private const string Prefix = "v{version:apiVersion}/";

    public ApiRouteAttribute(string template) : base(Prefix + template) { }
}