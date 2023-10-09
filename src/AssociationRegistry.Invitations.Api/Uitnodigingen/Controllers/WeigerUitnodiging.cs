using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using Marten;
using Microsoft.AspNetCore.Mvc;

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
    /// Weiger een uitnodiging
    /// </summary>
    /// <param name="uitnodigingsId">Het id van de te weigeren uitnodiging</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("uitnodigingen/{uitnodigingsId:guid}/weiger")]
    public async Task<IActionResult> Post([FromRoute] Guid uitnodigingsId,
        CancellationToken cancellationToken)
    {
        var uitnodiging = await _session.LoadAsync<Uitnodiging>(uitnodigingsId, cancellationToken);
        
        return await uitnodiging
            .BadRequestIfNietBestaand()
            .BadRequestIfReedsVerwerkt()
            .Handle(async () =>
            {
                await _handler.SetStatus(uitnodiging, UitnodigingsStatus.Geweigerd, cancellationToken);

                return Accepted();

            }, this);
    }
}

