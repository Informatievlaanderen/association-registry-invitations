namespace AssociationRegistry.Invitations.Api.Queries;

public static class QueryExtensions
{
    public static IQueryable<Invitations.Aanvraag> MetVCode(this IQueryable<Invitations.Aanvraag> source, string vCode)
    {
        return source.Where(u => u.VCode == vCode);
    }

    public static IQueryable<Invitations.Uitnodiging> MetVCode(this IQueryable<Invitations.Uitnodiging> source, string vCode)
    {
        return source.Where(u => u.VCode == vCode);
    }
}