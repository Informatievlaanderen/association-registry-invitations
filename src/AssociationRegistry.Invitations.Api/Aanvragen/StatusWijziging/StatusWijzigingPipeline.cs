namespace AssociationRegistry.Invitations.Api.Aanvragen.StatusWijziging;

using Microsoft.AspNetCore.Mvc;

public static class StatusWijzigingPipeline
{
    public static Either<Aanvraag> BadRequestIfNietBestaand(this Aanvraag? source)
    {
        if (source is null)
            return new Either<Aanvraag>
            {
                Failure = controller =>
                {
                    controller.ModelState.AddModelError(key: "Aanvraag", errorMessage: "Deze aanvraag is niet gekend.");

                    return Task.FromResult(controller.ValidationProblem(controller.ModelState));
                },
            };

        return new Either<Aanvraag> { Input = source };
    }

    public static Either<Aanvraag> BadRequestIfReedsVerwerkt(this Either<Aanvraag> source, string foutboodschap)
    {
        if (source.Failure is not null)
            return source;

        if (source.Input.Status != AanvraagStatus.WachtOpAntwoord)
            return new Either<Aanvraag>
            {
                Failure = controller =>
                {
                    controller.ModelState.AddModelError(key: "Aanvraag", foutboodschap);

                    return Task.FromResult(controller.ValidationProblem(controller.ModelState));
                },
            };

        return new Either<Aanvraag> { Input = source.Input };
    }

    public static async Task<IActionResult> Handle(
        this Either<Aanvraag> source,
        Func<Task<IActionResult>> action,
        ControllerBase controller)
    {
        if (source.Failure is not null)
            return await source.Failure(controller);

        return await action();
    }
}

public class Either<T> where T : class
{
    public T Input { get; set; } = null!;
    public Func<ControllerBase, Task<ActionResult>>? Failure { get; set; }
}