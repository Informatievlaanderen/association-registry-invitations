using AssociationRegistry.Invitations.Api.Uitnodigingen.Mapping;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Queries;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Responses;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Uitnodiging = AssociationRegistry.Invitations.Api.Uitnodigingen.Models.Uitnodiging;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Controllers;

public class GetByVCode : ControllerBase
{
    private readonly IQuerySession _session;

    public GetByVCode(IQuerySession session)
    {
        _session = session;
    }

    [HttpGet("/uitnodigingen")]
    public async Task<IActionResult> Get([FromQuery] string vCode, CancellationToken cancellationToken)
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
