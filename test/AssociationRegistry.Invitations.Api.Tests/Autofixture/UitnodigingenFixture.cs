using AssociationRegistry.Invitations.Api.Uitnodingen.Requests;

namespace AssociationRegistry.Invitations.Api.Tests.Autofixture;

public class UitnodigingMetVCode : ICustomization
{
    private readonly string? _vCode;

    public UitnodigingMetVCode(string? vCode = null)
    {
        _vCode = vCode;
    }

    public void Customize(IFixture fixture)
    {
        fixture.Customize<UitnodigingsRequest>(
            composer => composer
                .With(u => u.VCode, _vCode ?? $"V{fixture.Create<int>():0000000}"));
    }
}

public class UitgenodigdeMetInszEnEmail : ICustomization
{
    private readonly string? _insz;

    public UitgenodigdeMetInszEnEmail(string? insz = null)
    {
        _insz = insz;
    }

    public void Customize(IFixture fixture)
    {
        fixture.Customize<Uitgenodigde>(
            composer => composer
                .With(u => u.Insz, _insz ?? "01020312316")
                .With(u => u.Email, "test@example.org"));
    }
}

public class GeldigeUitnodigingen : CompositeCustomization
{
    public GeldigeUitnodigingen(string? vCode = null, string? insz = null)
        : base(
            new UitnodigingMetVCode(vCode),
            new UitgenodigdeMetInszEnEmail(insz))
    {
    }
}
