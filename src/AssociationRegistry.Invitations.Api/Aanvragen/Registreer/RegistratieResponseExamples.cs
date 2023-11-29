namespace AssociationRegistry.Invitations.Api.Aanvragen.Registreer;

using Swashbuckle.AspNetCore.Filters;

internal class RegistratieResponseExamples : IExamplesProvider<RegistratieResponse>
{
    public RegistratieResponse GetExamples()
        => new()
        {
            AanvraagId = Guid.NewGuid(),
        };
}