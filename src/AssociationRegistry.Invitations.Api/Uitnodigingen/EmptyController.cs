using System.Reflection;
using AssociationRegistry.Invitations.Api.Infrastructure;
using AssociationRegistry.Invitations.Api.Infrastructure.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen;

[ApiVersionNeutral]
[Route("")]
public class EmptyController : ApiController
{
    [HttpGet]
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    public IActionResult Get()
        => Request.Headers[HeaderNames.Accept].ToString()!.Contains("text/html")
            ? new RedirectResult("/docs")
            : new OkObjectResult($"Welcome to the Basisregisters Vlaanderen Beheer Api {Assembly.GetEntryAssembly()!.GetVersionText()}.");
}
