using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace AssociationRegistry.Invitations.Api.Infrastructure.Swagger;

public class BadRequestProblemDetailsExamples : IExamplesProvider<ProblemDetails>
{
    public ProblemDetails GetExamples()
        => new()
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Er heeft zich een fout voorgedaan!",
            Detail = "Meer informatie over foutieve aanvraag",
            Type = "urn:be.vlaanderen.basisregisters.api:bad-request",
            Instance = $"http://localhost/v1/foutmeldingen/{Guid.NewGuid():N}",
        };
}

public class BadRequestValidationProblemDetailsExamples : IExamplesProvider<ValidationProblemDetails>
{
    public ValidationProblemDetails GetExamples()
        => new()
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Er heeft zich een fout voorgedaan!",
            Detail = "Meer informatie over foutieve aanvraag",
            Type = "urn:be.vlaanderen.basisregisters.api:bad-request",
            Instance = $"http://localhost/v1/foutmeldingen/{Guid.NewGuid():N}",
            Errors =
            {
                { "veldnaam", new []
                {
                    "foutboodschap omschrijving 1",
                    "foutboodschap omschrijving 2",
                } },
            },
            Extensions =
            {
                { "traceId", "00-0000000000x0xxx0x00x000000x0x000-00000x000xx00x00-00"},
            },
        };
}


public class InternalServerErrorResponseExamples : IExamplesProvider<ProblemDetails>
{
    public ProblemDetails GetExamples()
        => new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Er heeft zich een fout voorgedaan!",
            Detail = "Meer informatie over de interne fout",
            Type = "urn:be.vlaanderen.basisregisters.api:internal-server-error",
            Instance = $"http://localhost/v1/foutmeldingen/{Guid.NewGuid():N}",
        };
}
