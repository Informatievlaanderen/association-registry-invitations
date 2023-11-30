namespace AssociationRegistry.Invitations.Api.Queries;

using Aanvragen.Ophalen.VoorVereniging;
using Gecombineerd.Ophalen;
using Marten;
using Uitnodigingen.Ophalen.VoorVereniging;
using Aanvraag = Invitations.Aanvraag;
using Uitnodiging = Invitations.Uitnodiging;

public static class Repository
{
    public static async Task<GecombineerdResponse> GetGecombineerd(
        this IQuerySession session,
        string vCode,
        CancellationToken cancellationToken)
    {
        var uitnodigingen = await session.GetUitnodigingen(vCode, cancellationToken);
        var aanvragen = await session.GetAanvragen(vCode, cancellationToken);

        var gecombineerdResponse = new GecombineerdResponse
        {
            Uitnodigingen = uitnodigingen.Uitnodigingen,
            Aanvragen = aanvragen.Aanvragen,
        };

        return gecombineerdResponse;
    }

    public static async Task<UitnodigingenResponse> GetUitnodigingen(
        this IQuerySession session,
        string vCode,
        CancellationToken cancellationToken)
    {
        var uitnodigingen = await session
                                 .Query<Invitations.Uitnodiging>()
                                 .MetVCode(vCode)
                                 .ToListAsync(token: cancellationToken);

        var uitnodigingenResponse = new UitnodigingenResponse
        {
            Uitnodigingen = uitnodigingen.Select(UitnodigingMapper.ToResponse).ToArray(),
        };

        return uitnodigingenResponse;
    }

    public static async Task<AanvragenResponse> GetAanvragen(this IQuerySession session, string vCode, CancellationToken cancellationToken)
    {
        var aanvragen = await session
                             .Query<Invitations.Aanvraag>()
                             .MetVCode(vCode)
                             .ToListAsync(token: cancellationToken);

        var aanvragenResponse = new AanvragenResponse
        {
            Aanvragen = aanvragen.Select(AanvraagMapper.ToResponse).ToArray(),
        };

        return aanvragenResponse;
    }

    public static async Task<bool> HeeftBestaandeAanvraagVoor(
        this IQuerySession session,
        string vCode,
        string insz,
        CancellationToken cancellationToken)
        => await session.Query<Aanvraag>().Where(u => u.VCode == vCode
                                                   && u.Aanvrager.Insz == insz
                                                   && u.Status.Status == AanvraagStatus.WachtOpAntwoord.Status)
                        .CountAsync(cancellationToken) > 0;

    public static async Task<bool> HeeftBestaandeUitnodigingVoor(
        this IQuerySession session,
        string vCode,
        string insz,
        CancellationToken cancellationToken)
        => await session.Query<Uitnodiging>().Where(u => u.VCode == vCode
                                                      && u.Uitgenodigde.Insz == insz
                                                      && u.Status.Status == UitnodigingsStatus.WachtOpAntwoord.Status)
                        .CountAsync(cancellationToken) > 0;
}
