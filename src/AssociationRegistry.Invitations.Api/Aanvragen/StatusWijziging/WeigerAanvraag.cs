namespace AssociationRegistry.Invitations.Api.Aanvragen.StatusWijziging;

using Infrastructure;
using Infrastructure.Swagger;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("")]
[SwaggerGroup.Beheer]
public class WeigerAanvraagController : ApiController
{
    private readonly AanvraagStatusHandler _handler;
    private readonly IQuerySession _session;

    public WeigerAanvraagController(IQuerySession session, AanvraagStatusHandler handler)
    {
        _session = session;
        _handler = handler;
    }

    /// <summary>
    ///     Aanvraag weigeren
    /// </summary>
    /// <param name="aanvraagId">Het id van de te weigeren aanvraag</param>
    /// <param name="cancellationToken"></param>
    /// <response code="202">De aanvraag werd geweigerd.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    /// <returns></returns>
    [HttpPost("aanvragen/{aanvraagId:guid}/weigeringen")]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> Post(
        [FromRoute] Guid aanvraagId,
        [FromBody] WijzigAanvraagStatusRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Validator is null)
        {
            var modelstate = new ModelStateDictionary();
            modelstate.AddModelError("Validator", "Validator is verplicht.");

            return ValidationProblem(modelstate);
        }

        var aanvraag = await _session.LoadAsync<Aanvraag>(aanvraagId, cancellationToken);

        return await aanvraag
                    .BadRequestIfNietBestaand()
                    .BadRequestIfReedsVerwerkt(Resources.WeigerenAanvraagOnmogelijk)
                    .Handle(action: async () =>
                     {
                         int? vertegenwoordigerId = request.Validator.VertegenwoordigerId;
                         await _handler.SetStatus(aanvraag!, AanvraagStatus.Geweigerd, new Invitations.Validator
                         {
                             VertegenwoordigerId = vertegenwoordigerId.Value,
                         }, cancellationToken);

                         return Accepted();
                     }, this);
    }
}
