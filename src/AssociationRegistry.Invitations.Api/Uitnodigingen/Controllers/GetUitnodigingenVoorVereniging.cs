using AssociationRegistry.Invitations.Api.Uitnodigingen.Mapping;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Queries;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Responses;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Uitnodiging = AssociationRegistry.Invitations.Api.Uitnodigingen.Models.Uitnodiging;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Controllers;

public class GetUitnodigingenVoorVereniging : ControllerBase
{
    private readonly IQuerySession _session;

    public GetUitnodigingenVoorVereniging(IQuerySession session)
    {
        _session = session;
    }

    [HttpGet("/verenigingen/{vcode}/uitnodigingen")]
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
