using AssociationRegistry.Invitations.Api.Infrastructure;
using AssociationRegistry.Invitations.Api.Infrastructure.Extensions;
using AssociationRegistry.Invitations.Api.Infrastructure.Swagger;
using Marten;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Swashbuckle.AspNetCore.Filters;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Ophalen.VoorPersoon;

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
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestValidationProblemDetailsExamples))]
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

        return Ok(ToDetail(uitnodiging));
    }

    private static UitnodigingsDetail ToDetail(Uitnodiging model) =>
        new()
        {
            UitnodigingId = model.Id,
            VCode = model.VCode,
            Boodschap = model.Boodschap,
            Status = model.Status,
            DatumRegistratie = Instant.FromDateTimeOffset(model.DatumRegistratie).AsFormattedString(),
            DatumLaatsteAanpassing = Instant.FromDateTimeOffset(model.DatumLaatsteAanpassing).AsFormattedString(),
            Uitnodiger = new UitnodigingsDetail.UitnodigerDetail
            {
                VertegenwoordigerId = model.Uitnodiger.VertegenwoordigerId,
            },
            Uitgenodigde = new UitnodigingsDetail.UitgenodigdeDetail
            {
                Insz = model.Uitgenodigde.Insz,
                Voornaam = model.Uitgenodigde.Voornaam,
                Achternaam = model.Uitgenodigde.Achternaam,
                Email = model.Uitgenodigde.Email,
            },
        };

}