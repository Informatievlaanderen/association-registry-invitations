using System.Net.Http.Json;
using AssociationRegistry.Invitations.Uitnodingen.Requests;

namespace AssociationRegistry.Invitations.Tests.Fixture;

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
        => await _httpClient.GetAsync($"/uitnodigingen?vcode={vCode}");

    public async Task<HttpResponseMessage> RegistreerUitnodiging(UitnodigingsRequest uitnodigingsRequest)
        => await _httpClient.PostAsJsonAsync($"/uitnodigingen", uitnodigingsRequest);
}
