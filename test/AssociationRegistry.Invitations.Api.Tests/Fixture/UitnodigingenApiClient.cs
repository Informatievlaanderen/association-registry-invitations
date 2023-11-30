using System.Net.Http.Json;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Registreer;

namespace AssociationRegistry.Invitations.Api.Tests.Fixture;

using Aanvragen.Registreer;

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

    public async Task<HttpResponseMessage> GetHealth()
        => await _httpClient.GetAsync("/health");

    #region UITNODIGINGEN

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

    public async Task<HttpResponseMessage> TrekUitnodigingIn(Guid uitnodigingId)
        => await _httpClient.PostAsync($"/v1/uitnodigingen/{uitnodigingId}/intrekkingen", null);

    #endregion

    #region AANVRAGEN

    public async Task<HttpResponseMessage> GetAanvragingenOpVcode(string vCode)
        => await _httpClient.GetAsync($"/v1/verenigingen/{vCode}/aanvragen");

    public async Task<HttpResponseMessage> GetAanvraagDetail(string insz, Guid aanvraagId)
        => await _httpClient.GetAsync($"/v1/personen/{insz}/aanvragen/{aanvraagId}");

    public async Task<HttpResponseMessage> RegistreerAanvraag(AanvraagRequest request)
        => await _httpClient.PostAsJsonAsync($"/v1/aanvragen", request);

    public async Task<HttpResponseMessage> AanvaardAanvraag(Guid aanvraagId)
        => await _httpClient.PostAsync($"/v1/aanvragen/{aanvraagId}/aanvaardingen", null);

    public async Task<HttpResponseMessage> WijgerAanvraag(Guid aanvraagId)
        => await _httpClient.PostAsync($"/v1/aanvragen/{aanvraagId}/weigeringen", null);

    public async Task<HttpResponseMessage> TrekAanvraagIn(Guid aanvraagId)
        => await _httpClient.PostAsync($"/v1/aanvragen/{aanvraagId}/intrekkingen", null);

    #endregion

}

public static class UitnodigingenApiClientExtensions
{
    public static async Task<HttpResponseMessage> EnsureSuccessOrThrowForUitnodiging(this Task<HttpResponseMessage> source)
    {
        var response = await source;

        if (!response.IsSuccessStatusCode)
            throw new Exception("Kon uitnodiging niet registreren: \n" + await response.Content.ReadAsStringAsync());

        return response;
    }

    public static async Task<HttpResponseMessage> EnsureSuccessOrThrowForAanvraag(this Task<HttpResponseMessage> source)
    {
        var response = await source;

        if (!response.IsSuccessStatusCode)
            throw new Exception("Kon aanvraag niet registreren: \n" + await response.Content.ReadAsStringAsync());

        return response;
    }
}
