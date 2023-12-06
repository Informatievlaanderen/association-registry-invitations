using System.Net.Http.Json;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Registreer;

namespace AssociationRegistry.Invitations.Api.Tests.Fixture;

using Aanvragen.Registreer;
using Aanvragen.StatusWijziging;
using Newtonsoft.Json;
using System.Text;
using Uitnodigingen.StatusWijziging;

public class TestApiClient : IDisposable
{
    private readonly HttpClient _httpClient;

    public TestApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        Uitnodiging = new UitnodigingApiClient(httpClient);
        Aanvragen = new AanvraagApiClient(httpClient);
    }

    public UitnodigingApiClient Uitnodiging { get; }
    public AanvraagApiClient Aanvragen { get; }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    public async Task<HttpResponseMessage> GetHealth()
        => await _httpClient.GetAsync("/health");

    public async Task<HttpResponseMessage> GetGecombineerdResultaat(string vCode)
        => await _httpClient.GetAsync($"/v1/verenigingen/{vCode}/gecombineerd");

    public class UitnodigingApiClient
    {
        private readonly HttpClient _httpClient;

        public UitnodigingApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> TrekUitnodigingIn(Guid uitnodigingId)
            => await _httpClient.PostAsync($"/v1/uitnodigingen/{uitnodigingId}/intrekkingen", null);

        public async Task<HttpResponseMessage> WeigerUitnodiging(Guid uitnodigingId)
            => await _httpClient.PostAsync($"/v1/uitnodigingen/{uitnodigingId}/weigeringen", null);

        public async Task<HttpResponseMessage> AanvaardUitnodiging(Guid uitnodigingId)
            => await _httpClient.PostAsync($"/v1/uitnodigingen/{uitnodigingId}/aanvaardingen",
                                           null);

        public async Task<HttpResponseMessage> GetUitnodigingenOpVcode(string vCode)
            => await _httpClient.GetAsync($"/v1/verenigingen/{vCode}/uitnodigingen");

        public async Task<HttpResponseMessage> GetUitnodigingsDetail(string insz, Guid uitnodigingId)
            => await _httpClient.GetAsync($"/v1/personen/{insz}/uitnodigingen/{uitnodigingId}");

        public async Task<HttpResponseMessage> RegistreerUitnodiging(UitnodigingsRequest uitnodigingsRequest)
            => await _httpClient.PostAsJsonAsync($"/v1/uitnodigingen", uitnodigingsRequest);


    }

    public class AanvraagApiClient
    {
        private readonly HttpClient _httpClient;

        public AanvraagApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> GetAanvragenOpVcode(string vCode)
            => await _httpClient.GetAsync($"/v1/verenigingen/{vCode}/aanvragen");

        public async Task<HttpResponseMessage> GetAanvraagDetail(string insz, Guid aanvraagId)
            => await _httpClient.GetAsync($"/v1/personen/{insz}/aanvragen/{aanvraagId}");

        public async Task<HttpResponseMessage> RegistreerAanvraag(AanvraagRequest request)
            => await _httpClient.PostAsJsonAsync($"/v1/aanvragen", request);

        public async Task<HttpResponseMessage> AanvaardAanvraag(Guid aanvraagId, WijzigAanvraagStatusRequest request)
            => await _httpClient.PostAsync($"/v1/aanvragen/{aanvraagId}/aanvaardingen", request.ToStringContent());

        public async Task<HttpResponseMessage> WeigerAanvraag(Guid aanvraagId,WijzigAanvraagStatusRequest request)
            => await _httpClient.PostAsync($"/v1/aanvragen/{aanvraagId}/weigeringen", request.ToStringContent());

        public async Task<HttpResponseMessage> TrekAanvraagIn(Guid aanvraagId)
            => await _httpClient.PostAsync($"/v1/aanvragen/{aanvraagId}/intrekkingen", null);
    }

    public async Task<HttpResponseMessage> GetRoot() => await _httpClient.GetAsync("/");
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

    public  static StringContent ToStringContent(this object request)
        => new(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
}
