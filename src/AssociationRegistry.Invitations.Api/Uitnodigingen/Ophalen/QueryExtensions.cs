using Marten;
using Marten.Linq;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Ophalen;

public static class QueryExtensions
{
    public static async Task<IMartenQueryable<Uitnodiging>> Uitnodigingen(this IDocumentStore source)
    {
        await using var querySession = source.QuerySession();
        return querySession.Query<Uitnodiging>();
    }


    public static IQueryable<Uitnodiging> MetVCode(this IQueryable<Uitnodiging> source, string vCode)
    {
        return source.Where(u => u.VCode == vCode);
    }

    public static async Task<bool> HeeftBestaandeUitnodigingVoor(this IQueryable<Uitnodiging> source,
        string vCode,
        string insz,
        CancellationToken cancellationToken)
    {
        var all = source.ToList();
        
        return (await source.Where(u => u.VCode == vCode 
                                        && u.Uitgenodigde.Insz == insz 
                                        && u.Status.Status == UitnodigingsStatus.WachtOpAntwoord.Status)
            .FirstOrDefaultAsync(cancellationToken)) != null;
    }
}