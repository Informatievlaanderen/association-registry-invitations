using AssociationRegistry.Invitations.Api.Infrastructure;
using AssociationRegistry.Invitations.Api.Infrastructure.Swagger;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Mapping;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Responses;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
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
    /// <param name="uitnodigingId">Het id van de uitnodiging die je wil ophalen</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Bevat de uitnodiging voor de gevraagde persoon.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    /// <returns></returns>
    [HttpGet("personen/{insz}/uitnodigingen/{uitnodigingId}")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UitnodigingsDetailExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(typeof(UitnodigingsDetail), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> Get([FromRoute] string insz, [FromRoute] Guid uitnodigingId,
        CancellationToken cancellationToken)
    {
        var uitnodiging = await _session
            .LoadAsync<Uitnodiging>(uitnodigingId, cancellationToken);
        if (uitnodiging is null)
        {
            ModelState.AddModelError("UitnodigingId", "Deze uitnodiging is niet gekend.");
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

internal class UitnodigingsDetailExamples : IExamplesProvider<UitnodigingsDetail>
{
    public UitnodigingsDetail GetExamples()
        => new()
        {
            UitnodigingId = Guid.NewGuid(),
            VCode = "V0000001",
            Boodschap = "Boodschap voor uitgenodigde",
            Status = UitnodigingsStatus.WachtOpAntwoord,
            DatumRegistratie = DateTime.Today.AddDays(-1).ToLongDateString(),
            DatumLaatsteAanpassing = DateTime.Today.ToLongDateString(),
            Uitnodiger = new UitnodigingsDetail.UitnodigerDetail
            {
                VertegenwoordigerId = 12345
            },
            Uitgenodigde = new UitnodigingsDetail.UitgenodigdeDetail
            {
                Voornaam = "John",
                Achternaam = "Doe",
                Email = "john.doe@example.com",
                Insz = "00000000000"
            }
        };
}
