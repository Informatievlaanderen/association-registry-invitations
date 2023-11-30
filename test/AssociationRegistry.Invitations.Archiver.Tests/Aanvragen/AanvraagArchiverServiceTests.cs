namespace AssociationRegistry.Invitations.Archiver.Tests.Aanvragen;

using AssociationRegistry.Invitations.Archiver.Tests.Fixture;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.DependencyInjection;

[Collection(nameof(ArchiverCollection))]
public class AanvraagArchiverServiceTests
{
    private readonly ArchiverFixture _fixture;
    private readonly AanvraagTestDataFactory _testDataFactory;

    public AanvraagArchiverServiceTests(ArchiverFixture fixture)
    {
        _fixture = fixture;
        _testDataFactory = fixture.AanvraagTestDataFactory;
    }

    [Fact]
    public async Task NietOverTijd_WachtendOpAntwoord_VerandertNiet()
    {
        using var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        var aanvraag = session.Query<Aanvraag>()
            .Where(x => x.Id == _testDataFactory.NietOverTijd.WachtOpAntwoord.Id)
            .Single();
        aanvraag.Status.Should().Be(AanvraagStatus.WachtOpAntwoord);
        aanvraag.DatumRegistratie.Should().Be(_testDataFactory.NietOverTijd.WachtOpAntwoord.DatumRegistratie);
        aanvraag.DatumLaatsteAanpassing.Should().Be(_testDataFactory.NietOverTijd.WachtOpAntwoord.DatumLaatsteAanpassing);
    }

    [Fact]
    public async Task NietOverTijd_Aanvaard_VerandertNiet()
    {
        using var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        var aanvraag = session.Query<Aanvraag>()
            .Where(x => x.Id == _testDataFactory.NietOverTijd.Aanvaard.Id)
            .Single();
        aanvraag.Status.Should().Be(AanvraagStatus.Aanvaard);
        aanvraag.DatumRegistratie.Should().Be(_testDataFactory.NietOverTijd.Aanvaard.DatumRegistratie);
        aanvraag.DatumLaatsteAanpassing.Should().Be(_testDataFactory.NietOverTijd.Aanvaard.DatumLaatsteAanpassing);
    }

    [Fact]
    public async Task NietOverTijd_Geweigerd_VerandertNiet()
    {
        using var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        var aanvraag = session.Query<Aanvraag>()
            .Where(x => x.Id == _testDataFactory.NietOverTijd.Geweigerd.Id)
            .Single();
        aanvraag.Status.Should().Be(AanvraagStatus.Geweigerd);
        aanvraag.DatumRegistratie.Should().Be(_testDataFactory.NietOverTijd.Geweigerd.DatumRegistratie);
        aanvraag.DatumLaatsteAanpassing.Should().Be(_testDataFactory.NietOverTijd.Geweigerd.DatumLaatsteAanpassing);
    }

    [Fact]
    public async Task NietOverTijd_Ingetrokken_VerandertNiet()
    {
        using var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        var aanvraag = session.Query<Aanvraag>()
            .Where(x => x.Id == _testDataFactory.NietOverTijd.Ingetrokken.Id)
            .Single();
        aanvraag.Status.Should().Be(AanvraagStatus.Ingetrokken);
        aanvraag.DatumRegistratie.Should().Be(_testDataFactory.NietOverTijd.Ingetrokken.DatumRegistratie);
        aanvraag.DatumLaatsteAanpassing.Should().Be(_testDataFactory.NietOverTijd.Ingetrokken.DatumLaatsteAanpassing);
    }

    [Fact]
    public async Task NietOverTijd_Verlopen_VerandertNiet()
    {
        using var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        var aanvraag = session.Query<Aanvraag>()
            .Where(x => x.Id == _testDataFactory.NietOverTijd.Verlopen.Id)
            .Single();
        aanvraag.Status.Should().Be(AanvraagStatus.Verlopen);
        aanvraag.DatumRegistratie.Should().Be(_testDataFactory.NietOverTijd.Verlopen.DatumRegistratie);
        aanvraag.DatumLaatsteAanpassing.Should().Be(_testDataFactory.NietOverTijd.Verlopen.DatumLaatsteAanpassing);
    }
    
    [Fact]
    public async Task OverTijd_WachtendOpAntwoord_VerandertNaar_Verlopen()
    {
        using var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        var aanvraag = session.Query<Aanvraag>()
            .Where(x => x.Id == _testDataFactory.OverTijd.WachtOpAntwoord.Id)
            .Single();

        aanvraag.Status.Should().Be(AanvraagStatus.Verlopen);
        aanvraag.DatumRegistratie.Should().Be(_testDataFactory.NietOverTijd.WachtOpAntwoord.DatumRegistratie);
        aanvraag.DatumLaatsteAanpassing.Should().BeAfter(_testDataFactory.NietOverTijd.WachtOpAntwoord.DatumLaatsteAanpassing);
    }
    
    [Fact]
    public async Task OverTijd_Aanvaard_WerdVerwijderd()
    {
        using var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        session.Query<Aanvraag>()
            .Where(x => x.Id == _testDataFactory.OverTijd.Aanvaard.Id)
            .SingleOrDefault()
            .Should()
            .BeNull();
    }

    [Fact]
    public async Task OverTijd_Geweigerd_WerdVerwijderd()
    {
        using var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        session.Query<Aanvraag>()
            .Where(x => x.Id == _testDataFactory.OverTijd.Geweigerd.Id)
            .SingleOrDefault()
            .Should()
            .BeNull();
    }
    [Fact]
    public async Task OverTijd_Ingetrokken_WerdVerwijderd()
    {
        using var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        session.Query<Aanvraag>()
            .Where(x => x.Id == _testDataFactory.OverTijd.Ingetrokken.Id)
            .SingleOrDefault()
            .Should()
            .BeNull();
    }
    [Fact]
    public async Task OverTijd_Verlopen_WerdVerwijderd()
    {
        using var store = _fixture.Application.Services.GetRequiredService<IDocumentStore>();
        await using var session = store.QuerySession();

        session.Query<Aanvraag>()
            .Where(x => x.Id == _testDataFactory.OverTijd.Verlopen.Id)
            .SingleOrDefault()
            .Should()
            .BeNull();
    }
}
