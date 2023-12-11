namespace AssociationRegistry.Invitations.Api.Aanvragen.Registreer;

using Marten;
using Microsoft.AspNetCore.Mvc;
using Queries;
using StatusWijziging;

public static class RegistreerPipeline
{
    public static async Task<Either<AanvraagRequest>> BadRequestIfNotValid(
        this AanvraagRequest? source,
        CancellationToken cancellationToken)
    {
        var result = await new AanvraagValidator().ValidateAsync(source!, cancellationToken);

        if (!result.IsValid)
            return new Either<AanvraagRequest>
            {
                Failure = controller =>
                {
                    foreach (var error in result.Errors)
                    {
                        controller.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }

                    return Task.FromResult(controller.ValidationProblem(controller.ModelState));
                },
            };

        return new Either<AanvraagRequest> { Input = source! };
    }

    public static async Task<Either<AanvraagRequest>> BadRequestIfAanvraagReedsBestaand(
        this Either<AanvraagRequest> source,
        IDocumentSession session,
        CancellationToken cancellationToken)
    {
        if (source.Failure is not null)
            return source;

        var hasDuplicate = await session.HeeftBestaandeAanvraagVoor(source.Input.VCode, source.Input.Aanvrager.Insz.Replace(".", string.Empty).Replace("-", string.Empty), cancellationToken);

        if (hasDuplicate)
            return new Either<AanvraagRequest>
            {
                Failure = controller =>
                {
                    controller.ModelState.AddModelError(key: "Aanvraag", errorMessage: "Deze aanvraag is reeds gekend.");

                    return Task.FromResult(controller.ValidationProblem(controller.ModelState));
                },
            };

        return new Either<AanvraagRequest> { Input = source.Input };
    }

    public static async Task<IActionResult> Handle(
        this Either<AanvraagRequest> source,
        Func<Task<IActionResult>> action,
        ControllerBase controller)
    {
        if (source.Failure is not null)
            return await source.Failure(controller);

        return await action();
    }
}
