using System.Globalization;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Mapping;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Queries;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Requests;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Responses;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Validators;
using Marten;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Controllers;

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
            .HeeftBestaandeUitnodigingVoor(request.VCode, request.Uitgenodigde.Insz, cancellationToken);

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
