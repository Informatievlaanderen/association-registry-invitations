﻿using AssociationRegistry.Invitations.Api.Infrastructure;
using AssociationRegistry.Invitations.Api.Infrastructure.Swagger;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Responses;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using Uitnodiging = AssociationRegistry.Invitations.Api.Uitnodigingen.Models.Uitnodiging;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Controllers;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("")]
[SwaggerGroup.Beheer]
public class WeigerUitnodiging : ApiController
{
    private readonly IQuerySession _session;
    private readonly UitnodigingsStatusHandler _handler;

    public WeigerUitnodiging(IQuerySession session, UitnodigingsStatusHandler handler)
    {
        _session = session;
        _handler = handler;
    }

    /// <summary>
    /// Uitnodiging weigeren
    /// </summary>
    /// <param name="uitnodigingId">Het id van de te weigeren uitnodiging</param>
    /// <param name="cancellationToken"></param>
    /// <response code="202">De uitnodiging werd geweigerd.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    /// <returns></returns>
    [HttpPost("uitnodigingen/{uitnodigingId:guid}/weigeringen")]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> Post([FromRoute] Guid uitnodigingId, CancellationToken cancellationToken)
    {
        var uitnodiging = await _session.LoadAsync<Uitnodiging>(uitnodigingId, cancellationToken);
        
        return await uitnodiging
            .BadRequestIfNietBestaand()
            .BadRequestIfReedsVerwerkt(Resources.WeigerenOnmogelijk)
            .Handle(async () =>
            {
                await _handler.SetStatus(uitnodiging, UitnodigingsStatus.Geweigerd, cancellationToken);

                return Accepted();

            }, this);
    }
}

