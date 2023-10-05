using AssociationRegistry.Invitations.Api.Uitnodingen.Mapping;
using AssociationRegistry.Invitations.Api.Uitnodingen.Queries;
using AssociationRegistry.Invitations.Api.Uitnodingen.Responses;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace AssociationRegistry.Invitations.Api.Uitnodingen.Controllers;

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
            .Query<AssociationRegistry.Invitations.Api.Uitnodingen.Models.Uitnodiging>()
            .MetVCode(vCode)
            .ToListAsync(token: cancellationToken);

        return Ok(new UitnodigingenResponse
        {
            Uitnodigingen = uitnodigingen.Select(UitnodigingsMapper.ToResponse).ToArray(),
        });
    }
}
