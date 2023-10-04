using System.Net;
using AssociationRegistry.Invitations.Api.Tests.Autofixture;
using AssociationRegistry.Invitations.Api.Tests.Fixture;
using AssociationRegistry.Invitations.Api.Uitnodingen.Requests;
using Newtonsoft.Json.Linq;

namespace AssociationRegistry.Invitations.Api.Tests.BijHetRegistrerenVanEenUitnodiging;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenUitgenodigdeMetEenOngeldigInsz : IDisposable
{
    private readonly UitnodigingenApiClient _client;
    private readonly UitnodigingenApiFixture _fixture;
    private readonly Func<string, UitnodigingsRequest> _request;

    public GegevenEenUitgenodigdeMetEenOngeldigInsz(UitnodigingenApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
        _request = s => new UitnodigingenFixture()
            .Build<UitnodigingsRequest>()
            .With(u => u.Uitgenodigde,
                new UitnodigingenFixture()
                    .Build<Uitgenodigde>()
                    .With(u2 => u2.Insz, s)
                    .Create())
            .Create();
    }

    [Theory]
    [MemberData(nameof(Data))]
    public async Task DanIsDeResponse400(string insz)
    {
        var response = await _client.RegistreerUitnodiging(_request(insz));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Theory]
    [MemberData(nameof(Data))]
    public async Task DanBevatDeBodyEenErrorMessage(string insz)
    {
        var response = await _client.RegistreerUitnodiging(_request(insz));

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("Uitgenodigde.Insz")
            .WhoseValue
            .Should().ContainEquivalentOf("Insz is ongeldig. (##.##.##-###.## of ###########)");
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            yield return new object[] { "random tekst" }; // Geen insz
            yield return new object[] { "0123456789" }; // Te wijnig character
            yield return new object[] { "012345678901234" }; // Te veel nummers
            yield return new object[] { "0.123.45.678.90" }; // . op de verkeerde plaats
            yield return new object[] { "01-02-03.123-08" }; // . en - gewisseld
            yield return new object[] { "01.02.03-123-07" }; // fout modulo
        }
    }

    public void Dispose()
    {
        _fixture.ResetDatabase();
    }
}
