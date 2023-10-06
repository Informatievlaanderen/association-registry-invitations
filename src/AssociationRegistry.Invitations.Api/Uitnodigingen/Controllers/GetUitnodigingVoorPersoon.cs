using AssociationRegistry.Invitations.Api.Uitnodigingen.Mapping;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Uitnodiging = AssociationRegistry.Invitations.Api.Uitnodigingen.Models.Uitnodiging;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Controllers;

public class GetUitnodigingVoorPersoon : ControllerBase
{
    private readonly IQuerySession _session;

    public GetUitnodigingVoorPersoon(IQuerySession session)
    {
        _session = session;
    }

    [HttpGet("/personen/{insz}/uitnodigingen/{uitnodigingsId}")]
    public async Task<IActionResult> Get([FromRoute] string insz, [FromRoute] Guid uitnodigingsId,
        CancellationToken cancellationToken)
    {
        var uitnodiging = await _session
            .LoadAsync<Uitnodiging>(uitnodigingsId, cancellationToken);
        if (uitnodiging is null)
        {
            ModelState.AddModelError("UitnodigingsId", "Deze uitnodiging is niet gekend.");
            return ValidationProblem(ModelState);
        }

        if (uitnodiging.Uitgenodigde.Insz != insz)
        {
            ModelState.AddModelError("Insz", "Deze uitnodiging is niet voor deze persoon bestemd.");
            return ValidationProblem(ModelState);
        }

        return Ok(uitnodiging.ToDetail());
    }
}
