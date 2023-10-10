using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using Microsoft.AspNetCore.Mvc;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Controllers;

public static class StatusWijzigingPipeline
{
    public static Either<Uitnodiging> BadRequestIfNietBestaand(this Uitnodiging? source)
    {
        if (source is null)
            return new Either<Uitnodiging> { Failure = controller =>
            {
                controller.ModelState.AddModelError("Uitnodiging", "Deze uitnodiging is niet gekend.");
                return Task.FromResult(controller.ValidationProblem(controller.ModelState));
            } };

        return new Either<Uitnodiging> { Input = source };
    }
    
    
    public static Either<Uitnodiging> BadRequestIfReedsVerwerkt(this Either<Uitnodiging> source, string foutboodschap)
    {
        if (source.Failure is not null)
            return source;
        
        if (source.Input.Status != UitnodigingsStatus.WachtOpAntwoord)
        {
            return new Either<Uitnodiging>
            {
                Failure = controller =>
                {
                    controller.ModelState.AddModelError("Uitnodiging", foutboodschap);
                    return Task.FromResult(controller.ValidationProblem(controller.ModelState));
                }
            };
        }
        
        return new Either<Uitnodiging> { Input = source.Input };
    }
    
    public static async Task<IActionResult> Handle(this Either<Uitnodiging> source, Func<Task<IActionResult>> action, ControllerBase controller)
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