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
        await using var lightweightSession = _store.LightweightSession();
        
        return await (await (await request
            .BadRequestIfNotValid(cancellationToken))
            .BadRequestIfUitnodidingReedsBestaand(lightweightSession, cancellationToken))
            .Handle(async () =>
            {
                var uitnodiging = request.ToModel();
                uitnodiging.Status = UitnodigingsStatus.WachtOpAntwoord;
                uitnodiging.DatumRegistratie = _clock.GetCurrentInstant().ToString("g", CultureInfo.InvariantCulture);
                lightweightSession.Store(uitnodiging);
                await lightweightSession.SaveChangesAsync(cancellationToken);

                return Created("uitnodigingen/0", new RegistratieResponse
                {
                    Id = uitnodiging.Id,
                });
            }, this);
    }
}
