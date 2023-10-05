using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using Microsoft.AspNetCore.Mvc;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Controllers;

public static class Pipeline
{
    public static Either BadRequestIfNietBestaand(this Uitnodiging? source)
    {
        if (source is null)
            return new Either { Failure = controller =>
            {
                controller.ModelState.AddModelError("Uitnodiging", "Deze uitnodiging is niet gekend.");
                return controller.ValidationProblem(controller.ModelState);
            } };

        return new Either { Uitnodiging = source };
    }
    
    
    public static Either BadRequestIfReedsAanvaard(this Either source)
    {
        if (source.Failure is not null)
            return source;
        
        if (source.Uitnodiging.Status == UitnodigingsStatus.Aanvaard)
        {
            return new Either
            {
                Failure = controller =>
                {
                    controller.ModelState.AddModelError("Uitnodiging", "Deze uitnodiging is reeds verwerkt.");
                    return controller.ValidationProblem(controller.ModelState);
                }
            };
        }
        
        return new Either { Uitnodiging = source.Uitnodiging };
    }
    
    public static async Task<IActionResult> Handle(this Either source, Func<Task<IActionResult>> action, ControllerBase controller)
    {
        if (source.Failure is not null)
            return source.Failure(controller);

        return await action();
    }
}

public class Either
{
    public Uitnodiging Uitnodiging { get; set; } = null!;
    public Func<ControllerBase, IActionResult>? Failure { get; set; }
}