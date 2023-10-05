using System.Globalization;
using AssociationRegistry.Invitations.Api.Uitnodingen.Mapping;
using AssociationRegistry.Invitations.Api.Uitnodingen.Models;
using AssociationRegistry.Invitations.Api.Uitnodingen.Requests;
using AssociationRegistry.Invitations.Api.Uitnodingen.Responses;
using AssociationRegistry.Invitations.Api.Uitnodingen.Validators;
using Marten;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace AssociationRegistry.Invitations.Api.Uitnodingen.Controllers;

public class RegistreerUitnodiging : ControllerBase
{
    private readonly IDocumentStore _store;
    private readonly IClock _clock;

    public RegistreerUitnodiging(IDocumentStore store, IClock clock)
    {
        _store = store;
        _clock = clock;
    }

    [HttpPost("/uitnodigingen")]
    public async Task<IActionResult> Post([FromBody] UitnodigingsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await new UitnodigingsValidator().ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return ValidationProblem(ModelState);
        }

        await using var lightweightSession = _store.LightweightSession();
        var hasDuplicate = await lightweightSession.Query<Models.Uitnodiging>()
            .Where(u => u.VCode == request.VCode && u.Uitgenodigde.Insz == request.Uitgenodigde.Insz).AnyAsync(cancellationToken);

        if (hasDuplicate)
        {
            ModelState.AddModelError("Uitnodiging", "Deze vertegenwoordiger is reeds uitgenodigd.");
            return ValidationProblem(ModelState);
        }

        var uitnodiging = request.ToModel();
        uitnodiging.Status = UitnodigingsStatus.WachtenOpAntwoord;
        uitnodiging.DatumLaatsteAanpassing = _clock.GetCurrentInstant().ToString("g", CultureInfo.InvariantCulture);
        lightweightSession.Store(uitnodiging);
        await lightweightSession.SaveChangesAsync(cancellationToken);

        return Created("uitnodigingen/0", new RegistratieResponse
        {
            Id = uitnodiging.Id,
        });
    }
}
