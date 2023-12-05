using AssociationRegistry.Invitations.Api.Infrastructure;
using AssociationRegistry.Invitations.Api.Infrastructure.Swagger;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.StatusWijziging;

using Microsoft.AspNetCore.Mvc.ModelBinding;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("")]
[SwaggerGroup.Beheer]
public class AanvaardUitnodigingsController : ApiController
{
    private readonly IQuerySession _session;
    private readonly UitnodigingsStatusHandler _handler;

    public AanvaardUitnodigingsController(IQuerySession session, UitnodigingsStatusHandler handler)
    {
        _session = session;
        _handler = handler;
    }

    /// <summary>
    /// Uitnodiging aanvaarden
    /// </summary>
    /// <param name="uitnodigingId">Het id van de te aanvaarden uitnodiging</param>
    /// <param name="cancellationToken"></param>
    /// <response code="202">De uitnodiging werd aanvaard.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    /// <returns></returns>
    [HttpPost("uitnodigingen/{uitnodigingId:guid}/aanvaardingen")]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> Post(
        [FromRoute] Guid uitnodigingId,
        [FromBody] WijzigUitnodigingStatusRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Validator is null)
        {
            var modelstate = new ModelStateDictionary();
            modelstate.AddModelError("Validator", "Validator is verplicht.");

            return ValidationProblem(modelstate);
        }

        var uitnodiging = await _session.LoadAsync<Uitnodiging>(uitnodigingId, cancellationToken);

        return await uitnodiging
                    .BadRequestIfNietBestaand()
                    .BadRequestIfReedsVerwerkt(Resources.AanvaardenUitnodigingOnmogelijk)
                    .Handle(async () =>
                     {
                         await _handler.SetStatus(uitnodiging!, UitnodigingsStatus.Aanvaard,
                                                  new Invitations.Validator
                                                  {
                                                      VertegenwoordigerId = request.Validator.VertegenwoordigerId,
                                                  }, cancellationToken);

                         return Accepted();
                     }, this);
    }
}
