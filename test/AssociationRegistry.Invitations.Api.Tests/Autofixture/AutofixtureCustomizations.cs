using AssociationRegistry.Invitations.Api.Uitnodigingen.Requests;

namespace AssociationRegistry.Invitations.Api.Tests.Autofixture;

public static class AutofixtureCustomizations
{
    public static IFixture CustomizeAll(this IFixture fixture)
    {
        return fixture
            .CustomizeTestInsz()
            .CustomizeTestVCode()
            .CustomizeUitgenodigde()
            .CustomizeUitnodigingsRequest();
    }
    
    public static IFixture CustomizeTestInsz(this IFixture fixture)
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
    
    public static IFixture CustomizeTestVCode(this IFixture fixture)
    {
        fixture.Customize<TestVCode>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () =>
                    {
                        var randomCode = new Random().Next(0, 9999999);
                        return $"V{randomCode:0000000}";
                    })
                .OmitAutoProperties()
        );

        return fixture;
    }
    
    public static IFixture CustomizeUitgenodigde(this IFixture fixture)
    {
        fixture.Customize<AssociationRegistry.Invitations.Api.Uitnodigingen.Requests.Uitgenodigde>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => new AssociationRegistry.Invitations.Api.Uitnodigingen.Requests.Uitgenodigde()
                    {
                        Email = "test@example.com",
                        Insz = fixture.Create<TestInsz>(),
                        Achternaam = fixture.Create<string>(),
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
                    factory: () =>
                    {
                        var randomCode = new Random().Next(0, 9999999);
                        return new UitnodigingsRequest()
                        {
                            Uitgenodigde = fixture.Create<AssociationRegistry.Invitations.Api.Uitnodigingen.Requests.Uitgenodigde>(),
                            Boodschap = fixture.Create<string>(),
                            VCode = fixture.Create<TestVCode>(),
                            Uitnodiger = fixture.Create<AssociationRegistry.Invitations.Api.Uitnodigingen.Requests.Uitnodiger>(),
                        };
                    })
                .OmitAutoProperties()
        );

        return fixture;
    }
}