
// ReSharper disable ReplaceWithSingleCallToSingleOrDefault

// ReSharper disable ReplaceWithSingleCallToSingle

namespace AssociationRegistry.Invitations.Archiver.Tests.Uitnodigingen;

using Fixture;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.DependencyInjection;

[Collection(nameof(ArchiverCollection))]
public class UitnodigingArchiverServiceTests
{
    private readonly ArchiverFixture _fixture;
    private readonly UitnodigingTestDataFactory _testDataFactory;

    public UitnodigingArchiverServiceTests(ArchiverFixture fixture)
    {
        _fixture = fixture;

        _testDataFactory = fixture.UitnodigingTestDataFactory;
    }

    [Fact]
    public async Task NietOverTijd_WachtendOpAntwoord_VerandertNiet()
    {
        var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        var uitnodiging = session.Query<Uitnodiging>()
            .Where(x => x.Id == _testDataFactory.NietOverTijd.WachtOpAntwoord.Id)
            .Single();
        uitnodiging.Status.Should().Be(UitnodigingsStatus.WachtOpAntwoord);
        uitnodiging.DatumRegistratie.Should().Be(_testDataFactory.NietOverTijd.WachtOpAntwoord.DatumRegistratie);
        uitnodiging.DatumLaatsteAanpassing.Should().Be(_testDataFactory.NietOverTijd.WachtOpAntwoord.DatumLaatsteAanpassing);
    }

    [Fact]
    public async Task NietOverTijd_Aanvaard_VerandertNiet()
    {
        var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        var uitnodiging = session.Query<Uitnodiging>()
            .Where(x => x.Id == _testDataFactory.NietOverTijd.Aanvaard.Id)
            .Single();
        uitnodiging.Status.Should().Be(UitnodigingsStatus.Aanvaard);
        uitnodiging.DatumRegistratie.Should().Be(_testDataFactory.NietOverTijd.Aanvaard.DatumRegistratie);
        uitnodiging.DatumLaatsteAanpassing.Should().Be(_testDataFactory.NietOverTijd.Aanvaard.DatumLaatsteAanpassing);
    }

    [Fact]
    public async Task NietOverTijd_Geweigerd_VerandertNiet()
    {
        var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        var uitnodiging = session.Query<Uitnodiging>()
            .Where(x => x.Id == _testDataFactory.NietOverTijd.Geweigerd.Id)
            .Single();
        uitnodiging.Status.Should().Be(UitnodigingsStatus.Geweigerd);
        uitnodiging.DatumRegistratie.Should().Be(_testDataFactory.NietOverTijd.Geweigerd.DatumRegistratie);
        uitnodiging.DatumLaatsteAanpassing.Should().Be(_testDataFactory.NietOverTijd.Geweigerd.DatumLaatsteAanpassing);
    }

    [Fact]
    public async Task NietOverTijd_Ingetrokken_VerandertNiet()
    {
        var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        var uitnodiging = session.Query<Uitnodiging>()
            .Where(x => x.Id == _testDataFactory.NietOverTijd.Ingetrokken.Id)
            .Single();
        uitnodiging.Status.Should().Be(UitnodigingsStatus.Ingetrokken);
        uitnodiging.DatumRegistratie.Should().Be(_testDataFactory.NietOverTijd.Ingetrokken.DatumRegistratie);
        uitnodiging.DatumLaatsteAanpassing.Should().Be(_testDataFactory.NietOverTijd.Ingetrokken.DatumLaatsteAanpassing);
    }

    [Fact]
    public async Task NietOverTijd_Verlopen_VerandertNiet()
    {
        var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        var uitnodiging = session.Query<Uitnodiging>()
            .Where(x => x.Id == _testDataFactory.NietOverTijd.Verlopen.Id)
            .Single();
        uitnodiging.Status.Should().Be(UitnodigingsStatus.Verlopen);
        uitnodiging.DatumRegistratie.Should().Be(_testDataFactory.NietOverTijd.Verlopen.DatumRegistratie);
        uitnodiging.DatumLaatsteAanpassing.Should().Be(_testDataFactory.NietOverTijd.Verlopen.DatumLaatsteAanpassing);
    }

    [Fact]
    public async Task OverTijd_WachtendOpAntwoord_VerandertNaar_Verlopen()
    {
        var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        var uitnodiging = session.Query<Uitnodiging>()
            .Where(x => x.Id == _testDataFactory.OverTijd.WachtOpAntwoord.Id)
            .Single();

        uitnodiging.Status.Should().Be(UitnodigingsStatus.Verlopen);
        uitnodiging.DatumRegistratie.Should().Be(_testDataFactory.NietOverTijd.WachtOpAntwoord.DatumRegistratie);
        uitnodiging.DatumLaatsteAanpassing.Should().BeAfter(_testDataFactory.NietOverTijd.WachtOpAntwoord.DatumLaatsteAanpassing);
    }

    [Fact]
    public async Task OverTijd_Aanvaard_WerdVerwijderd()
    {
        var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        session.Query<Uitnodiging>()
            .Where(x => x.Id == _testDataFactory.OverTijd.Aanvaard.Id)
            .SingleOrDefault()
            .Should()
            .BeNull();
    }

    [Fact]
    public async Task OverTijd_Geweigerd_WerdVerwijderd()
    {
        var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        session.Query<Uitnodiging>()
            .Where(x => x.Id == _testDataFactory.OverTijd.Geweigerd.Id)
            .SingleOrDefault()
            .Should()
            .BeNull();
    }
    [Fact]
    public async Task OverTijd_Ingetrokken_WerdVerwijderd()
    {
        var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        session.Query<Uitnodiging>()
            .Where(x => x.Id == _testDataFactory.OverTijd.Ingetrokken.Id)
            .SingleOrDefault()
            .Should()
            .BeNull();
    }
    [Fact]
    public async Task OverTijd_Verlopen_WerdVerwijderd()
    {
        var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        session.Query<Uitnodiging>()
            .Where(x => x.Id == _testDataFactory.OverTijd.Verlopen.Id)
            .SingleOrDefault()
            .Should()
            .BeNull();
    }
}
