using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Queries;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Requests;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Validators;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Controllers;

public static class RegistreerPipeline
{
    public static async Task<Either<UitnodigingsRequest>> BadRequestIfNotValid(this UitnodigingsRequest? source, CancellationToken cancellationToken)
    {
        var result = await new UitnodigingsValidator().ValidateAsync(source!, cancellationToken);
        if (!result.IsValid)
            return new Either<UitnodigingsRequest>
            {
                Failure = controller =>
                {
                    {
                        foreach (var error in result.Errors)
                        {
                            controller.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                        }

                        return Task.FromResult(controller.ValidationProblem(controller.ModelState));
                    }
                }
            };

        return new Either<UitnodigingsRequest> { Input = source! };
    }
    
    
    public static async Task<Either<UitnodigingsRequest>> BadRequestIfUitnodidingReedsBestaand(this Either<UitnodigingsRequest> source, IDocumentSession session, CancellationToken cancellationToken)
    {
        if (source.Failure is not null)
            return source;
        
        var hasDuplicate = await session.Query<Uitnodiging>()
            .HeeftBestaandeUitnodigingVoor(source.Input.VCode, source.Input.Uitgenodigde.Insz, cancellationToken);
        
        if (hasDuplicate)
        {
            return new Either<UitnodigingsRequest>
            {
                Failure = controller =>
                {
                    controller.ModelState.AddModelError("Uitnodiging", "Deze vertegenwoordiger is reeds uitgenodigd.");
                    return Task.FromResult(controller.ValidationProblem(controller.ModelState));
                }
            };
        }
        
        return new Either<UitnodigingsRequest> { Input = source.Input };
    }
    
    public static async Task<IActionResult> Handle(this Either<UitnodigingsRequest> source, Func<Task<IActionResult>> action, ControllerBase controller)
    {
        if (source.Failure is not null)
            return await source.Failure(controller);

        return await action();
    }
}