namespace AssociationRegistry.Invitations.Api.Aanvragen.Ophalen.VoorPersoon;

using Infrastructure;
using Infrastructure.Extensions;
using Infrastructure.Swagger;
using Marten;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Swashbuckle.AspNetCore.Filters;
using VoorVereniging;
using Aanvraag = Invitations.Aanvraag;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("")]
[SwaggerGroup.Personen]
public class GetAanvraagVoorPersoon : ApiController
{
    private readonly IQuerySession _session;

    public GetAanvraagVoorPersoon(IQuerySession session)
    {
        _session = session;
    }

    /// <summary>
    ///     Aanvragen ophalen voor persoon
    /// </summary>
    /// <param name="insz">Het insz van de persoon waarvoor je de aanvragen wil ophalen</param>
    /// <param name="aanvraagId">Het id van de aanvraag die je wil ophalen</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Bevat de aanvraag voor de gevraagde persoon.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    /// <returns></returns>
    [HttpGet("personen/{insz}/aanvragen/{aanvraagId}")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AanvraagDetailExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(typeof(AanvraagDetail), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> Get(
        [FromRoute] string insz,
        [FromRoute] Guid aanvraagId,
        CancellationToken cancellationToken)
    {
        var aanvraag = await _session
           .LoadAsync<Aanvraag>(aanvraagId, cancellationToken);

        if (aanvraag is null)
        {
            ModelState.AddModelError(key: "AanvraagId", errorMessage: "Deze aanvraag is niet gekend.");

            return ValidationProblem(ModelState);
        }

        if (aanvraag.Aanvrager.Insz != insz)
        {
            ModelState.AddModelError(key: "Insz", errorMessage: "Deze aanvraag werd niet door deze persoon aangevraagd.");

            return ValidationProblem(ModelState);
        }

        return Ok(AanvraagMapper.ToDetail(aanvraag));
    }
}
