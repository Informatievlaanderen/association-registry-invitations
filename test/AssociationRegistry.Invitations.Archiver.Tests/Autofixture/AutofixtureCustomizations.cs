using AutoFixture;

namespace AssociationRegistry.Invitations.Archiver.Tests.Autofixture;

public static class AutofixtureCustomizations
{
    public static IFixture CustomizeAll(this IFixture fixture)
    {
        return fixture
                .CustomizeTestInsz()
                .CustomizeTestVCode()
                .CustomizeUitgenodigde()
                .CustomizeUitnodiging();
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
        fixture.Customize<Uitgenodigde>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => new Uitgenodigde()
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
    
    public static IFixture CustomizeUitnodiging(this IFixture fixture)
    {
        fixture.Customize<Uitnodiging>(
            composerTransformation: composer => composer.FromFactory<int>(
                    factory: value =>
                    {
                        var randomCode = new Random().Next(0, 9999999);
                        return new Uitnodiging()
                        {
                            Uitgenodigde = fixture.Create<Uitgenodigde>(),
                            Boodschap = fixture.Create<string>(),
                            VCode = fixture.Create<TestVCode>(),
                            Uitnodiger = fixture.Create<Uitnodiger>(),
                            Status = UitnodigingsStatus.All[value % UitnodigingsStatus.All.Length]
                        };
                    })
                .OmitAutoProperties()
        );

        return fixture;
    }
}