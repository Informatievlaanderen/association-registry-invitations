using AssociationRegistry.Invitations.Api.Uitnodingen.Mapping;
using AssociationRegistry.Invitations.Api.Uitnodingen.Querries;
using AssociationRegistry.Invitations.Api.Uitnodingen.Responses;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace AssociationRegistry.Invitations.Api.Uitnodingen.Controllers;

public class GetByVCode : ControllerBase
{
    private readonly IDocumentStore _store;

    public GetByVCode(IDocumentStore store)
    {
        _store = store;
    }

    [HttpGet("/uitnodigingen")]
    public async Task<IActionResult> async([FromQuery] string vcode, CancellationToken cancellationToken)
    {
        var uitnodigingen = await GetUitnodigingen.MetVCode(vcode).ExecuteAsync(_store, cancellationToken);
        return Ok(new UitnodigingenResponse
        {
            Uitnodigingen = uitnodigingen.Select(UitnodigingsMapper.ToResponse).ToArray(),
        });
    }
}
