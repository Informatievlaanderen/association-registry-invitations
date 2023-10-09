using AssociationRegistry.Invitations.Api.Uitnodigingen.Mapping;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Uitnodiging = AssociationRegistry.Invitations.Api.Uitnodigingen.Models.Uitnodiging;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Controllers;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("")]
[SwaggerGroup.Personen]
public class GetUitnodigingVoorPersoon : ApiController
{
    private readonly IQuerySession _session;

    public GetUitnodigingVoorPersoon(IQuerySession session)
    {
        _session = session;
    }

    /// <summary>
    /// Uitnodiging ophalen voor persoon
    /// </summary>
    /// <param name="insz">Het insz van de persoon waarvoor je de uitnodiging wil ophalen</param>
    /// <param name="uitnodigingsId">Het id van de uitnodiging die je wil ophalen</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("personen/{insz}/uitnodigingen/{uitnodigingsId}")]
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
