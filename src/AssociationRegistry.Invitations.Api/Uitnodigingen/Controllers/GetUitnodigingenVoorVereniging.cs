using AssociationRegistry.Invitations.Api.Infrastructure;
using AssociationRegistry.Invitations.Api.Infrastructure.Swagger;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Mapping;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Queries;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Responses;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using Uitgenodigde = AssociationRegistry.Invitations.Api.Uitnodigingen.Responses.Uitgenodigde;
using Uitnodiger = AssociationRegistry.Invitations.Api.Uitnodigingen.Responses.Uitnodiger;
using Uitnodiging = AssociationRegistry.Invitations.Api.Uitnodigingen;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Controllers;

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
    {
        var uitnodigingen = await _session
            .Query<Uitnodiging>()
            .MetVCode(vCode)
            .ToListAsync(token: cancellationToken);

        return Ok(new UitnodigingenResponse
        {
            Uitnodigingen = uitnodigingen.Select(UitnodigingsMapper.ToResponse).ToArray(),
        });
    }
}

internal class UitnodigingenResponseExamples : IExamplesProvider<UitnodigingenResponse>
{
    public UitnodigingenResponse GetExamples()
        => new()
        {
            Uitnodigingen = new []
            {
                new Responses.Uitnodiging()
                {
                    UitnodigingId = Guid.NewGuid(),
                    VCode = "V0000001",
                    Boodschap = "Boodschap voor uitgenodigde",
                    Uitgenodigde = new Responses.Uitgenodigde()
                    {
                        Voornaam = "John",
                        Achternaam = "Doe",
                        Email = "john.doe@example.com",
                        Insz = "00000000001"
                    },
                    Uitnodiger = new Responses.Uitnodiger()
                    {
                        VertegenwoordigerId = 12345
                    },
                    Status = UitnodigingsStatus.All[Random.Shared.Next(0, UitnodigingsStatus.All.Length - 1)],
                    DatumRegistratie = DateTime.Today.AddDays(-1).ToLongDateString(),
                    DatumLaatsteAanpassing = DateTime.Today.ToLongDateString()
                },
                new Responses.Uitnodiging()
                {
                    UitnodigingId = Guid.NewGuid(),
                    VCode = "V0000001",
                    Boodschap = "Boodschap voor uitgenodigde",
                    Uitgenodigde = new Responses.Uitgenodigde()
                    {
                        Voornaam = "Jane",
                        Achternaam = "Smith",
                        Email = "jane.smith@example.com",
                        Insz = "00000000002"
                    },
                    Uitnodiger = new Responses.Uitnodiger()
                    {
                        VertegenwoordigerId = 12345
                    },
                    Status = UitnodigingsStatus.All[Random.Shared.Next(0, UitnodigingsStatus.All.Length - 1)],
                    DatumRegistratie = DateTime.Today.AddDays(-1).ToLongDateString(),
                    DatumLaatsteAanpassing = DateTime.Today.ToLongDateString()
                }
            }
        };
}
