namespace AssociationRegistry.Invitations.Api.Aanvragen.Registreer;

using Infrastructure;
using Infrastructure.Swagger;
using Marten;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("")]
[SwaggerGroup.Beheer]
public class RegistreerAanvraag : ApiController
{
    private readonly IClock _clock;
    private readonly IDocumentStore _store;

    public RegistreerAanvraag(IDocumentStore store, IClock clock)
    {
        _store = store;
        _clock = clock;
    }

    /// <summary>
    ///     Aanvraag registreren
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="201">De aanvraag werd geregistreerd.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    /// <returns></returns>
    [HttpPost("aanvragen")]
    [SwaggerResponseExample(StatusCodes.Status201Created, typeof(RegistratieResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(typeof(RegistratieResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    [ConsumesJson]
    public async Task<IActionResult> Post([FromBody] AanvraagRequest request, CancellationToken cancellationToken)
    {
        await using var lightweightSession = _store.LightweightSession();

        return await (await (await request
                   .BadRequestIfNotValid(cancellationToken))
               .BadRequestIfAanvraagReedsBestaand(lightweightSession, cancellationToken))
           .Handle(action: async () =>
            {
                var datumRegistratie = _clock.GetCurrentInstant().ToDateTimeOffset();

                var aanvraag = ToModel(request);
                aanvraag.Status = AanvraagStatus.WachtOpAntwoord;
                aanvraag.DatumRegistratie = datumRegistratie;
                aanvraag.DatumLaatsteAanpassing = datumRegistratie;
                lightweightSession.Store(aanvraag);
                await lightweightSession.SaveChangesAsync(cancellationToken);

                return Created(uri: "aanvragen/0", new RegistratieResponse
                {
                    AanvraagId = aanvraag.Id,
                });
            }, this);
    }

    public static Aanvraag ToModel(AanvraagRequest request) =>
        new()
        {
            VCode = request.VCode,
            Boodschap = request.Boodschap,
            Aanvrager = new Invitations.Aanvrager
            {
                Insz = request.Aanvrager.Insz.Trim('.', '-'),
                Voornaam = request.Aanvrager.Voornaam,
                Achternaam = request.Aanvrager.Achternaam,
                Email = request.Aanvrager.Email,
            },
        };
}
