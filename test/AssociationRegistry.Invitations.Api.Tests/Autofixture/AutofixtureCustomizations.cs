using AssociationRegistry.Invitations.Api.Uitnodigingen.Registreer;

namespace AssociationRegistry.Invitations.Api.Tests.Autofixture;

using Aanvragen.Registreer;

public static class AutofixtureCustomizations
{
    public static IFixture CustomizeAll(this IFixture fixture)
    {
        return fixture
              .CustomizeTestInsz()
              .CustomizeTestVCode()
              .CustomizeUitgenodigde()
              .CustomizeUitnodigingsRequest()
              .CustomizeAanvrager()
              .CustomizeAanvraagRequest();
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
        fixture.Customize<Uitnodigingen.Registreer.Uitgenodigde>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () => new Uitnodigingen.Registreer.Uitgenodigde
                                                             {
                                                                 Email = "test@example.com",
                                                                 Insz = fixture.Create<TestInsz>(),
                                                                 Achternaam = fixture.Create<string>(),
                                                                 Voornaam = fixture.Create<string>(),
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

                                                                 return new UitnodigingsRequest
                                                                 {
                                                                     Uitgenodigde = fixture.Create<Uitnodigingen.Registreer.Uitgenodigde>(),
                                                                     Boodschap = fixture.Create<string>(),
                                                                     VCode = fixture.Create<TestVCode>(),
                                                                     Uitnodiger = fixture.Create<Uitnodigingen.Registreer.Uitnodiger>(),
                                                                 };
                                                             })
                                                        .OmitAutoProperties()
        );

        return fixture;
    }

    public static IFixture CustomizeAanvrager(this IFixture fixture)
    {
        fixture.Customize<Aanvrager>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () => new Aanvrager
                                                             {
                                                                 Email = "test@example.com",
                                                                 Insz = fixture.Create<TestInsz>(),
                                                                 Achternaam = fixture.Create<string>(),
                                                                 Voornaam = fixture.Create<string>(),
                                                             })
                                                        .OmitAutoProperties()
        );

        return fixture;
    }

    public static IFixture CustomizeAanvraagRequest(this IFixture fixture)
    {
        fixture.Customize<AanvraagRequest>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () =>
                                                             {
                                                                 var randomCode = new Random().Next(0, 9999999);

                                                                 return new AanvraagRequest()
                                                                 {
                                                                     Aanvrager = fixture.Create<Aanvrager>(),
                                                                     Boodschap = fixture.Create<string>(),
                                                                     VCode = fixture.Create<TestVCode>(),
                                                                 };
                                                             })
                                                        .OmitAutoProperties()
        );

        return fixture;
    }
}