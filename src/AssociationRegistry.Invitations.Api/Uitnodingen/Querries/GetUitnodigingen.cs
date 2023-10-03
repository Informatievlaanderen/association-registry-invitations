using AssociationRegistry.Invitations.Api.Uitnodingen.Models;
using Marten;

namespace AssociationRegistry.Invitations.Api.Uitnodingen.Querries;

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
