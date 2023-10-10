using AssociationRegistry.Invitations.Api.Uitnodigingen.Requests;

namespace AssociationRegistry.Invitations.Api.Tests.Autofixture;

public static class AutofixtureCustomizations
{
    public static IFixture CustomizeAll(this IFixture fixture)
    {
        return fixture
            .CustomizeInsz()
            .CustomizeUitgenodigde()
            .CustomizeUitnodigingsRequest();
    }
    
    public static IFixture CustomizeInsz(this IFixture fixture)
    {
        fixture.Customize<TestInsz>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () =>
                    {
                        var inszBase = new Random().Next(0, 999999999);
                        var inszModulo = 97 - inszBase % 97;
                        return new TestInsz($"{inszBase:D9}{inszModulo:D2}");
                    })
                .OmitAutoProperties()
        );

        return fixture;
    }
    
    public static IFixture CustomizeUitgenodigde(this IFixture fixture)
    {
        fixture.Customize<Uitgenodigde>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => new Uitgenodigde()
                    {
                        Email = "test@example.com",
                        Insz = fixture.Create<TestInsz>(),
                        Naam = fixture.Create<string>(),
                        Voornaam = fixture.Create<string>()
                    })
                .OmitAutoProperties()
        );

        return fixture;
    }
    
    public static IFixture CustomizeUitnodigingsRequest(this IFixture fixture)
    {
        fixture.Customize<UitnodigingsRequest>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => new UitnodigingsRequest()
                    {
                        Uitgenodigde = fixture.Create<Uitgenodigde>(),
                        Boodschap = fixture.Create<string>(),
                        VCode = $"V{fixture.Create<int>():0000000}",
                        Uitnodiger = fixture.Create<Uitnodiger>(),
                    })
                .OmitAutoProperties()
        );

        return fixture;
    }
}