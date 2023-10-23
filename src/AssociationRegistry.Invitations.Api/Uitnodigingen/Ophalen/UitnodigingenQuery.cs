using Marten;

namespace AssociationRegistry.Invitations.Api.Uitnodigingen.Ophalen;

public class UitnodigingenQuery
{
    private string? _vCode;

    public UitnodigingenQuery(string vCode)
    {
        _vCode = vCode;
    }

    public static UitnodigingenQuery MetVCode(string vCode) => new(vCode);

    public async Task<IEnumerable<Uitnodiging>> ExecuteAsync(IDocumentStore store, CancellationToken cancellationToken)
    {
        await using var querySession = store.QuerySession();
        return await querySession.Query<Uitnodiging>()
            .Where(u => u.VCode == _vCode).ToListAsync(cancellationToken);
    }
}