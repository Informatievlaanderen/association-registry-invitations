using Marten;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Ophalen;

public static class QueryExtensions
{
    public static IQueryable<Uitnodiging> MetVCode(this IQueryable<Uitnodiging> source, string vCode)
    {
        return source.Where(u => u.VCode == vCode);
    }

    public static async Task<bool> HeeftBestaandeUitnodigingVoor(this IQueryable<Uitnodiging> source,
        string vCode,
        string insz,
        CancellationToken cancellationToken)
        => (await source.Where(u => u.VCode == vCode 
                                 && u.Uitgenodigde.Insz == insz 
                                 && u.Status.Status == UitnodigingsStatus.WachtOpAntwoord.Status)
                        .FirstOrDefaultAsync(cancellationToken)) != null;
}