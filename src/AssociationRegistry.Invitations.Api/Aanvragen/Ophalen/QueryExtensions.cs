namespace AssociationRegistry.Invitations.Api.Aanvragen.Ophalen;

using Marten;

public static class QueryExtensions
{
    public static IQueryable<Aanvraag> MetVCode(this IQueryable<Aanvraag> source, string vCode)
    {
        return source.Where(u => u.VCode == vCode);
    }

    public static async Task<bool> HeeftBestaandeAanvraagVoor(
        this IQueryable<Aanvraag> source,
        string vCode,
        string insz,
        CancellationToken cancellationToken)
        => await source.Where(u => u.VCode == vCode
                                && u.Aanvrager.Insz == insz
                                && u.Status.Status == AanvraagStatus.WachtOpAntwoord.Status)
                       .FirstOrDefaultAsync(cancellationToken) != null;
}