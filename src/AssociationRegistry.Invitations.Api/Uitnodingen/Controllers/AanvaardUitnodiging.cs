using System.Globalization;
using AssociationRegistry.Invitations.Api.Uitnodingen.Models;
using AssociationRegistry.Invitations.Api.Uitnodingen.Requests;
using Marten;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace AssociationRegistry.Invitations.Api.Uitnodingen.Controllers;

public class AanvaardUitnodiging : ControllerBase
{
    private readonly IDocumentStore _store;
    private readonly IClock _clock;

    public AanvaardUitnodiging(IDocumentStore store, IClock clock)
    {
        _store = store;
        _clock = clock;
    }

    [HttpPost("/uitnodigingen/{uitnodigingsId:guid}/aanvaard")]
    public async Task<IActionResult> Post([FromRoute] Guid uitnodigingsId,
        CancellationToken cancellationToken)
    {
        await using var session = _store.LightweightSession();
        var uitnodiging = await session.LoadAsync<Uitnodiging>(uitnodigingsId, cancellationToken);
        if (uitnodiging is null) return NotFound();

        if (uitnodiging.Status == UitnodigingsStatus.Aanvaard)
        {
            ModelState.AddModelError("Uitnodiging", "Deze uitnodiging is verwerkt.");
            return ValidationProblem(ModelState);
        }

        uitnodiging.Status = UitnodigingsStatus.Aanvaard;
        uitnodiging.DatumLaatsteAanpassing = _clock.GetCurrentInstant().ToString("g", CultureInfo.InvariantCulture);

        session.Store(uitnodiging);
        await session.SaveChangesAsync(cancellationToken);

        return Accepted();
    }
}
