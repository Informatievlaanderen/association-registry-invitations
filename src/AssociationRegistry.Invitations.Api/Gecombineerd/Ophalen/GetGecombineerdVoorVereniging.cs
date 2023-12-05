namespace AssociationRegistry.Invitations.Api.Gecombineerd.Ophalen;

using Infrastructure;
using Infrastructure.Swagger;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Queries;
using Swashbuckle.AspNetCore.Filters;
using Uitnodigingen.Ophalen.VoorVereniging;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("")]
[SwaggerGroup.Verenigingen]
public class GetUitnodigingenVoorVereniging : ApiController
{
    private readonly IQuerySession _session;

    public GetUitnodigingenVoorVereniging(IQuerySession session)
    {
        _session = session;
    }

    /// <summary>
    /// Uitnodigingen en aanvragen ophalen voor vereniging
    /// </summary>
    /// <param name="vCode">De vCode van de vereniging waarvoor je de uitnodigingen en aanvragen wil ophalen</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Bevat een lijst met uitnodigingen en aanvragen voor de gevraagde vereniging.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    /// <returns></returns>
    [HttpGet("verenigingen/{vcode}/gecombineerd")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GecombineerdResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(typeof(GecombineerdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> Get([FromRoute] string vCode, CancellationToken cancellationToken)
        => Ok(await _session.GetGecombineerd(vCode, cancellationToken));
}
