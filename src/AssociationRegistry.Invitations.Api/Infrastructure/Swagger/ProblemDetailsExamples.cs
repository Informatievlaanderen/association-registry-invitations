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
            Detail = "<meer informatie over foutieve aanvraag>",
            Type = "urn:be.vlaanderen.basisregisters.api:bad-request",
            Instance = $"http://localhost/v1/foutmeldingen/{Guid.NewGuid():N}"
        };
}

public class InternalServerErrorResponseExamples : IExamplesProvider<ProblemDetails>
{
    public ProblemDetails GetExamples()
        => new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Er heeft zich een fout voorgedaan!",
            Detail = "<meer informatie over de interne fout>",
            Type = "urn:be.vlaanderen.basisregisters.api:internal-server-error",
            Instance = $"http://localhost/v1/foutmeldingen/{Guid.NewGuid():N}"
        };
}
