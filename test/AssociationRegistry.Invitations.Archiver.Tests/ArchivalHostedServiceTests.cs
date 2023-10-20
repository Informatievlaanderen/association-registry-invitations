using AssociationRegistry.Invitations.Archiver.Tests.Fixture;

namespace AssociationRegistry.Invitations.Archiver.Tests;

[Collection(nameof(UitnodigingenApiCollection))]
public class ArchivalHostedServiceTests
{
    private readonly UitnodigingenApiFixture _fixture;

    public ArchivalHostedServiceTests(UitnodigingenApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public Task BackgroundService_Should_Start()
    {
        return Task.CompletedTask;
    }
}
