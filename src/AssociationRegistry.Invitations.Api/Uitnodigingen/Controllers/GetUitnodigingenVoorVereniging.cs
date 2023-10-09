using AssociationRegistry.Invitations.Api.Infrastructure;
using AssociationRegistry.Invitations.Api.Infrastructure.Swagger;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Mapping;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Queries;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Responses;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Uitnodiging = AssociationRegistry.Invitations.Api.Uitnodigingen.Models.Uitnodiging;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Controllers;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("")]
[SwaggerGroup.Verenigingen]
public class GetUitnodigingenVoorVereniging : ApiController
{
    private readonly IQuerySession _session;

    public GetUitnodigingenVoorVereniging(IQuerySession session)
    {
        _session = session;
    }

    /// <summary>
    /// Uitnodigingen ophalen voor vereniging 
    /// </summary>
    /// <param name="vCode">De vCode van de vereniging waarvoor je de uitnodigingen wil ophalen</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("verenigingen/{vcode}/uitnodigingen")]
    [ProducesJson]
    [ConsumesJson]
    public async Task<IActionResult> Get([FromRoute] string vCode, CancellationToken cancellationToken)
    {
        var uitnodigingen = await _session
            .Query<Uitnodiging>()
            .MetVCode(vCode)
            .ToListAsync(token: cancellationToken);

        return Ok(new UitnodigingenResponse
        {
            Uitnodigingen = uitnodigingen.Select(UitnodigingsMapper.ToResponse).ToArray(),
        });
    }
}
