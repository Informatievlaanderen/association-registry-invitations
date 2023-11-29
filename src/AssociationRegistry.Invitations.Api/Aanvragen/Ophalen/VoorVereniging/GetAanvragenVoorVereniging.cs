namespace AssociationRegistry.Invitations.Api.Aanvragen.Ophalen.VoorVereniging;

using Infrastructure;
using Infrastructure.Extensions;
using Infrastructure.Swagger;
using Marten;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("")]
[SwaggerGroup.Verenigingen]
public class GetAanvragenVoorVereniging : ApiController
{
    private readonly IQuerySession _session;

    public GetAanvragenVoorVereniging(IQuerySession session)
    {
        _session = session;
    }

    /// <summary>
    ///     Aanvragen ophalen voor vereniging
    /// </summary>
    /// <param name="vCode">De vCode van de vereniging waarvoor je de aanvragen wil ophalen</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Bevat een lijst met aanvragen voor de gevraagde vereniging.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    /// <returns></returns>
    [HttpGet("verenigingen/{vcode}/aanvragen")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AanvragenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(typeof(AanvragenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> Get([FromRoute] string vCode, CancellationToken cancellationToken)
    {
        var aanvragen = await _session
                             .Query<Invitations.Aanvraag>()
                             .MetVCode(vCode)
                             .ToListAsync(token: cancellationToken);

        return Ok(new AanvragenResponse
        {
            Aanvragen = aanvragen.Select(ToResponse).ToArray(),
        });
    }

    private static Aanvraag ToResponse(Invitations.Aanvraag model) =>
        new()
        {
            AanvraagId = model.Id,
            VCode = model.VCode,
            Boodschap = model.Boodschap,
            Status = model.Status,
            DatumRegistratie = Instant.FromDateTimeOffset(model.DatumRegistratie).AsFormattedString(),
            DatumLaatsteAanpassing = Instant.FromDateTimeOffset(model.DatumLaatsteAanpassing).AsFormattedString(),
            Aanvrager = new Aanvrager
            {
                Insz = model.Aanvrager.Insz,
                Voornaam = model.Aanvrager.Voornaam,
                Achternaam = model.Aanvrager.Achternaam,
                Email = model.Aanvrager.Email,
            },
        };
}