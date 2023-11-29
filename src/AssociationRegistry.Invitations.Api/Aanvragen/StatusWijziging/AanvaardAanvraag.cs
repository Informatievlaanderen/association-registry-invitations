﻿namespace AssociationRegistry.Invitations.Api.Aanvragen.StatusWijziging;

using Infrastructure;
using Infrastructure.Swagger;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("")]
[SwaggerGroup.Beheer]
public class AanvaardAanvraagController : ApiController
{
    private readonly AanvraagStatusHandler _handler;
    private readonly IQuerySession _session;

    public AanvaardAanvraagController(IQuerySession session, AanvraagStatusHandler handler)
    {
        _session = session;
        _handler = handler;
    }

    /// <summary>
    ///     Aanvraag aanvaarden
    /// </summary>
    /// <param name="aanvraagId">Het id van de te aanvaarden aanvraag</param>
    /// <param name="cancellationToken"></param>
    /// <response code="202">De aanvraag werd aanvaard.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    /// <returns></returns>
    [HttpPost("aanvragen/{aanvraagId:guid}/aanvaardingen")]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> Post([FromRoute] Guid aanvraagId, CancellationToken cancellationToken)
    {
        var aanvraag = await _session.LoadAsync<Aanvraag>(aanvraagId, cancellationToken);

        return await aanvraag
                    .BadRequestIfNietBestaand()
                    .BadRequestIfReedsVerwerkt(Resources.AanvaardenOnmogelijk)
                    .Handle(action: async () =>
                     {
                         await _handler.SetStatus(aanvraag!, AanvraagStatus.Aanvaard, cancellationToken);

                         return Accepted();
                     }, this);
    }
}