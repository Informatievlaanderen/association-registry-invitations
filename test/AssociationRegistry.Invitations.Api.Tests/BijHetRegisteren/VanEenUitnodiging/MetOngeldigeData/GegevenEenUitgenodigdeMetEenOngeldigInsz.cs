namespace AssociationRegistry.Invitations.Api.Tests.BijHetRegisteren.VanEenUitnodiging.MetOngeldigeData;

using Autofixture;
using Fixture;
using Uitnodigingen.Registreer;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenUitgenodigdeMetEenOngeldigInsz : IDisposable
{
    private readonly TestApiClient _client;
    private readonly TestApiFixture _fixture;
    private readonly Func<string, UitnodigingsRequest> _request;

    public GegevenEenUitgenodigdeMetEenOngeldigInsz(TestApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
        _request = s =>
        {
            var uitnodigingsRequest = new AutoFixture.Fixture()
                .CustomizeAll()
                .Create<UitnodigingsRequest>();
            uitnodigingsRequest.Uitgenodigde.Insz = s;
            return uitnodigingsRequest;
        };
    }

    [Theory]
    [MemberData(nameof(Data))]
    public async Task DanIsDeResponse400(string insz)
    {
        var response = await _client.Uitnodiging.RegistreerUitnodiging(_request(insz), _client);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Theory]
    [MemberData(nameof(Data))]
    public async Task DanBevatDeBodyEenErrorMessage(string insz)
    {
        var response = await _client.Uitnodiging.RegistreerUitnodiging(_request(insz), _client);

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("uitgenodigde.Insz")
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
