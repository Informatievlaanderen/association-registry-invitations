using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using AssociationRegistry.Invitations.Archiver.Tests.Fixture;
using FluentAssertions;
using Marten;
using Marten.Internal.Sessions;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable ReplaceWithSingleCallToSingle

namespace AssociationRegistry.Invitations.Archiver.Tests;

[Collection(nameof(UitnodigingenApiCollection))]
public class ArchivalHostedServiceTests
{
    private readonly UitnodigingenApiFixture _fixture;
    private readonly UitnodigingTestDataFactory _testDataFactory;

    public ArchivalHostedServiceTests(UitnodigingenApiFixture fixture)
    {
        _fixture = fixture;

        _testDataFactory = fixture.TestDataFactory;
    }

    [Fact]
    public async Task NogNietVerlopen_WachtendOpAntwoord_VerandertNiet()
    {
        using var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();


        session.Query<Uitnodiging>()
            .Where(x => x.Id == _testDataFactory.NietVerlopenUitnodigingen.WachtOpAntwoord.Id)
            .Single()
            .Status
            .Should().Be(UitnodigingsStatus.WachtOpAntwoord);
    }
}
