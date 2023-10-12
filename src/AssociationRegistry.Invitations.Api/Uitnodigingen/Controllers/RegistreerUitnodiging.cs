using System.Globalization;
using AssociationRegistry.Invitations.Api.Infrastructure;
using AssociationRegistry.Invitations.Api.Infrastructure.Swagger;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Mapping;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Requests;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Responses;
using Marten;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Swashbuckle.AspNetCore.Filters;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Controllers;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("")]
[SwaggerGroup.Beheer]
public class RegistreerUitnodiging : ApiController
{
    private readonly IDocumentStore _store;
    private readonly IClock _clock;

    public RegistreerUitnodiging(IDocumentStore store, IClock clock)
    {
        _store = store;
        _clock = clock;
    }

    /// <summary>
    /// Uitnodiging registreren
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="201">De uitnodiging werd geregistreerd.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    /// <returns></returns>
    [HttpPost("uitnodigingen")]
    [SwaggerResponseExample(StatusCodes.Status201Created, typeof(RegistratieResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(typeof(RegistratieResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> Post([FromBody] UitnodigingsRequest request,
        CancellationToken cancellationToken)
    {
        await using var lightweightSession = _store.LightweightSession();
        
        return await (await (await request
            .BadRequestIfNotValid(cancellationToken))
            .BadRequestIfUitnodidingReedsBestaand(lightweightSession, cancellationToken))
            .Handle(async () =>
            {
                var uitnodiging = request.ToModel();
                uitnodiging.Status = UitnodigingsStatus.WachtOpAntwoord;
                uitnodiging.DatumRegistratie = _clock.GetCurrentInstant().ToString("g", CultureInfo.InvariantCulture);
                lightweightSession.Store(uitnodiging);
                await lightweightSession.SaveChangesAsync(cancellationToken);

                return Created("uitnodigingen/0", new RegistratieResponse
                {
                    UitnodigingId = uitnodiging.Id,
                });
            }, this);
    }
}

internal class RegistratieResponseExamples : IExamplesProvider<RegistratieResponse>
{
    public RegistratieResponse GetExamples()
        => new()
        {
            UitnodigingId = Guid.NewGuid()
        };
}
