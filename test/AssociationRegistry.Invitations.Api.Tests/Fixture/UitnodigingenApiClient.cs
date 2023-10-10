using System.Net.Http.Json;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Requests;

namespace AssociationRegistry.Invitations.Api.Tests.Fixture;

public class UitnodigingenApiClient : IDisposable
{
    private readonly HttpClient _httpClient;

    public UitnodigingenApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    public async Task<HttpResponseMessage> GetRoot() => await _httpClient.GetAsync("/");

    public async Task<HttpResponseMessage> GetUitnodigingenOpVcode(string vCode)
        => await _httpClient.GetAsync($"/v1/verenigingen/{vCode}/uitnodigingen");

    public async Task<HttpResponseMessage> GetUitnodigingsDetail(string insz, Guid uitnodigingsId)
        => await _httpClient.GetAsync($"/v1/personen/{insz}/uitnodigingen/{uitnodigingsId}");

    public async Task<HttpResponseMessage> RegistreerUitnodiging(UitnodigingsRequest uitnodigingsRequest)
        => await _httpClient.PostAsJsonAsync($"/v1/uitnodigingen", uitnodigingsRequest);

    public async Task<HttpResponseMessage> AanvaardUitnodiging(Guid uitnodigingsId)
        => await _httpClient.PostAsync($"/v1/uitnodigingen/{uitnodigingsId}/aanvaard", null);

    public async Task<HttpResponseMessage> WeigerUitnodiging(Guid uitnodigingsId)
        => await _httpClient.PostAsync($"/v1/uitnodigingen/{uitnodigingsId}/weiger", null);

    public async Task<HttpResponseMessage> GetHealth()
        => await _httpClient.GetAsync("/health");

    public async Task<HttpResponseMessage> TrekUitnodigingIn(Guid uitnodigingsId)
        => await _httpClient.PostAsync($"/v1/uitnodigingen/{uitnodigingsId}/trekin", null);
}
