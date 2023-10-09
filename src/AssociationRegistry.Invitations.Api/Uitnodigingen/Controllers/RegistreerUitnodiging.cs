using System.Globalization;
using AssociationRegistry.Invitations.Api.Infrastructure;
using AssociationRegistry.Invitations.Api.Infrastructure.Swagger;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Mapping;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Requests;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Responses;
using Marten;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Controllers;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("")]
[SwaggerGroup.Beheer]
public class RegistreerUitnodiging : ApiController
{
    private readonly IDocumentStore _store;
    private readonly IClock _clock;

    public RegistreerUitnodiging(IDocumentStore store, IClock clock)
    {
        _store = store;
        _clock = clock;
    }

    /// <summary>
    /// Registreer een uitnodiging
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("uitnodigingen")]
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
