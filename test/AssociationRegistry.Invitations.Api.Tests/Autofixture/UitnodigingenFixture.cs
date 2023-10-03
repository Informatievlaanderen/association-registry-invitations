using AssociationRegistry.Invitations.Api.Uitnodingen.Requests;

namespace AssociationRegistry.Invitations.Api.Tests.Autofixture;

public class UitnodigingenFixture:AutoFixture.Fixture
{
    public UitnodigingenFixture()
    {
        Customize<UitnodigingsRequest>(
            composer => composer
                .With(u => u.VCode, "V0000001")
                .With(u => u.Boodschap)
                .With(u => u.Uitgenodigde)
                .With(u => u.Uitnodiger)
                .OmitAutoProperties());
        Customize<Uitgenodigde>(
            composer => composer
                .With(u => u.Insz, "01020312316")
                .With(u => u.Naam)
                .With(u => u.Voornaam)
                .OmitAutoProperties());
        Customize<Uitnodiger>(
            composer => composer
                .With(u => u.VertegenwoordigerId)
                .OmitAutoProperties());
    }
}
