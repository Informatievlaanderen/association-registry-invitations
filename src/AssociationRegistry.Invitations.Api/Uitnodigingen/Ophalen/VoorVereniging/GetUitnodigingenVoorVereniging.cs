using AssociationRegistry.Invitations.Api.Infrastructure;
using AssociationRegistry.Invitations.Api.Infrastructure.Extensions;
using AssociationRegistry.Invitations.Api.Infrastructure.Swagger;
using Marten;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Swashbuckle.AspNetCore.Filters;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Ophalen.VoorVereniging;

using Queries;

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
    /// Uitnodigingen ophalen voor vereniging
    /// </summary>
    /// <param name="vCode">De vCode van de vereniging waarvoor je de uitnodigingen wil ophalen</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Bevat een lijst met uitnodigingen voor de gevraagde vereniging.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    /// <returns></returns>
    [HttpGet("verenigingen/{vcode}/uitnodigingen")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UitnodigingenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(typeof(UitnodigingenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> Get([FromRoute] string vCode, CancellationToken cancellationToken)
        => Ok(await _session.GetUitnodigingen(vCode, cancellationToken));

    private static Uitnodiging ToResponse(Invitations.Uitnodiging model) =>
        new()
        {
            UitnodigingId = model.Id,
            VCode = model.VCode,
            Boodschap = model.Boodschap,
            Status = model.Status,
            DatumRegistratie = Instant.FromDateTimeOffset(model.DatumRegistratie).AsFormattedString(),
            DatumLaatsteAanpassing = Instant.FromDateTimeOffset(model.DatumLaatsteAanpassing).AsFormattedString(),
            Uitnodiger = new Uitnodiger
            {
                VertegenwoordigerId = model.Uitnodiger.VertegenwoordigerId,
            },
            Uitgenodigde = new Uitgenodigde
            {
                Insz = model.Uitgenodigde.Insz,
                Voornaam = model.Uitgenodigde.Voornaam,
                Achternaam = model.Uitgenodigde.Achternaam,
                Email = model.Uitgenodigde.Email,
            },
        };
}
