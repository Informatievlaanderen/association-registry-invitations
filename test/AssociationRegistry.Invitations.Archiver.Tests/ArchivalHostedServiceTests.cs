using AssociationRegistry.Invitations.Api.Infrastructure.Extensions;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using AssociationRegistry.Invitations.Archiver.Tests.Fixture;
using FluentAssertions;
using Marten;
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
    public async Task NietOverTijd_WachtendOpAntwoord_VerandertNiet()
    {
        using var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();


        var uitnodiging = session.Query<Uitnodiging>()
            .Where(x => x.Id == _testDataFactory.NietOverTijdUitnodigingen.WachtOpAntwoord.Id)
            .Single();
        
        uitnodiging.Status.Should().Be(UitnodigingsStatus.WachtOpAntwoord);
        uitnodiging.DatumLaatsteAanpassing =
            _testDataFactory.NietOverTijdUitnodigingen.WachtOpAntwoord.DatumLaatsteAanpassing;
    }

    [Fact]
    public async Task OverTijd_WachtendOpAntwoord_VerandertNaar_Verlopen()
    {
        using var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();


        var uitnodiging = session.Query<Uitnodiging>()
            .Where(x => x.Id == _testDataFactory.OverTijdUitnodigingen.WachtOpAntwoord.Id)
            .Single();

        uitnodiging.Status.Should().Be(UitnodigingsStatus.Verlopen);
        uitnodiging.DatumLaatsteAanpassing = _testDataFactory.Date.AsFormattedString();
    }
}
