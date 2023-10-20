using AssociationRegistry.Invitations.Api.Infrastructure.Extensions;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using AssociationRegistry.Invitations.Archiver.Tests.Fixture;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable ReplaceWithSingleCallToSingle

namespace AssociationRegistry.Invitations.Archiver.Tests;

[Collection(nameof(ArchiverCollection))]
public class ArchiverServiceTests
{
    private readonly ArchiverFixture _fixture;
    private readonly UitnodigingTestDataFactory _testDataFactory;

    public ArchiverServiceTests(ArchiverFixture fixture)
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
        uitnodiging.DatumRegistratie.Should().Be(_testDataFactory.NietOverTijdUitnodigingen.WachtOpAntwoord.DatumRegistratie);
        uitnodiging.DatumLaatsteAanpassing.Should().Be(_testDataFactory.NietOverTijdUitnodigingen.WachtOpAntwoord.DatumLaatsteAanpassing);
    }

    [Fact]
    public async Task NietOverTijd_Aanvaard_VerandertNiet()
    {
        using var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        var uitnodiging = session.Query<Uitnodiging>()
            .Where(x => x.Id == _testDataFactory.NietOverTijdUitnodigingen.Aanvaard.Id)
            .Single();
        uitnodiging.Status.Should().Be(UitnodigingsStatus.Aanvaard);
        uitnodiging.DatumRegistratie.Should().Be(_testDataFactory.NietOverTijdUitnodigingen.Aanvaard.DatumRegistratie);
        uitnodiging.DatumLaatsteAanpassing.Should().Be(_testDataFactory.NietOverTijdUitnodigingen.Aanvaard.DatumLaatsteAanpassing);
    }

    [Fact]
    public async Task NietOverTijd_Geweigerd_VerandertNiet()
    {
        using var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        var uitnodiging = session.Query<Uitnodiging>()
            .Where(x => x.Id == _testDataFactory.NietOverTijdUitnodigingen.Geweigerd.Id)
            .Single();
        uitnodiging.Status.Should().Be(UitnodigingsStatus.Geweigerd);
        uitnodiging.DatumRegistratie.Should().Be(_testDataFactory.NietOverTijdUitnodigingen.Geweigerd.DatumRegistratie);
        uitnodiging.DatumLaatsteAanpassing.Should().Be(_testDataFactory.NietOverTijdUitnodigingen.Geweigerd.DatumLaatsteAanpassing);
    }

    [Fact]
    public async Task NietOverTijd_Ingetrokken_VerandertNiet()
    {
        using var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        var uitnodiging = session.Query<Uitnodiging>()
            .Where(x => x.Id == _testDataFactory.NietOverTijdUitnodigingen.Ingetrokken.Id)
            .Single();
        uitnodiging.Status.Should().Be(UitnodigingsStatus.Ingetrokken);
        uitnodiging.DatumRegistratie.Should().Be(_testDataFactory.NietOverTijdUitnodigingen.Ingetrokken.DatumRegistratie);
        uitnodiging.DatumLaatsteAanpassing.Should().Be(_testDataFactory.NietOverTijdUitnodigingen.Ingetrokken.DatumLaatsteAanpassing);
    }

    [Fact]
    public async Task NietOverTijd_Verlopen_VerandertNiet()
    {
        using var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        var uitnodiging = session.Query<Uitnodiging>()
            .Where(x => x.Id == _testDataFactory.NietOverTijdUitnodigingen.Verlopen.Id)
            .Single();
        uitnodiging.Status.Should().Be(UitnodigingsStatus.Verlopen);
        uitnodiging.DatumRegistratie.Should().Be(_testDataFactory.NietOverTijdUitnodigingen.Verlopen.DatumRegistratie);
        uitnodiging.DatumLaatsteAanpassing.Should().Be(_testDataFactory.NietOverTijdUitnodigingen.Verlopen.DatumLaatsteAanpassing);
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
        uitnodiging.DatumRegistratie.Should().Be(_testDataFactory.NietOverTijdUitnodigingen.WachtOpAntwoord.DatumRegistratie);
        uitnodiging.DatumLaatsteAanpassing.Should().BeAfter(_testDataFactory.NietOverTijdUitnodigingen.WachtOpAntwoord.DatumLaatsteAanpassing);
    }
    
    [Fact]
    public async Task OverTijd_Aanvaard_WerdVerwijderd()
    {
        using var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        session.Query<Uitnodiging>()
            .Where(x => x.Id == _testDataFactory.OverTijdUitnodigingen.Aanvaard.Id)
            .SingleOrDefault()
            .Should()
            .BeNull();
    }

    [Fact]
    public async Task OverTijd_Geweigerd_WerdVerwijderd()
    {
        using var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        session.Query<Uitnodiging>()
            .Where(x => x.Id == _testDataFactory.OverTijdUitnodigingen.Geweigerd.Id)
            .SingleOrDefault()
            .Should()
            .BeNull();
    }
    [Fact]
    public async Task OverTijd_Ingetrokken_WerdVerwijderd()
    {
        using var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        session.Query<Uitnodiging>()
            .Where(x => x.Id == _testDataFactory.OverTijdUitnodigingen.Ingetrokken.Id)
            .SingleOrDefault()
            .Should()
            .BeNull();
    }
    [Fact]
    public async Task OverTijd_Verlopen_WerdVerwijderd()
    {
        using var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        session.Query<Uitnodiging>()
            .Where(x => x.Id == _testDataFactory.OverTijdUitnodigingen.Verlopen.Id)
            .SingleOrDefault()
            .Should()
            .BeNull();
    }
}
