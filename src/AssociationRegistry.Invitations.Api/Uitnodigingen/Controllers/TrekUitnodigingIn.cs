using AssociationRegistry.Invitations.Api.Infrastructure;
using AssociationRegistry.Invitations.Api.Infrastructure.Swagger;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Controllers;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("")]
[SwaggerGroup.Beheer]

public class TrekUitnodigingInController : ApiController
{
    private readonly IQuerySession _session;
    private readonly UitnodigingsStatusHandler _handler;

    public TrekUitnodigingInController(IQuerySession session, UitnodigingsStatusHandler handler)
    {
        _session = session;
        _handler = handler;
    }

    /// <summary>
    /// Uitnodiging intrekken
    /// </summary>
    /// <param name="uitnodigingsId">Het id van de in te trekken uitnodiging</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("uitnodigingen/{uitnodigingsId:guid}/trekin")]
    [ConsumesJson]
    [ProducesJson]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> Post([FromRoute] Guid uitnodigingsId,
        CancellationToken cancellationToken)
    {
        var uitnodiging = await _session.LoadAsync<Uitnodiging>(uitnodigingsId, cancellationToken);
        
        return await uitnodiging
            .BadRequestIfNietBestaand()
            .BadRequestIfReedsVerwerkt()
            .Handle(async () =>
            {
                await _handler.SetStatus(uitnodiging, UitnodigingsStatus.Ingetrokken, cancellationToken);

                return Accepted();

            }, this);
    }
}

