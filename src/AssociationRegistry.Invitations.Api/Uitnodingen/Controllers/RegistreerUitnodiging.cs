using AssociationRegistry.Invitations.Api.Uitnodingen.Mapping;
using AssociationRegistry.Invitations.Api.Uitnodingen.Requests;
using AssociationRegistry.Invitations.Api.Uitnodingen.Responses;
using AssociationRegistry.Invitations.Api.Uitnodingen.Validators;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace AssociationRegistry.Invitations.Api.Uitnodingen.Controllers;

public class RegistreerUitnodiging : ControllerBase
{
    private readonly IDocumentStore _store;

    public RegistreerUitnodiging(IDocumentStore store)
    {
        _store = store;
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
        var uitnodiging = request.ToModel();
        lightweightSession.Store(uitnodiging);
        await lightweightSession.SaveChangesAsync(cancellationToken);

        return Created("uitnodigingen/0", new RegistratieResponse
        {
            Id = uitnodiging.Id,
        });
    }
}
