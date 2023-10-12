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

    public async Task<HttpResponseMessage> GetUitnodigingsDetail(string insz, Guid uitnodigingId)
        => await _httpClient.GetAsync($"/v1/personen/{insz}/uitnodigingen/{uitnodigingId}");

    public async Task<HttpResponseMessage> RegistreerUitnodiging(UitnodigingsRequest uitnodigingsRequest)
        => await _httpClient.PostAsJsonAsync($"/v1/uitnodigingen", uitnodigingsRequest);

    public async Task<HttpResponseMessage> AanvaardUitnodiging(Guid uitnodigingId)
        => await _httpClient.PostAsync($"/v1/uitnodigingen/{uitnodigingId}/aanvaardingen", null);

    public async Task<HttpResponseMessage> WeigerUitnodiging(Guid uitnodigingId)
        => await _httpClient.PostAsync($"/v1/uitnodigingen/{uitnodigingId}/weigeringen", null);

    public async Task<HttpResponseMessage> GetHealth()
        => await _httpClient.GetAsync("/health");

    public async Task<HttpResponseMessage> TrekUitnodigingIn(Guid uitnodigingId)
        => await _httpClient.PostAsync($"/v1/uitnodigingen/{uitnodigingId}/intrekkingen", null);
}
