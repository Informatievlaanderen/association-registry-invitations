using Swashbuckle.AspNetCore.Filters;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Registreer;

internal class RegistratieResponseExamples : IExamplesProvider<RegistratieResponse>
{
    public RegistratieResponse GetExamples()
        => new()
        {
            UitnodigingId = Guid.NewGuid(),
        };
}