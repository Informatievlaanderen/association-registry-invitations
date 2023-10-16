using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using Marten;
using Marten.Linq;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Queries;

public class GetUitnodigingen
{
    private string? _vCode;

    public GetUitnodigingen(string vCode)
    {
        _vCode = vCode;
    }

    public static GetUitnodigingen MetVCode(string vCode) => new(vCode);

    public async Task<IEnumerable<Uitnodiging>> ExecuteAsync(IDocumentStore store, CancellationToken cancellationToken)
    {
        await using var querySession = store.QuerySession();
        return await querySession.Query<Uitnodiging>()
            .Where(u => u.VCode == _vCode).ToListAsync(cancellationToken);
    }
}

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

    public static Task<bool> HeeftBestaandeUitnodigingVoor(this IQueryable<Uitnodiging> source,
        string vCode,
        string insz,
        CancellationToken cancellationToken)
    {
        return source.Where(u => u.VCode == vCode 
                                 && u.Uitgenodigde.Insz == insz 
                                 && u.Status.Status == UitnodigingsStatus.WachtOpAntwoord.Status)
            .AnyAsync(cancellationToken);
    }
}
