using AssociationRegistry.Invitations.Api.Uitnodingen.Requests;

namespace AssociationRegistry.Invitations.Api.Tests.Autofixture;

public class UitnodigingenFixture : AutoFixture.Fixture
{
    public UitnodigingenFixture()
    {
        Customize<UitnodigingsRequest>(
            composer => composer
                .With(u => u.VCode, $"V{this.Create<int>():0000000}")
                .With(u => u.Boodschap)
                .With(u => u.Uitgenodigde)
                .With(u => u.Uitnodiger)
                .OmitAutoProperties());
        Customize<Uitgenodigde>(
            composer => composer
                .With(u => u.Insz, "01020312316")
                .With(u => u.Naam)
                .With(u => u.Voornaam)
                .With(u => u.Email, "test@example.org")
                .OmitAutoProperties());
        Customize<Uitnodiger>(
            composer => composer
                .With(u => u.VertegenwoordigerId)
                .OmitAutoProperties());
    }
}
